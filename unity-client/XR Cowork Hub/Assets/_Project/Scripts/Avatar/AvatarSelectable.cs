using UnityEngine;

public class AvatarSelectable : MonoBehaviour
{
    [Header("Linked Participant")]
    public string userId; // Assigned when the avatar is spawned
    // called when the avatar is selected (via raycast or interaction).
    public void OnSelected()
    {
        FocusBubbleManager.Instance.SetTarget(userId);
        Debug.Log("[FocusBubble] Target selected: " + userId);
    }
}
