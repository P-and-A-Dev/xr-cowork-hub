using UnityEngine;

namespace _Project.Scripts.Avatar
{
    public class Billboard : MonoBehaviour
    {
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
        }

        public void LateUpdate()
        {
            if (!_cam) return;

            transform.LookAt(transform.position + _cam.transform.forward);
        }
    }
}