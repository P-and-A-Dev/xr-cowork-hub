using UnityEngine;
using System.Collections.Generic;
using System;
using Firebase.Firestore;

/// <summary>
/// Manages all operations related to panels: creating, updating,
/// deleting and monitoring changes via Firestore listeners.
/// </summary>
public class PanelManager : MonoBehaviour
{
    public FirestoreService firestore;
    [SerializeField] private string roomId = "main-room";

    public Action<List<PanelData>> OnPanelsChanged;

    private ListenerRegistration panelsListener;

    /// <summary>
    /// Starts listening to Firestore for panel changes.
    /// </summary>
    void Start()
    {
        ListenToPanels();
    }

    /// <summary>
    /// Subscribes to Firestore panel changes in the room.
    /// </summary>
    void ListenToPanels()
    {
        panelsListener = firestore.ListenCollection<PanelData>(
            $"rooms/{roomId}/panels",
            OnPanelsCollectionChanged
        );

        Debug.Log("PanelManager: Listening panels");
    }

    /// <summary>
    /// Callback triggered when panel data is updated in Firestore.
    /// </summary>
    void OnPanelsCollectionChanged(List<PanelData> panels)
    {
        Debug.Log("Update Panels. Total: " + panels.Count);
        OnPanelsChanged?.Invoke(panels);
    }

    /// <summary>
    /// Creates a new panel and writes it to Firestore.
    /// </summary>
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

    /// <summary>
    /// Updates a panel's text content.
    /// </summary>
    public async void UpdatePanelContent(string panelId, string newContent)
    {
        string path = $"rooms/{roomId}/panels/{panelId}";

        await firestore.SetDocument(path, new
        {
            content = newContent
        });

        Debug.Log("Update Panel: " + panelId);
    }

    /// <summary>
    /// Removes a panel from Firestore.
    /// </summary>
    public async void DeletePanel(string panelId)
    {
        string path = $"rooms/{roomId}/panels/{panelId}";
        await FirebaseFirestore.DefaultInstance.Document(path).DeleteAsync();
        Debug.Log("Panel remove: " + panelId);
    }

    void OnDestroy()
    {
        panelsListener?.Stop();
    }
}
