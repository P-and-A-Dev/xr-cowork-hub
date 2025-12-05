using TMPro;
using UnityEngine;

namespace _Project.Scripts.Avatar
{
    public class NameTagController : MonoBehaviour
    {
        [Header("References")] public TMP_Text userNameText;
        public TMP_Text userStateText;

        public void SetName(string userName)
        {
            if (userNameText != null)
                userNameText.text = userName;
        }

        public void SetState(string state)
        {
            if (userStateText != null)
                userStateText.text = state;
        }
    }
}