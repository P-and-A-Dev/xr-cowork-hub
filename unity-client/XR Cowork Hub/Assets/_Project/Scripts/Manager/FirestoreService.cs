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
            Debug.Log("[FirestoreService] Waiting for FirebaseInit...");

            float timeout = 10f;
            float timer = 0f;

            while (!FirebaseInit.isInitialized && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (!FirebaseInit.isInitialized)
            {
                Debug.LogError("[FirestoreService] FirebaseInit never initialized (timeout 10s).");
                yield break;
            }

            Debug.Log("[FirestoreService] FirebaseInit.IsInitialized == true, trying Firestore...");

            try
            {
                _db = FirebaseFirestore.DefaultInstance;
                IsInitialized = true;
                Debug.Log("[FirestoreService] Firestore initialized.");
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