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

public class ParticipantManager : MonoBehaviour
{
    public FirestoreService firestore;
    public event Action<List<Participant>> OnParticipantsUpdated;

    //add to avatarverso integration
    public event Action<Participant> OnParticipantJoined;   // send a call when someone go in
    public event Action<Participant> OnParticipantLeft;     //send a sinal when some one go out 
    //// Stores the previous list of participants to detect changes
    private Dictionary<string, Participant> previousParticipants = new Dictionary<string, Participant>();

    private string roomId = "test-room";
    private string userId;
    private Participant localParticipant;
    public Participant LocalParticipant => localParticipant;
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

    // Create or update the participant document in firestore
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


        // create new dicionary
        Dictionary<string, Participant> currentDict = new Dictionary<string, Participant>();
        foreach (var p in participants)
            currentDict[p.userId] = p;

        // Listen who enter
        foreach (var kv in currentDict)
        {
            if (!previousParticipants.ContainsKey(kv.Key))
            {
                Debug.Log($"[Participants] JOINED → {kv.Key}");
                OnParticipantJoined?.Invoke(kv.Value);
            }
        }

        // listen who go uot
        foreach (var kv in previousParticipants)
        {
            if (!currentDict.ContainsKey(kv.Key))
            {
                Debug.Log($"[Participants] LEFT → {kv.Key}");
                OnParticipantLeft?.Invoke(kv.Value);
            }
        }

        // Update 
        previousParticipants = currentDict;
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


    // cleanup to evitate ghosts 
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


    //applications quit
    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit -> Cleaning up presence...");
        CleanupPresence();
    }

    //standybY when you take off the glasses
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("OnApplicationPause -> Cleaning up presence...");
            CleanupPresence();
        }
    }
}
