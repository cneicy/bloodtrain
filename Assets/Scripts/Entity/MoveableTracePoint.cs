using Manager;
using UnityEngine;

namespace Entity
{
    public class MoveableTracePoint : MonoBehaviour
    {
        [SerializeField] private Vector3 firstPosition;
        [SerializeField] private Vector3 secondPosition;
        [SerializeField] private Vector3 thirdPosition;

        public Vector3 NowPosition { get; set; }

        private void Update()
        {
            //todo:使用最新最热的输入控制器
            if (Input.GetKeyDown(KeyCode.A)) NowPosition = firstPosition;
        }

        private void ChangePosition()
        {
            EventManager.Instance.TriggerEvent("TracePositionChange", NowPosition);
        }
    }
}