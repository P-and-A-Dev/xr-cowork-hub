using UnityEngine;

namespace _Project.Scripts.Common
{
    public class QuadToRectTransform : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Transform quadTransform;

        void Update()
        {
            float w = rectTransform.rect.width;
            float h = rectTransform.rect.height;

            float scaleFactor = rectTransform.lossyScale.x;

            quadTransform.localScale = new Vector3(
                w * scaleFactor,
                h * scaleFactor,
                quadTransform.localScale.z
            );
        }
    }
}