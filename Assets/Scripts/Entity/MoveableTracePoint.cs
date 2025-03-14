using Manager;
using UnityEngine;

namespace Entity
{
    public class MoveableTracePoint : MonoBehaviour
    {
        [SerializeField] private Vector3 leftPosition;
        [SerializeField] private Vector3 midPosition;
        [SerializeField] private Vector3 rightPosition;

        public Vector3 NowPosition { get; set; }

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
        }

        private void ChangePosition()
        {
            EventManager.Instance.TriggerEvent("TracePositionChange", NowPosition);
        }
    }
}