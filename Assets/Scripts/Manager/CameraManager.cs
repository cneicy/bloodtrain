using System.Collections;
using UnityEngine;
//todo:应用此模块
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
        public float smoothTime = 0.5f;
        private Vector3 _posVelocity;
        private Vector3 _rotVelocity;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
            viewPoint = ViewPoint.TopDown;
        }

        private void Update()
        {
            if (viewPoint == ViewPoint.TopDown)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _topDownPosition, ref _posVelocity, smoothTime);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _topDownRotation, ref _rotVelocity, smoothTime);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _sidePosition, ref _posVelocity, smoothTime);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _sideRotation, ref _rotVelocity, smoothTime);
            }
        }

        private IEnumerator CoolSwitch()
        {
            yield return new WaitForSeconds(1f);
            smoothTime = 1.2f;
            viewPoint = viewPoint == ViewPoint.Side ? ViewPoint.TopDown : ViewPoint.Side;
            yield return new WaitForSeconds(1.2f);
            smoothTime = 0.5f;
        }
        [EventSubscribe("GameStart")]
        public object SwitchViewPoint(Object obj)
        {
            StartCoroutine(CoolSwitch());
            return null;
        }
    }
}