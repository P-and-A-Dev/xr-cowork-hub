using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class FirebaseInit : MonoBehaviour
    {
        public static bool IsInitialized { get; private set; } = false;
        public static FirebaseApp App;

        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    App = FirebaseApp.DefaultInstance;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}