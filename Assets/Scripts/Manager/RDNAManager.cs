using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class RDNAManager : Singleton<RDNAManager>
    {
        [FormerlySerializedAs("RDNA")]
        [SerializeField]
        public List<GameObject> rdna = new();

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }
        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            //事件取消订阅
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        [EventSubscribe("MouseLeftClick")]
        public object OnMouseLeftClick(Vector3 hitPoint)
        {
            foreach (var item in rdna)
            {
                item.transform.position = hitPoint;
            }
            return this;
        }
    }
}