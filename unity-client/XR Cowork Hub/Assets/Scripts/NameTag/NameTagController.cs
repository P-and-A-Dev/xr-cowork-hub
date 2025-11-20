using TMPro;
using UnityEngine;

namespace NameTag
{
    public class NameTagController : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text userNameText;
        public TMP_Text userStateText;

        // Define the name of the user
        public void SetName(string userName)
        {
            if (userNameText != null)
                userNameText.text = userName;
        }

        // Define the state of the user
        public void SetState(string state)
        {
            if (userStateText != null)
                userStateText.text = state;
        }
    }
}
