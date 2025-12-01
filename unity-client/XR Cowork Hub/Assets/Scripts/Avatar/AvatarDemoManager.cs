using UnityEngine;
using System.Collections.Generic;

public class AvatarDemoManager : MonoBehaviour
{
    [Header("Dependencies")]
    public ParticipantManager participantManager;   
    [Header("Avatar Prefab")]
    public GameObject avatarPrefab;

    // Store active avatars by userId
    private Dictionary<string, GameObject> avatars = new Dictionary<string, GameObject>();


    void Start()
    {
        // Subscribe to ParticipantManager events
        participantManager.OnParticipantJoined += OnParticipantJoined;
        participantManager.OnParticipantLeft += OnParticipantLeft;
        participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
    }


    // SPAWN — when a user joins
    void OnParticipantJoined(Participant p)
    {
        Debug.Log("[AvatarDemo] Spawn avatar for: " + p.userId);

        if (avatars.ContainsKey(p.userId))
            return; 
        GameObject avatar = Instantiate(avatarPrefab, RandomSpawnPosition(), Quaternion.identity);
        avatars[p.userId] = avatar;
        UpdateAvatarVisual(p);
    }

    // despawn when a user leaves
    void OnParticipantLeft(Participant p)
    {
        Debug.Log("[AvatarDemo] Despawn avatar for: " + p.userId);

        if (!avatars.ContainsKey(p.userId))
            return;

        Destroy(avatars[p.userId]);
        avatars.Remove(p.userId);
    }

    void OnParticipantsUpdated(List<Participant> list)
    {
        foreach (var p in list)
            if (avatars.ContainsKey(p.userId))
                UpdateAvatarVisual(p);
    }
    void UpdateAvatarVisual(Participant p)
    {
        GameObject avatar = avatars[p.userId];

        // Name tag
        var tag = avatar.GetComponentInChildren<TextMesh>();
        if (tag)
            tag.text = p.displayName;

        //Online/Offline
        var renderer = avatar.GetComponentInChildren<Renderer>();
        if (renderer)
            renderer.material.color = p.isOnline ? Color.white : Color.gray;

        //focus Bubble / Ghost Mode
        bool sameGroup = (p.voiceGroupId == participantManager.LocalParticipant.voiceGroupId);

        // example rule for non-group users become transparent/ghost
        if (renderer)
            renderer.material.color = sameGroup ? Color.white : new Color(1,1,1,0.25f);

        // Example for  move avatar into bubble space
        if (p.inBubbleSpace)
        {
            avatar.transform.localScale = Vector3.one * 0.8f;
            avatar.transform.position = new Vector3(0, 0, 3f); // fake bubble position
        }
        else
        {
            avatar.transform.localScale = Vector3.one;
        }
    }
// Random spawn position
    Vector3 RandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(-2f, 2f),
            0f,
            Random.Range(-2f, 2f)
        );
    }
}
