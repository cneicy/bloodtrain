using UnityEngine;

namespace Manager
{
    public enum ViewPoint
    {
        Side,
        TopDown
    }

    public class CameraManager : MonoBehaviour
    {
        public ViewPoint viewPoint;
        private readonly Vector3 _sidePosition = new(0, 1.2f, -10);
        private readonly Vector3 _sideRotation = new(6.2f, 0, 0);
        private readonly Vector3 _topDownPosition = new(-4.96f, 6.47f, -10f);
        private readonly Vector3 _topDownRotation = new(29.35f, 24.71f, 0);
        private Camera _camera;
        private Vector3 _posVelocity;
        private Vector3 _rotVelocity;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            viewPoint = ViewPoint.Side;
        }

        private void Update()
        {
            if (viewPoint == ViewPoint.TopDown)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _topDownPosition, ref _posVelocity, 0.5f);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _topDownRotation, ref _rotVelocity, 0.5f);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _sidePosition, ref _posVelocity, 0.5f);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _sideRotation, ref _rotVelocity, 0.5f);
            }
        }

        public void SwitchViewPoint()
        {
            viewPoint = viewPoint == ViewPoint.Side ? ViewPoint.TopDown : ViewPoint.Side;
        }
    }
}