using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace _Project.Scripts.Manager
{
    public class FirebaseInit : MonoBehaviour
    {
        public static bool IsInitialized { get; private set; } = false;

        private void Start()
        {
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