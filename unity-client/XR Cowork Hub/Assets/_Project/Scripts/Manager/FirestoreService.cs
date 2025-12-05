using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class FirestoreService : MonoBehaviour
    {
        private FirebaseFirestore _db;
        public bool IsInitialized { get; private set; } = false;

        private IEnumerator Start()
        {
#if UNITY_EDITOR
            // Skip Firestore in Editor
            Debug.LogWarning("[FirestoreService] Firestore DISABLED in Editor. Will only work on device.");
            IsInitialized = true; // Fake initialization for Editor
            yield break;
#endif

            Debug.Log("[FirestoreService] Waiting for FirebaseInit...");
            yield return new WaitUntil(() => FirebaseInit.IsInitialized);

            try
            {
                _db = FirebaseFirestore.DefaultInstance;
                IsInitialized = true;
                Debug.Log("Firestore initialized.");
            }
            catch (Exception ex)
            {
                LogFirestoreError("Start", ex);
            }
        }

        private static void LogFirestoreError(string method, Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogError($"[FirestoreService] Error in {method}: {ex.Message}");
#endif
        }

        public Task SetDocument(string path, object data)
        {
#if UNITY_EDITOR
            Debug.Log($"[FirestoreService] EDITOR MODE: Would set document at {path}");
            return Task.CompletedTask;
#endif

            if (!IsInitialized) return Task.CompletedTask;

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
#if UNITY_EDITOR
            Debug.Log($"[FirestoreService] EDITOR MODE: Would listen to collection {path}");
            return;
#endif

            if (!IsInitialized) return;

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
#if UNITY_EDITOR
            Debug.Log($"[FirestoreService] EDITOR MODE: Would listen to document {path}");
            return;
#endif

            if (!IsInitialized) return;

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
#if UNITY_EDITOR
            Debug.Log($"[FirestoreService] EDITOR MODE: Would update voice group to {newGroupId}");
            return;
#endif

            if (!IsInitialized) return;

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