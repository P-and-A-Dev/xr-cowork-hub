using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class FirebaseInit : MonoBehaviour
    {
        public static bool isInitialized { get; private set; } = false;
        public static FirebaseApp App;

        private void Start()
        {
            Debug.Log("[FirebaseInit] Checking Firebase dependencies...");

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("[FirebaseInit] Dependency check faulted: " + task.Exception);
                    return;
                }

                if (task.IsCanceled)
                {
                    Debug.LogError("[FirebaseInit] Dependency check was canceled.");
                    return;
                }

                var dependencyStatus = task.Result;
                Debug.Log("[FirebaseInit] DependencyStatus = " + dependencyStatus);

                if (dependencyStatus == DependencyStatus.Available)
                {
                    try
                    {
                        App = FirebaseApp.DefaultInstance;
                        isInitialized = true;
                        Debug.Log("[FirebaseInit] Firebase initialized, IsInitialized = true");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("[FirebaseInit] Error while creating FirebaseApp: " + ex);
                    }
                }
                else
                {
                    Debug.LogError($"[FirebaseInit] Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}