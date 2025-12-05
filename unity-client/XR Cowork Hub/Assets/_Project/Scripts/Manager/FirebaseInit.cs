using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
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
                    Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
#if UNITY_EDITOR
            // Skip Firebase in Editor to avoid crashes
            Debug.LogWarning("[FirebaseInit] Firebase DISABLED in Editor. Will only work on device.");
            IsInitialized = true; // Fake initialization for Editor
            return;
#endif

            Debug.Log("[FirebaseInit] Starting Firebase initialization...");

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError("Firebase dependencies missing: " + task.Result);
                    return;
                }

                Debug.Log("Firebase Initialized");

                var auth = FirebaseAuth.DefaultInstance;

                auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsFaulted)
                    {
                        Debug.LogError("Auth Error: " + authTask.Exception);
                        return;
                    }

                    FirebaseUser user = auth.CurrentUser;

                    if (user != null)
                    {
                        Debug.Log("✔ Login Anonymous OK");
                        Debug.Log("UID: " + user.UserId);
                        IsInitialized = true;
                    }
                    else
                    {
                        Debug.LogError("User NULL after Auth");
                    }
                });
            });
        }
    }
}