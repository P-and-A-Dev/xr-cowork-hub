/*
 PARTICIPANT MANAGER – API FOR VISUAL / AVATARS SYSTEM.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class ParticipantManager : MonoBehaviour
    {
        [SerializeField] public FirestoreService firestore;
        public event Action<List<Participant>> OnParticipantsUpdated;

        public event Action<Participant> OnParticipantJoined;

        public event Action<Participant> OnParticipantLeft;
        private Dictionary<string, Participant> _previousParticipants = new();

        private string _roomId = "test-room";
        private string _userId;
        private Participant _localParticipant;
        public Participant localParticipant => _localParticipant;
        public string LocalParticipantId => _userId;
        public string RoomId => _roomId;

        private IEnumerator Start()
        {
            if (firestore == null)
            {
                Debug.LogError("[ParticipantsManager] Critical: FirestoreService reference is missing! Please assign it in the Inspector.");
                yield break;
            }

            Debug.Log("[ParticipantsManager] Waiting for FirestoreService...");
            yield return new WaitUntil(() => firestore.IsInitialized);

            try
            {
                LoadOrCreateUserId();
                CreateOrUpdateLocalParticipant();
                ListenToParticipants();
                StartCoroutine(PresenceHeartbeat());
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ParticipantsManager] Error in Start: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadOrCreateUserId()
        {
            if (PlayerPrefs.HasKey("userId"))
                _userId = PlayerPrefs.GetString("userId");
            else
            {
                _userId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("userId", _userId);
            }

            Debug.Log("Local userId: " + _userId);
        }

        private async void CreateOrUpdateLocalParticipant()
        {
            try
            {
                _localParticipant = new Participant
                {
                    userId = _userId,
                    displayName = "Test User",
                    voiceGroupId = 0,
                    isOnline = true,
                    inBubbleSpace = false,
                    lastSeen = Timestamp.GetCurrentTimestamp()
                };

                string path = $"rooms/{_roomId}/participants/{_userId}";
                await firestore.SetDocument(path, _localParticipant);

                Debug.Log("Participant document created or updated.");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ListenToParticipants()
        {
            firestore.ListenCollection<Participant>(
                $"rooms/{_roomId}/participants",
                OnParticipantsChanged
            );
        }

        private void OnParticipantsChanged(List<Participant> participants)
        {
            Debug.Log("Participants updated. Total: " + participants.Count);
            OnParticipantsUpdated?.Invoke(participants);

            foreach (var p in participants)
            {
                Debug.Log($"{p.displayName} | online={p.isOnline} | group={p.voiceGroupId}");
            }

            Dictionary<string, Participant> currentDict = new Dictionary<string, Participant>();
            foreach (var p in participants)
                currentDict[p.userId] = p;


            foreach (var kv in currentDict.Where(kv => !_previousParticipants.ContainsKey(kv.Key)))
            {
                Debug.Log($"[Participants] JOINED → {kv.Key}");
                OnParticipantJoined?.Invoke(kv.Value);
            }

            foreach (var kv in _previousParticipants.Where(kv => !currentDict.ContainsKey(kv.Key)))
            {
                Debug.Log($"[Participants] LEFT → {kv.Key}");
                OnParticipantLeft?.Invoke(kv.Value);
            }

            _previousParticipants = currentDict;
        }

        private IEnumerator PresenceHeartbeat()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);

                string path = $"rooms/{_roomId}/participants/{_userId}";

                firestore.SetDocument(path, new
                {
                    lastSeen = Timestamp.GetCurrentTimestamp(),
                    isOnline = true
                });
            }
        }

        private async void CleanupPresence()
        {
            try
            {
                string path = $"rooms/{_roomId}/participants/{_userId}";

                await firestore.SetDocument(path, new
                {
                    isOnline = false,
                    voiceGroupId = 0,
                    inBubbleSpace = false,
                    lastSeen = Timestamp.GetCurrentTimestamp()
                });

                Debug.Log("CleanupPresence: Participant marked offline.");
            }
            catch (Exception)
            {
                // ignored
            }
        }


        //applications quit
        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit -> Cleaning up presence...");
            CleanupPresence();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause) return;
            Debug.Log("OnApplicationPause -> Cleaning up presence...");
            CleanupPresence();
        }
    }
}