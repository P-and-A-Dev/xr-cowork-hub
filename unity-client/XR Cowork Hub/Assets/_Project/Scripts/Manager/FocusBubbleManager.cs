using UnityEngine;
using System;
using System.Collections.Generic;
using _Project.Scripts.Manager;

public class FocusBubbleManager : MonoBehaviour
{
    public static FocusBubbleManager Instance { get; private set; }

    [Header("Selected participant")]
    public string currentTargetParticipantId;

    [Header("Dependencies")]
    public FirestoreService firestoreService;
    public ParticipantManager participantManager;
    public AgoraVoiceManager agoraVoiceManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetTarget(string userId)
    {
        currentTargetParticipantId = userId;
        Debug.Log("[FocusBubble] Target selected: " + userId);
    }

    /*
     Creates a Focus Bubble between the local user and the selected participant.
     Uses FirestoreService.UpdateVoiceGroup() with a batch update.
    */
    public async void CreateFocusBubble()
    {
        if (string.IsNullOrEmpty(currentTargetParticipantId))
        {
            Debug.LogWarning("[FocusBubble] Cannot create bubble — no target selected.");
            return;
        }

        string localUserId = participantManager.LocalParticipantId;
        string roomId = participantManager.RoomId;

        long newVoiceGroupId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Debug.Log("[FocusBubble] Creating bubble with ID: " + newVoiceGroupId);

        // using your FirestoreService method
        List<string> users = new List<string>
        {
            localUserId,
            currentTargetParticipantId
        };

        await firestoreService.UpdateVoiceGroup(roomId, users, newVoiceGroupId);

        participantManager.LocalParticipant.voiceGroupId = newVoiceGroupId;

        if (agoraVoiceManager != null)
        {
            agoraVoiceManager.JoinGroup(newVoiceGroupId);
        }

        currentTargetParticipantId = null;

        Debug.Log("[FocusBubble] Bubble created successfully!");
    }

    public async void LeaveFocusBubble()
    {
        string localUserId = participantManager.LocalParticipantId;
        string roomId = participantManager.RoomId;

        Debug.Log("[FocusBubble] Leaving focus bubble...");

        // update the local participant to global group 0
        List<string> users = new List<string> { localUserId };

        await firestoreService.UpdateVoiceGroup(roomId, users, 0);
        participantManager.LocalParticipant.voiceGroupId = 0;

        if (agoraVoiceManager != null)
        {
            agoraVoiceManager.JoinGroup(0);
        }
        currentTargetParticipantId = null;

        Debug.Log("[FocusBubble] You are now back in the global voice group.");
    }
}
