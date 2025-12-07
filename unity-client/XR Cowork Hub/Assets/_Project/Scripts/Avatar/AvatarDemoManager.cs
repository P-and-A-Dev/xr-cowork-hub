using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Manager;
using UnityEngine;

namespace _Project.Scripts.Avatar
{
    public class AvatarDemoManager : MonoBehaviour
    {
        [Header("Dependencies")] public ParticipantManager participantManager;
        [Header("Avatar Prefab")] public GameObject avatarPrefab;

        private readonly Dictionary<string, GameObject> _avatars = new();


        private void Start()
        {
            participantManager.OnParticipantJoined += OnParticipantJoined;
            participantManager.OnParticipantLeft += OnParticipantLeft;
            participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
        }


        private void OnParticipantJoined(Participant p)
        {
            Debug.Log("[AvatarDemo] Spawn avatar for: " + p.userId);

            if (_avatars.ContainsKey(p.userId))
                return;
            GameObject avatar = Instantiate(avatarPrefab, RandomSpawnPosition(), Quaternion.identity);
            _avatars[p.userId] = avatar;
            UpdateAvatarVisual(p);
        }

        private void OnParticipantLeft(Participant p)
        {
            Debug.Log("[AvatarDemo] Despawn avatar for: " + p.userId);

            if (!_avatars.TryGetValue(p.userId, out var avatar))
                return;

            Destroy(avatar);
            _avatars.Remove(p.userId);
        }

        public void OnParticipantsUpdated(List<Participant> list)
        {
            foreach (var p in list.Where(p => _avatars.ContainsKey(p.userId)))
                UpdateAvatarVisual(p);
        }

       private void UpdateAvatarVisual(Participant p)
{
    GameObject avatar = _avatars[p.userId];

    // Update Name Tag
    var nameTag = avatar.GetComponentInChildren<TextMesh>();
    if (nameTag)
        nameTag.text = p.displayName;

    // Get mesh renderer
    var rendererComponent = avatar.GetComponentInChildren<Renderer>();
    if (rendererComponent == null) return;

    Color baseColor = p.isOnline ? Color.white : Color.gray;
    bool sameGroup = (p.voiceGroupId == participantManager.localParticipant.voiceGroupId);

    if (!sameGroup)
    {
        // If NOT same voice group  apply ghost transparency
        baseColor.a = 0.25f;
    }
    else
    {
       // If same voice group fully visible
        baseColor.a = 1f;
    }

    rendererComponent.material.color = baseColor;

    if (p.inBubbleSpace)
    {
        avatar.transform.localScale = Vector3.one * 0.8f;
        avatar.transform.position = new Vector3(0, 0, 3f); 
    }
    else
    {
        avatar.transform.localScale = Vector3.one;
    }
}
        private Vector3 RandomSpawnPosition()
        {
            return new Vector3(
                Random.Range(-2f, 2f),
                0f,
                Random.Range(-2f, 2f)
            );
        }
    }
}