using UnityEngine;
using NameTag;

public class UserManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject userPrefab;
    public Transform spawnPoint;

    public static UserManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Todo : remove this after testing
        SpawnTestUser();
    }

    /// <summary>
    /// Spawns a user at a specific position with a name.
    /// </summary>
    public void SpawnUser(string userId, string userName, Vector3 position)
    {
        if (userPrefab == null)
        {
            Debug.LogError("UserManager: User Prefab is not assigned!");
            return;
        }

        GameObject newUser = Instantiate(userPrefab, position, Quaternion.identity);
        newUser.name = "User_" + userId;

        NameTagController nameTag = newUser.GetComponentInChildren<NameTagController>();
        if (nameTag != null)
        {
            nameTag.SetName(userName);
            nameTag.SetState("Online");
        }
        else
        {
            Debug.LogWarning($"UserManager: No NameTagController found on user {userId}");
        }
    }

    /// <summary>
    /// Test method to spawn a dummy user.
    /// Right-click on the component in Inspector to run this.
    /// </summary>
    [ContextMenu("Spawn Test User")]
    public void SpawnTestUser()
    {
        Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        if (spawnPoint != null) randomPos += spawnPoint.position;

        SpawnUser("test_01", "Test User " + Random.Range(0, 100), randomPos);
    }
}
