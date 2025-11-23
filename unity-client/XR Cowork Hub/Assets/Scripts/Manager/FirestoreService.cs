using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreService : MonoBehaviour
{
    private FirebaseFirestore db;

    void Awake()
    {
        // Initialize the Firestore
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Firestore initialized.");
    }

    public Task SetDocument(string path, object data)
    {
        // Reference a Firestore document from the given path
        DocumentReference docRef = db.Document(path);

        return docRef.SetAsync(data, SetOptions.MergeAll);
    }

      public void ListenCollection<T>(string path, Action<List<T>> onChanged)
    {
        
        CollectionReference colRef = db.Collection(path);

    
        colRef.Listen(snapshot =>
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
    }


    public void ListenDocument<T>(string path, Action<T> onChanged)
    {
        DocumentReference docRef = db.Document(path);

      
        docRef.Listen(snapshot =>
        {
            
            if (!snapshot.Exists) return;        
            T obj = snapshot.ConvertTo<T>();

        
            onChanged?.Invoke(obj);
        });

        Debug.Log("[FirestoreService] Listening to document: " + path);
    }

        //4. UPTADEVOICEGOUPID
    public async Task UpdateVoiceGroup(string roomId, List<string> userIds, long newGroupId)
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

        // show the new uptades 
        await batch.CommitAsync();

        Debug.Log($"[FirestoreService] VoiceGroup updated to {newGroupId} (users: {userIds.Count})");
    }
}
