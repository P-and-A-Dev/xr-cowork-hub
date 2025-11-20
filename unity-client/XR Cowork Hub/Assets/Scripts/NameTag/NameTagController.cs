using UnityEngine;
using TMPro;

public class NameTagController : MonoBehaviour
{
    [Header("References")]
    public TMP_Text userNameText;
    public TMP_Text userStateText;

    // User namee defien
    public void SetName(string name)
    {
        if (userNameText != null)
            userNameText.text = name;
    }

    // what the user is doing
    public void SetState(string state)
    {
        if (userStateText != null)
            userStateText.text = state;
    }
}
