using UnityEngine;
using UnityEngine.EventSystems;
using _Project.Scripts.Manager;

namespace _Project.Scripts.Avatar
{
    /*
     ParticipantOrb: component attached to the visual "orb" representing a participant.
     - Expose participantId so it can be assigned during avatar spawn/initialization.
     - Call OnSelected() when the raycast detects the orb.
    - Implements IPointerClickHandler for compatibility with UI-based raycasts.
    */
    public class ParticipantOrb : MonoBehaviour, IPointerClickHandler
    {
        [Header("Participant Data")]
        [Tooltip("Participant ID (userId).")]
        public string participantId;

        [Tooltip("Displayed name (optional).")]
        public string displayName;

        [Header("References")]
        [Tooltip("Object used for highlight visuals (enabled/disabled).")]
        public GameObject highlightObject;

        [Tooltip("Optional: renderers that will receive tint colors when selected.")]
        public Renderer[] renderersToTint;

        [Tooltip("Tint color applied when this orb is selected.")]
        public Color selectedTint = Color.cyan;

        [Tooltip("Tint color applied when this orb is not selected.")]
        public Color normalTint = Color.white;

        // Cached reference to the focus bubble manager
        private FocusBubbleManager focusBubbleManager;

        // Not used globally; reserved for possible future selection chaining
        private ParticipantOrb previouslySelected;

        private bool isHighlighted = false;

        private void Awake()
        {
    
            if (FocusBubbleManager.Instance != null)
                focusBubbleManager = FocusBubbleManager.Instance;
        }

        private void Start()
        {
            ApplyTint(normalTint);
            SetHighlight(false);
        }
        // Called by the Raycast/Interactor when the orb is "click"
        public void OnSelected()
        {
            if (string.IsNullOrEmpty(participantId))
            {
                Debug.LogWarning("[ParticipantOrb] participantId is empty on " + gameObject.name);
                return;
            }
            if (focusBubbleManager == null)
            {
                Debug.LogWarning("[ParticipantOrb] FocusBubbleManager not found in scene.");
            }
            else
            {
                focusBubbleManager.SetTarget(participantId);
            }
            ApplyLocalSelectionVisual();
            Debug.Log("[ParticipantOrb] Selected participant: " + participantId);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelected();
        }
        // Triggers the highlight visuals when this orb is selected.
        // If you want only one orb highlighted at a time, store the previously selected orb in the manager and call ClearSelectionVisual() on it.
        private void ApplyLocalSelectionVisual()
        {
            SetHighlight(true);
            ApplyTint(selectedTint);
        }
        // Clears selection visuals — use when another orb becomes the active target.
        // This is not automatically handled here; the manager should call this on the previously selected orb.
        public void ClearSelectionVisual()
        {
            SetHighlight(false);
            ApplyTint(normalTint);
        }

        private void SetHighlight(bool enable)
        {
            isHighlighted = enable;
            if (highlightObject != null)
                highlightObject.SetActive(enable);
        }

        private void ApplyTint(Color tint)
        {
            if (renderersToTint == null || renderersToTint.Length == 0) return;

            foreach (var r in renderersToTint)
            {
                if (r == null) continue;

                if (r.material != null && r.material.HasProperty("_Color"))
                {
                    r.material.color = tint;
                }
            }
        }
        // Utility function: initializes this orb's participant data when spawned.
        public void Initialize(string userId, string name = null)
        {
            participantId = userId;

            if (!string.IsNullOrEmpty(name))
                displayName = name;
        }
    }
}

