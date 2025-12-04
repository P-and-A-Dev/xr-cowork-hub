using UnityEngine;
using System.Collections.Generic;
using System;
using Firebase.Firestore;

public class PanelManager : MonoBehaviour
{
    public FirestoreService firestore;
    public string roomId = "test-room";

    public Action<List<PanelData>> OnPanelsChanged;

    void Start()
    {
        ListenToPanels();
    }

    void ListenToPanels()
    {
        firestore.ListenCollection<PanelData>(
            $"rooms/{roomId}/panels",
            OnPanelsCollectionChanged
        );

        Debug.Log("PanelManager: Listening panels");
    }

    void OnPanelsCollectionChanged(List<PanelData> panels)
    {
        Debug.Log("Update Panels. Total: " + panels.Count);

        
        OnPanelsChanged?.Invoke(panels);
    }

  
    public async void CreatePanel(string type, string title)
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

    
    public async void UpdatePanelContent(string panelId, string newContent)
    {
        string path = $"rooms/{roomId}/panels/{panelId}";

        await firestore.SetDocument(path, new
        {
            content = newContent
        });

        Debug.Log("Update Panel: " + panelId);
    }

    public async void DeletePanel(string panelId)
    {
        string path = $"rooms/{roomId}/panels/{panelId}";

        await FirebaseFirestore.DefaultInstance.Document(path).DeleteAsync();

        Debug.Log("Panel remove: " + panelId);
    }
}
