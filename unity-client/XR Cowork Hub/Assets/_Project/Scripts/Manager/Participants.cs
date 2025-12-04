using Firebase.Firestore;

[FirestoreData]
public class Participant
{
    [FirestoreProperty] public string userId { get; set; }
    [FirestoreProperty] public string displayName { get; set; }
    [FirestoreProperty] public long voiceGroupId { get; set; }
    [FirestoreProperty] public bool isOnline { get; set; }
    [FirestoreProperty] public bool inBubbleSpace { get; set; }
    [FirestoreProperty] public Timestamp lastSeen { get; set; }
}
