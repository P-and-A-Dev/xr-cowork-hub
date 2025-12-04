using UnityEngine;
using System.Collections.Generic;

public class FocusBubbleTest : MonoBehaviour
{
    public FirestoreService firestore;

    async void Start()
    {
        Debug.Log("Focus Bubble test started.");

        List<string> ids = new List<string>
        {
            "user-1",
            "user-2"
        };

        long groupId = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await firestore.UpdateVoiceGroup("test-room", ids, groupId);

        Debug.Log("Focus Bubble applied to Firestore.");
    }
}
