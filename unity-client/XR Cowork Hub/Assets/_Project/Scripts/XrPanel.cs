using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class XrPanel : MonoBehaviour
    {
        private static readonly int CornerRadius = Shader.PropertyToID("_CornerRadius");
        private Material mat;

        [Range(0f, 1f)] public float cornerRadius = 0.15f;

        void Start()
        {
            mat = GetComponent<Renderer>().material;

            if (mat.HasProperty(CornerRadius))
            {
                mat.SetFloat(CornerRadius, cornerRadius);
            }
        }
    }
}