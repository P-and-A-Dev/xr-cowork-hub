using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreTest : MonoBehaviour
{
    public FirestoreService firestore;

    async void Start()
    {
        Debug.Log("Firestore test started.");

        
        //test document creation
        await firestore.SetDocument(
            "test/testing123",
            new {
                message = "Hello Firestore!",
                timestamp = System.DateTime.UtcNow
            }
        );

        Debug.Log("Document successfully created.");

       
        // test collection listener
        firestore.ListenCollection<TestData>("test", OnCollectionChanged);

        Debug.Log("Listening to collection 'test'...");
    }

    void OnCollectionChanged(List<TestData> docs)
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
