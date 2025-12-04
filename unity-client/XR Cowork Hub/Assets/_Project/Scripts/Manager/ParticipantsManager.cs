/*
 PARTICIPANT MANAGER – API FOR VISUAL / AVATARS SYSTEM.
 ------------------------------------------------------
 EVENTS AVAILABLE
----------------

1) OnParticipantsUpdated(List<Participant>)
   -> Triggered whenever the full list of participants changes (Firestore update).
   -> Use this for full sync operations (UI lists, refreshing all avatars, etc.).

2) OnParticipantJoined(Participant)
   -> Triggered when a new participant appears in Firestore.
   -> Use this to spawn a new avatar.

3) OnParticipantLeft(Participant)
   -> Triggered when a participant disappears from Firestore.
   -> Use this to despawn/destroy an avatar or remove UI elements.


PARTICIPANT FIELDS IMPORTANT FOR VISUAL/AVATARS
-----------------------------------------------

displayName
    The name displayed above the avatar or in participant lists.

isOnline
    true  -> participant is active (headset on / app open)
    false -> participant left the app or removed the headset
    Use this to:
    - fade out the avatar
    - mark user as offline
    - show offline indicators

lastSeen
    Updated every 3 seconds by PresenceHeartbeat().
    Useful for:
    - detecting frozen users (network issues)
    - showing time since last activity
    - triggering idle animations

voiceGroupId
    Defines Focus Bubble / audio grouping.
    Rules:
      - 0 = public room
      - equal to my voiceGroupId = I hear and see this user normally
      - different = this user should appear as "ghost" and be muted

    Typical usage:
        bool sameGroup = (other.voiceGroupId == my.voiceGroupId);
        avatar.SetGhost(!sameGroup);
        audio.SetMuted(!sameGroup);

inBubbleSpace
    true = participant is inside a Private 3D Bubble Space.
    Use this to:
    - teleport/move avatar into the bubble environment
    - hide avatar from the main room
    - enable bubble-only visuals or effects


TYPICAL AVATAR MANAGER SUBSCRIPTIONS
------------------------------------

participantsManager.OnParticipantJoined += SpawnAvatar;
participantsManager.OnParticipantLeft    += DespawnAvatar;
participantsManager.OnParticipantsUpdated += UpdateAvatarData;
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
        public FirestoreService firestore;
        public event Action<List<Participant>> OnParticipantsUpdated;

        public event Action<Participant> OnParticipantJoined;

        public event Action<Participant> OnParticipantLeft;
        private Dictionary<string, Participant> _previousParticipants = new();

        private string _roomId = "test-room";
        private string _userId;
        private Participant _localParticipant;
        public Participant localParticipant => _localParticipant;

        private void Start()
        {
            LoadOrCreateUserId();
            CreateOrUpdateLocalParticipant();
            ListenToParticipants();
            StartCoroutine(PresenceHeartbeat());
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