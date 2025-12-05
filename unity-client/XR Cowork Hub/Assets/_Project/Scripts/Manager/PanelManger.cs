using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class PanelManager : MonoBehaviour
    {
        public FirestoreService firestore;
        public string roomId = "test-room";

        public readonly Action<List<PanelData>> onPanelsChanged;

        public PanelManager(Action<List<PanelData>> onPanelsChanged)
        {
            this.onPanelsChanged = onPanelsChanged;
        }

        private void Start()
        {
            ListenToPanels();
        }

        private void ListenToPanels()
        {
            firestore.ListenCollection<PanelData>(
                $"rooms/{roomId}/panels",
                OnPanelsCollectionChanged
            );

            Debug.Log("PanelManager: Listening panels");
        }

        private void OnPanelsCollectionChanged(List<PanelData> panels)
        {
            Debug.Log("Update Panels. Total: " + panels.Count);


            onPanelsChanged?.Invoke(panels);
        }


        public async void CreatePanel(string type, string title)
        {
            try
            {
                string panelId = Guid.NewGuid().ToString();

                PanelData panel = new PanelData
                {
                    panelId = panelId,
                    type = type,
                    title = title,
                    content = "",
                    visibility = "room",
                    bubbleGroupId = 0
                };

                string path = $"rooms/{roomId}/panels/{panelId}";
                await firestore.SetDocument(path, panel);

                Debug.Log("Panel created: " + panelId);
            }
            catch (Exception)
            {
                // ignored
            }
        }


        public async void UpdatePanelContent(string panelId, string newContent)
        {
            try
            {
                string path = $"rooms/{roomId}/panels/{panelId}";

                await firestore.SetDocument(path, new
                {
                    content = newContent
                });

                Debug.Log("Update Panel: " + panelId);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async void DeletePanel(string panelId)
        {
            try
            {
                string path = $"rooms/{roomId}/panels/{panelId}";

                await FirebaseFirestore.DefaultInstance.Document(path).DeleteAsync();

                Debug.Log("Panel remove: " + panelId);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}