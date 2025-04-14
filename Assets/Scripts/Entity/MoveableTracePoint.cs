using System.Collections;
using Manager;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// 追踪点
    /// 敌人会追踪追踪点的位置进行移动
    /// </summary>
    public class MoveableTracePoint : MonoBehaviour
    {
        [SerializeField] private Vector3 leftPosition; //左追踪点
        [SerializeField] private Vector3 midPosition; //中追踪点
        [SerializeField] private Vector3 rightPosition; //右追踪点
        private Vector3 _posVelocity; //追踪点速度 SmoothDamp方法使用
        public Vector3 NowPosition { get; set; } //追踪点现在的位置

        private void Start()
        {
            StartCoroutine(Loop());
        }

        private void Update()
        {
            //todo:使用最新最热的输入控制器
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (NowPosition == midPosition)
                    NowPosition = leftPosition;
                else if (NowPosition == rightPosition) NowPosition = midPosition;
                ChangePosition();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (NowPosition == leftPosition)
                    NowPosition = midPosition;
                else if (NowPosition == midPosition) NowPosition = rightPosition;
                ChangePosition();
            }

            transform.position = Vector3.SmoothDamp(transform.position, NowPosition, ref _posVelocity, 0.5f);
        }

        /// <summary>
        /// 每1s通知一次场景中的敌人当前追踪点的位置
        /// </summary>
        private IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                ChangePosition();
            }
        }

        /// <summary>
        /// 触发追踪点改变事件
        /// 用于更新场景内敌人的追踪位置
        /// </summary>
        private void ChangePosition()
        {
            EventManager.Instance.TriggerEvent("TracePositionChange", NowPosition);
        }
    }
}