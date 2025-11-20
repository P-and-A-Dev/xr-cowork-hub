using UnityEngine;

namespace _Project.Scripts.NameTag
{
    public class Billboard : MonoBehaviour
    {
        private Camera _cam;

        void Start()
        {
            _cam = Camera.main;
        }

        void LateUpdate()
        {
            if (_cam is null) return;

            transform.LookAt(transform.position + _cam.transform.forward);
        }
    }
}
