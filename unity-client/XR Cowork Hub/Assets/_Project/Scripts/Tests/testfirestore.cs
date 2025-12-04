using System;
using System.Collections.Generic;
using _Project.Scripts.Manager;
using Firebase.Firestore;
using UnityEngine;

namespace _Project.Scripts.Tests
{
    public class FirestoreTest : MonoBehaviour
    {
        public FirestoreService firestore;

        private async void Start()
        {
            try
            {
                Debug.Log("Firestore test started.");

                await firestore.SetDocument(
                    "test/testing123",
                    new
                    {
                        message = "Hello Firestore!",
                        timestamp = DateTime.UtcNow
                    }
                );

                Debug.Log("Document successfully created.");


                firestore.ListenCollection<TestData>("test", OnCollectionChanged);

                Debug.Log("Listening to collection 'test'...");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnCollectionChanged(List<TestData> docs)
        {
            Debug.Log("Change detected. Total documents: " + docs.Count);

            foreach (var d in docs)
            {
                Debug.Log("Document -> message: " + d.message + " | timestamp: " + d.timestamp);
            }
        }
    }

    [FirestoreData]
    public class TestData
    {
        [FirestoreProperty] public string message { get; set; }
        [FirestoreProperty] public Timestamp timestamp { get; set; }
    }
}