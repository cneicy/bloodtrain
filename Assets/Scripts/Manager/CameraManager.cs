using System.Collections;
using UnityEngine;

namespace Manager
{
    public enum ViewPoint //相机视角枚举类型
    {
        Side, //侧视角
        TopDown //俯视角
    }

    public class CameraManager : MonoBehaviour
    {
        public ViewPoint viewPoint;
        private readonly Vector3 _sidePosition = new(0, 1.2f, -10);//侧视角相机位置
        private readonly Vector3 _sideRotation = new(6.2f, 0, 0);//侧视角相机欧拉角
        private readonly Vector3 _topDownPosition = new(-4.96f, 6.47f, -10f);//俯视角相机位置
        private readonly Vector3 _topDownRotation = new(29.35f, 24.71f, 0);//俯视角相机欧拉角
        private Camera _camera;
        public float smoothTime = 0.5f;//平滑时间
        private Vector3 _posVelocity;//相机位移速度 SmoothDamp方法使用
        private Vector3 _rotVelocity;//相机旋转速度 SmoothDamp方法使用

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

        private void Start()
        {
            _camera = GetComponent<Camera>();
            viewPoint = ViewPoint.TopDown;
        }

        private void Update()
        {
            //相机位移方法
            if (viewPoint == ViewPoint.TopDown)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _topDownPosition, ref _posVelocity, smoothTime);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _topDownRotation, ref _rotVelocity, smoothTime);
            }
            else
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _sidePosition, ref _posVelocity, smoothTime);
                transform.eulerAngles =
                    Vector3.SmoothDamp(transform.eulerAngles, _sideRotation, ref _rotVelocity, smoothTime);
            }
        }
        
        /// <summary>
        /// 游戏开始相机调度
        /// 游戏开始的相机调度延迟1s
        /// </summary>
        private IEnumerator CoolSwitch()
        {
            yield return new WaitForSeconds(1f);
            smoothTime = 1.2f;
            viewPoint = viewPoint == ViewPoint.Side ? ViewPoint.TopDown : ViewPoint.Side;
            yield return new WaitForSeconds(1.2f);
            smoothTime = 0.5f;
        }

        //同上
        [EventSubscribe("GameStart")]
        public object SwitchViewPoint(Object obj)
        {
            StartCoroutine(CoolSwitch());
            return null;
        }

        /// <summary>
        /// 常规相机调度
        /// 无延时
        /// todo:为此方法绑定一个键位设置
        /// </summary>
        public void SwitchViewPoint()
        {
            viewPoint = viewPoint == ViewPoint.Side ? ViewPoint.TopDown : ViewPoint.Side;
        }
    }
}