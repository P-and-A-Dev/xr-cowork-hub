using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
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
                }
                else
                {
                    Debug.LogError("User NULL after Auth");
                }
            });
        });
    }
}
