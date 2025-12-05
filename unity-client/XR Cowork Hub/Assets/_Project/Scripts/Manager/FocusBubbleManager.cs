using UnityEngine;

public class FocusBubbleManager : MonoBehaviour
{
    public static FocusBubbleManager Instance { get; private set; }

    [Header("Currently selected target user")]
    public string currentTargetParticipantId = null;

    private void Awake()
    {
        // Simple Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /*
    Sets the currently selected participant ID.
    This is used later when creating the Focus Bubble.
    */
    public void SetTarget(string userId)
    {
        currentTargetParticipantId = userId;
        Debug.Log("[FocusBubble] New target set: " + userId);
    }
}
