using System;
using UnityEngine;

namespace Build
{
    public class BuildInput : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;
        private Vector3 _lastPosition;
        [SerializeField] private LayerMask placementLayerMask;
        public event Action OnClick, OnExit;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnExit?.Invoke();
            }
        }

        public Vector3 GetSelectedMapPosition()
        {
            Vector3 mousePos= Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
            {
                _lastPosition = hit.point;
            }
            return _lastPosition;
        }
    }
}
