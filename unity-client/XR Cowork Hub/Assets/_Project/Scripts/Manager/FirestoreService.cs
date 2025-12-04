using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class FirestoreService : MonoBehaviour
    {
        private FirebaseFirestore _db;

        private void Awake()
        {
            _db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firestore initialized.");
        }

        private static void LogFirestoreError(string method, Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogError($"[FirestoreService] Error in {method}: {ex.Message}");
#endif
        }

        public Task SetDocument(string path, object data)
        {
            try
            {
                DocumentReference docRef = _db.Document(path);
                return docRef.SetAsync(data, SetOptions.MergeAll);
            }
            catch (Exception ex)
            {
                LogFirestoreError("SetDocument", ex);
                return Task.CompletedTask;
            }
        }

        public void ListenCollection<T>(string path, Action<List<T>> onChanged)
        {
            try
            {
                CollectionReference colRef = _db.Collection(path);

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
            catch (Exception ex)
            {
                LogFirestoreError("ListenCollection", ex);
            }
        }

        public void ListenDocument<T>(string path, Action<T> onChanged)
        {
            try
            {
                DocumentReference docRef = _db.Document(path);

                docRef.Listen(snapshot =>
                {
                    if (!snapshot.Exists) return;
                    T obj = snapshot.ConvertTo<T>();
                    onChanged?.Invoke(obj);
                });

                Debug.Log("[FirestoreService] Listening to document: " + path);
            }
            catch (Exception ex)
            {
                LogFirestoreError("ListenDocument", ex);
            }
        }

        public async Task UpdateVoiceGroup(string roomId, List<string> userIds, long newGroupId)
        {
            try
            {
                WriteBatch batch = _db.StartBatch();

                foreach (var userId in userIds)
                {
                    DocumentReference docRef = _db.Document($"rooms/{roomId}/participants/{userId}");
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
}