using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace _Project.Scripts.Configuration
{
    public class XRModelButton : MonoBehaviour
    {
        [Header("XR")] public XRBaseInteractable interactable;

        [Header("Images")] public Image bg;
        public Image hover;
        public Image selected;
        public Image orbImage;

        [Header("Alphas")] [Range(0f, 1f)] public float hoverAlpha = 1f;
        [Range(0f, 1f)] public float selectedAlpha = 1f;

        private float _hoverBaseAlpha;
        private float _selectedBaseAlpha;

        public event Action<XRModelButton, bool> OnSelectionChanged;

        public bool isSelected { get; private set; }

        private void Reset()
        {
            if (interactable == null)
                interactable = GetComponent<XRBaseInteractable>();

            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                switch (img.gameObject.name)
                {
                    case "bg":
                        bg = img;
                        break;
                    case "hover":
                        hover = img;
                        break;
                    case "selected":
                        selected = img;
                        break;
                    case "Image":
                        orbImage = img;
                        break;
                }
            }
        }

        private void Awake()
        {
            if (interactable == null)
                interactable = GetComponent<XRBaseInteractable>();
        }

        private void OnEnable()
        {
            if (interactable == null) return;

            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (interactable == null) return;

            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelectEnter);
            interactable.activated.RemoveListener(OnActivated);
        }

        private void Start()
        {
            if (hover != null)
            {
                _hoverBaseAlpha = hover.color.a <= 0f ? 1f : hover.color.a;
                SetHover(false);
            }

            if (selected != null)
            {
                _selectedBaseAlpha = selected.color.a <= 0f ? 1f : selected.color.a;
                SetSelected(false);
            }
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            SetHover(true);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            SetHover(false);
        }

        private void OnSelectEnter(SelectEnterEventArgs args)
        {
            ToggleSelected("SELECT");
        }

        private void OnActivated(ActivateEventArgs args)
        {
            ToggleSelected("ACTIVATE");
        }

        private void ToggleSelected(string source)
        {
            isSelected = !isSelected;
            Debug.Log($"{name} -> {source} | Selected: {isSelected}");

            SetSelected(isSelected);

            OnSelectionChanged?.Invoke(this, isSelected);
        }

        public void SetSelectedFromManager(bool active)
        {
            isSelected = active;
            SetSelected(active);
        }

        private void SetHover(bool active)
        {
            if (hover == null) return;

            var c = hover.color;
            c.a = active ? _hoverBaseAlpha * hoverAlpha : 0f;
            hover.color = c;
        }

        private void SetSelected(bool active)
        {
            if (selected == null) return;

            var c = selected.color;
            c.a = active ? _selectedBaseAlpha * selectedAlpha : 0f;
            selected.color = c;
        }
    }
}