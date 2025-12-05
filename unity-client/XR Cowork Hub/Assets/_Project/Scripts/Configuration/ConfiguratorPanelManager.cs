using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Configuration
{
    public class ConfiguratorPanelManager : MonoBehaviour
    {
        [Header("Button Groups")] [Tooltip("Parent for orb buttons (B1, B2, B3)")]
        public Transform orbsParent;

        [Tooltip("Parent for style buttons (Fill, Empty)")]
        public Transform stylesParent;

        public XRModelButton currentOrbSelected => _currentOrbSelected;
        public XRModelButton currentStyleSelected => _currentStyleSelected;

        private readonly List<XRModelButton> _orbButtons = new();
        private readonly List<XRModelButton> _styleButtons = new();

        private XRModelButton _currentOrbSelected;
        private XRModelButton _currentStyleSelected;

        private void Start()
        {
            InitOrbGroup();
            InitStyleGroup();
        }

        private void InitOrbGroup()
        {
            if (orbsParent == null)
                return;

            var buttons = orbsParent.GetComponentsInChildren<XRModelButton>(true);

            foreach (var button in buttons)
            {
                if (button == null) continue;
                if (_orbButtons.Contains(button)) continue;

                _orbButtons.Add(button);
                button.OnSelectionChanged += HandleOrbSelectionChanged;

                if (!button.isSelected) continue;

                if (_currentOrbSelected == null)
                    _currentOrbSelected = button;
                else
                    button.SetSelectedFromManager(false);
            }

            if (_currentOrbSelected != null || _orbButtons.Count <= 0) return;
            _currentOrbSelected = _orbButtons[0];
            _currentOrbSelected.SetSelectedFromManager(true);
        }

        private void InitStyleGroup()
        {
            if (stylesParent == null)
                return;

            var buttons = stylesParent.GetComponentsInChildren<XRModelButton>(true);

            foreach (var button in buttons)
            {
                if (button == null) continue;
                if (_styleButtons.Contains(button)) continue;

                _styleButtons.Add(button);
                button.OnSelectionChanged += HandleStyleSelectionChanged;

                if (!button.isSelected) continue;

                if (_currentStyleSelected == null)
                    _currentStyleSelected = button;
                else
                    button.SetSelectedFromManager(false);
            }

            if (_currentStyleSelected != null || _styleButtons.Count <= 0) return;
            _currentStyleSelected = _styleButtons[0];
            _currentStyleSelected.SetSelectedFromManager(true);
        }

        private void OnDestroy()
        {
            foreach (var btn in _orbButtons.Where(btn => btn != null))
                btn.OnSelectionChanged -= HandleOrbSelectionChanged;

            foreach (var btn in _styleButtons.Where(btn => btn != null))
                btn.OnSelectionChanged -= HandleStyleSelectionChanged;
        }

        private void HandleOrbSelectionChanged(XRModelButton button, bool isSelected)
        {
            if (!isActiveAndEnabled || button == null)
                return;

            if (!isSelected)
            {
                if (_currentOrbSelected == button)
                    _currentOrbSelected = null;
                return;
            }

            if (_currentOrbSelected != null && _currentOrbSelected != button)
                _currentOrbSelected.SetSelectedFromManager(false);

            _currentOrbSelected = button;
        }

        private void HandleStyleSelectionChanged(XRModelButton button, bool isSelected)
        {
            if (!isActiveAndEnabled || button == null)
                return;

            if (!isSelected)
            {
                if (_currentStyleSelected == button)
                    _currentStyleSelected = null;
                return;
            }

            if (_currentStyleSelected != null && _currentStyleSelected != button)
                _currentStyleSelected.SetSelectedFromManager(false);

            _currentStyleSelected = button;
        }
    }
}