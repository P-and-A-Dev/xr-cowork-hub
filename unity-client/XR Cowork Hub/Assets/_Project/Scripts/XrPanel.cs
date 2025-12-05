using UnityEngine;

namespace _Project.Scripts
{
    public class XrPanel : MonoBehaviour
    {
        private static readonly int CornerRadius = Shader.PropertyToID("_CornerRadius");
        private Material _mat;

        [Range(0f, 1f)] public float cornerRadius = 0.15f;

        private void Start()
        {
            _mat = GetComponent<Renderer>().material;

            if (_mat.HasProperty(CornerRadius))
            {
                _mat.SetFloat(CornerRadius, cornerRadius);
            }
        }
    }
}