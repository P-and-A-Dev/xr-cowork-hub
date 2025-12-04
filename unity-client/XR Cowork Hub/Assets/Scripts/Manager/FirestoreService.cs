using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Handles all Firestore operations such as setting documents,
/// listening to collections/documents, and updating participant voice groups.
/// Provides error handling and safety validation.
/// </summary>
public class FirestoreService : MonoBehaviour
{
    private FirebaseFirestore db;

    void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Firestore initialized.");
    }

    /// <summary>
    /// Writes or merges data into a Firestore document.
    /// </summary>
    /// <param name="path">Full Firestore path to the document.</param>
    /// <param name="data">Object with Firestore fields.</param>
    public Task SetDocument(string path, object data)
    {
        if (db == null)
        {
            Debug.LogError("[FirestoreService] Firestore is NULL in SetDocument");
            return Task.CompletedTask;
        }

        try
        {
            DocumentReference docRef = db.Document(path);
            return docRef.SetAsync(data, SetOptions.MergeAll);
        }
        catch (Exception ex)
        {
            LogFirestoreError("SetDocument", ex);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Listens for realtime updates in a Firestore collection.
    /// </summary>
    /// <typeparam name="T">Model to convert Firestore documents into.</typeparam>
    /// <param name="path">Collection path.</param>
    /// <param name="onChanged">Callback fired when collection changes.</param>
    public ListenerRegistration ListenCollection<T>(string path, Action<List<T>> onChanged)
    {
        if (db == null)
        {
            Debug.LogError("[FirestoreService] Firestore is NULL in ListenCollection");
            return null;
        }

        try
        {
            CollectionReference colRef = db.Collection(path);

            ListenerRegistration listener = colRef.Listen(snapshot =>
            {
                if (snapshot == null) return;

                List<T> results = new List<T>();
                foreach (var doc in snapshot.Documents)
                {
                    T obj = doc.ConvertTo<T>();
                    results.Add(obj);
                }

                onChanged?.Invoke(results);
            });

            Debug.Log("[FirestoreService] Listening to collection: " + path);
            return listener;
        }
        catch (Exception ex)
        {
            LogFirestoreError("ListenCollection", ex);
            return null;
        }
    }

    /// <summary>
    /// Listens for realtime updates in a Firestore document.
    /// </summary>
    /// <typeparam name="T">Model to convert the document into.</typeparam>
    /// <param name="path">Document path.</param>
    /// <param name="onChanged">Callback fired when the document changes.</param>
    public ListenerRegistration ListenDocument<T>(string path, Action<T> onChanged)
    {
        if (db == null)
        {
            Debug.LogError("[FirestoreService] Firestore is NULL in ListenDocument");
            return null;
        }

        try
        {
            DocumentReference docRef = db.Document(path);

            ListenerRegistration listener = docRef.Listen(snapshot =>
            {
                if (!snapshot.Exists) return;

                T obj = snapshot.ConvertTo<T>();
                onChanged?.Invoke(obj);
            });

            Debug.Log("[FirestoreService] Listening to document: " + path);
            return listener;
        }
        catch (Exception ex)
        {
            LogFirestoreError("ListenDocument", ex);
            return null;
        }
    }

    /// <summary>
    /// Updates the voice group ID of multiple participants in one batch.
    /// </summary>
    /// <param name="roomId">Room identifier.</param>
    /// <param name="userIds">List of participant userIds.</param>
    /// <param name="newGroupId">New voice group value.</param>
    public async Task UpdateVoiceGroup(string roomId, List<string> userIds, long newGroupId)
    {
        if (db == null)
        {
            Debug.LogError("[FirestoreService] Firestore is NULL in UpdateVoiceGroup");
            return;
        }

        try
        {
            WriteBatch batch = db.StartBatch();

            foreach (var userId in userIds)
            {
                DocumentReference docRef = db.Document($"rooms/{roomId}/participants/{userId}");
                batch.Update(docRef, new Dictionary<string, object>
                {
                    { "voiceGroupId", newGroupId }
                });
            }

            await batch.CommitAsync();

            Debug.Log($"[FirestoreService] VoiceGroup updated to {newGroupId} (users: {userIds.Count})");
        }
        catch (Exception ex)
        {
            LogFirestoreError("UpdateVoiceGroup", ex);
        }
    }
}
