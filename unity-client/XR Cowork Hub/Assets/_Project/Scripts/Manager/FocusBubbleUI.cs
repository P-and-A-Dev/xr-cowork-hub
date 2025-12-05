using UnityEngine;
using UnityEngine.UI;

public class FocusBubbleUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button focusBubbleButton;
    public Button backToGlobalButton;

    private void Start()
    {
        if (FocusBubbleManager.Instance == null)
        {
            Debug.LogError("[FocusBubbleUI] Missing FocusBubbleManager in the scene!");
            return;
        }

        // Connect button events
        focusBubbleButton.onClick.AddListener(OnFocusBubblePressed);
        backToGlobalButton.onClick.AddListener(OnBackToGlobalPressed);
    }

    private void OnFocusBubblePressed()
    {
        FocusBubbleManager.Instance.CreateFocusBubble();
    }

    private void OnBackToGlobalPressed()
    {
        FocusBubbleManager.Instance.LeaveFocusBubble();
    }
}
