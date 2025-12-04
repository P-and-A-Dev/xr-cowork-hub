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
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;

/// <summary>
/// Manages all participant-related behaviors such as presence,
/// Firestore syncing, join/leave detection and local user state.
/// </summary>
public class ParticipantManager : MonoBehaviour
{
    public FirestoreService firestore;
    public event Action<List<Participant>> OnParticipantsUpdated;

    public event Action<Participant> OnParticipantJoined;
    public event Action<Participant> OnParticipantLeft;

    private Dictionary<string, Participant> previousParticipants = new Dictionary<string, Participant>();
    private ListenerRegistration participantsListener;

    [SerializeField] private string roomId = "main-room";
    [SerializeField] private float heartbeatInterval = 3f;

    private string userId;
    private Participant localParticipant;
    public Participant LocalParticipant => localParticipant;

    /// <summary>
    /// Initializes local user, sets presence, listens to Firestore participants
    /// and starts the presence heartbeat routine.
    /// </summary>
    void Start()
    {
        LoadOrCreateUserId();               
        CreateOrUpdateLocalParticipant();   
        ListenToParticipants();             
        StartCoroutine(PresenceHeartbeat());
    }

    /// <summary>
    /// Loads or creates a stable local userId.
    /// </summary>
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

    /// <summary>
    /// Creates or updates the local participant document in Firestore.
    /// </summary>
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

    /// <summary>
    /// Begins listening to participant updates from Firestore.
    /// </summary>
    void ListenToParticipants()
    {
        participantsListener = firestore.ListenCollection<Participant>(
            $"rooms/{roomId}/participants",
            OnParticipantsChanged
        );
    }

    /// <summary>
    /// Handles participant list changes, join/leave detection,
    /// and fires the OnParticipantsUpdated event.
    /// </summary>
    void OnParticipantsChanged(List<Participant> participants)
    {
        Debug.Log("Participants updated. Total: " + participants.Count);
        OnParticipantsUpdated?.Invoke(participants);
        
        Dictionary<string, Participant> currentDict = new Dictionary<string, Participant>();
        foreach (var p in participants)
            currentDict[p.userId] = p;

        foreach (var kv in currentDict)
            if (!previousParticipants.ContainsKey(kv.Key))
                OnParticipantJoined?.Invoke(kv.Value);

        foreach (var kv in previousParticipants)
            if (!currentDict.ContainsKey(kv.Key))
                OnParticipantLeft?.Invoke(kv.Value);

        previousParticipants = currentDict;
    }

    /// <summary>
    /// Repeatedly updates presence and lastSeen timestamps on Firestore.
    /// </summary>
    IEnumerator PresenceHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(heartbeatInterval);

            string path = $"rooms/{roomId}/participants/{userId}";

            firestore.SetDocument(path, new
            {
                lastSeen = Timestamp.GetCurrentTimestamp(),
                isOnline = true
            });
        }
    }

    async void CleanupPresence()
    {
        string path = $"rooms/{roomId}/participants/{userId}";

        await firestore.SetDocument(path, new
        {
            isOnline = false,
            voiceGroupId = 0,
            inBubbleSpace = false,
            lastSeen = Timestamp.GetCurrentTimestamp()
        });

        Debug.Log("CleanupPresence: Participant marked offline.");
    }

    void OnApplicationQuit()
    {
        CleanupPresence();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            CleanupPresence();
    }

    void OnDestroy()
    {
        participantsListener?.Stop();
    }
}
