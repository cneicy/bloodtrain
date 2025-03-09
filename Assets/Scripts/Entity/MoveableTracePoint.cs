using System;
using Manager;
using UnityEngine;
using Utils.KeyboardInput;

namespace Entity
{
    public class MoveableTracePoint : MonoBehaviour
    {
        [SerializeField] private Vector3 firstPosition;
        [SerializeField] private Vector3 secondPosition;
        [SerializeField] private Vector3 thirdPosition;
        private Vector2 _direction;
        public Vector3 NowPosition { get; set; }

        private void Update()
        {
            _direction = KeySettingManager.Instance.Direction;
            
        }

        private void ChangePosition()
        {
            EventManager.Instance.TriggerEvent("TracePositionChange", NowPosition);
        }
    }
}