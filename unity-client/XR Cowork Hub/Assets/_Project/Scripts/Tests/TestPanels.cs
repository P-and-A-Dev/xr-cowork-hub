using _Project.Scripts.Manager;
using UnityEngine;

namespace _Project.Scripts.Tests
{
    public class PanelTest : MonoBehaviour
    {
        public PanelManager panelManager;

        private void Start()
        {
            panelManager.CreatePanel("note", "firrts panel");
        }
    }
}