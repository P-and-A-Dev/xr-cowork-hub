using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;

public class ParticipantManager : MonoBehaviour
{
    public FirestoreService firestore;
    public event Action<List<Participant>> OnParticipantsUpdated;
    private string roomId = "test-room";
    private string userId;
    private Participant localParticipant;

    void Start()
    {
        LoadOrCreateUserId();               
        CreateOrUpdateLocalParticipant();   
        ListenToParticipants();             
        StartCoroutine(PresenceHeartbeat()); // Keep updating presence
    }

    void LoadOrCreateUserId()
    {
        if (PlayerPrefs.HasKey("userId"))
            userId = PlayerPrefs.GetString("userId");
        else
        {
            userId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("userId", userId);
        }

        Debug.Log("Local userId: " + userId);
    }

    //Create or update the participant document in firestore
    async void CreateOrUpdateLocalParticipant()
    {
        localParticipant = new Participant
        {
            userId = userId,
            displayName = "Test User",
            voiceGroupId = 0,
            isOnline = true,
            inBubbleSpace = false,
            lastSeen = Timestamp.GetCurrentTimestamp()
        };

        string path = $"rooms/{roomId}/participants/{userId}";
        await firestore.SetDocument(path, localParticipant);

        Debug.Log("Participant document created or updated.");
    }

    // List participants in the room
    void ListenToParticipants()
    {
        firestore.ListenCollection<Participant>(
            $"rooms/{roomId}/participants",
            OnParticipantsChanged
        );
    }

    // Callback
    void OnParticipantsChanged(List<Participant> participants)
    {
        Debug.Log("Participants updated. Total: " + participants.Count);
        OnParticipantsUpdated?.Invoke(participants);
        foreach (var p in participants)
        {
            Debug.Log($"{p.displayName} | online={p.isOnline} | group={p.voiceGroupId}");
        }
    }

    // Update presence every 'x' seconds
    IEnumerator PresenceHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            string path = $"rooms/{roomId}/participants/{userId}";

            firestore.SetDocument(path, new
            {
                lastSeen = Timestamp.GetCurrentTimestamp(),
                isOnline = true
            });
        }
    }
}
