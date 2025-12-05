using Firebase.Firestore;

namespace _Project.Scripts.Manager
{
    [FirestoreData]
    public class PanelData
    {
        [FirestoreProperty] public string panelId { get; set; }
        [FirestoreProperty] public string type { get; set; }
        [FirestoreProperty] public string title { get; set; }
        [FirestoreProperty] public string content { get; set; }
        [FirestoreProperty] public string visibility { get; set; }
        [FirestoreProperty] public long bubbleGroupId { get; set; }
    }
}