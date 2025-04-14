using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Manager
{
    public class CameraManager : MonoBehaviour
    {
        private Vector3 _posVelocity; //相机位移速度 SmoothDamp方法使用
        private Vector3 _rotVelocity; //相机旋转速度 SmoothDamp方法使用
        [SerializeField] private float limitFov;
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private CinemachineVirtualCamera firstPerson;
        [SerializeField] private float mouseSensitivity = 300f;
        [SerializeField] private CinemachinePOV cinemachinePov;
        private bool _isFirstPerson;
        private bool _canOp = true; //相机过渡冷却用
        private float _xRotation;
        private (float, float) _rotateSpeed; //Freelook拖动旋转用
        private Camera _camera;

        private void OnEnable()
        {
            //处理[EventSubscribe()]特性标注的事件订阅
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            //事件取消订阅
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        private IEnumerator CoolDown()
        {
            _canOp = false;
            yield return new WaitForSeconds(1);
            _canOp = true;
        }

        private void Start()
        {
            _camera = Camera.main;
            _rotateSpeed = (freeLook.m_XAxis.m_MaxSpeed, freeLook.m_YAxis.m_MaxSpeed);
            freeLook.m_XAxis.m_MaxSpeed = 0;
            freeLook.m_YAxis.m_MaxSpeed = 0;
            cinemachinePov = FindObjectOfType<CinemachinePOV>().GetComponent<CinemachinePOV>();
            cinemachinePov.m_HorizontalAxis.m_MaxSpeed = mouseSensitivity;
            cinemachinePov.m_VerticalAxis.m_MaxSpeed = mouseSensitivity;
        }

        private void Update()
        {
            FreeLookToFirstPerson();
            if (!_canOp) return;
            DragToFreeLook();
            FovScale();
        }

        private void FovScale()
        {
            switch (Input.mouseScrollDelta.y)
            {
                case > 0 when freeLook.m_Lens.FieldOfView > 10:
                case < 0 when freeLook.m_Lens.FieldOfView < limitFov:
                    freeLook.m_Lens.FieldOfView -= Input.mouseScrollDelta.y;
                    break;
            }

            switch (Input.mouseScrollDelta.y)
            {
                case > 0 when firstPerson.m_Lens.FieldOfView > 10:
                case < 0 when firstPerson.m_Lens.FieldOfView < limitFov:
                    firstPerson.m_Lens.FieldOfView -= Input.mouseScrollDelta.y;
                    break;
            }
        }

        private void DragToFreeLook()
        {
            if (!Input.GetMouseButton(0))
            {
                freeLook.m_XAxis.m_MaxSpeed = 0;
                freeLook.m_YAxis.m_MaxSpeed = 0;
            }
            else
            {
                freeLook.m_XAxis.m_MaxSpeed = _rotateSpeed.Item1;
                freeLook.m_YAxis.m_MaxSpeed = _rotateSpeed.Item2;
            }
        }

        private void FreeLookToFirstPerson()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            StartCoroutine(CoolDown());
            _isFirstPerson = !_isFirstPerson;
            var temp = _camera.transform.eulerAngles;
            freeLook.Priority = _isFirstPerson ? 0 : 10;
            firstPerson.Priority = _isFirstPerson ? 10 : 0;
            if (_isFirstPerson)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cinemachinePov.m_HorizontalAxis.Value = temp.y;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}