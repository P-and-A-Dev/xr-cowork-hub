using System;
using System.Collections.Generic;
using _Project.Scripts.Manager;
using UnityEngine;

namespace _Project.Scripts.Tests
{
    public class FocusBubbleTest : MonoBehaviour
    {
        public FirestoreService firestore;

        private async void Start()
        {
            try
            {
                Debug.Log("Focus Bubble test started.");

                List<string> ids = new List<string>
                {
                    "user-1",
                    "user-2"
                };

                long groupId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                await firestore.UpdateVoiceGroup("test-room", ids, groupId);

                Debug.Log("Focus Bubble applied to Firestore.");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}