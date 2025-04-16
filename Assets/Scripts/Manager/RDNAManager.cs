using System.Collections.Generic;
using System.Linq;
using Entity.Base;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class RDNAManager : Singleton<RDNAManager>
    {
        [FormerlySerializedAs("RDNA")] [SerializeField]
        public List<GameObject> rdna = new();

        public bool isRDNALevelUpPhrase = true;
        [SerializeField] private GameObject nextRdna;

        [SerializeField] public SerializableDictionary<string, int> total = new();

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            if (rdna == null) return;
            nextRdna = AccessAvailableRDNA();
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
            if (!isRDNALevelUpPhrase) return null;
            if (rdna == null) return this;
            if (hitPoint.y < 2.2f) hitPoint.y = 2.2f;
            var temp = AccessAvailableRDNA();

            Instantiate(temp, hitPoint, Quaternion.identity).GetComponent<Rigidbody2D>().simulated = true;
            rdna.Remove(temp);
            nextRdna = AccessAvailableRDNA();
            return this;
        }

        private GameObject AccessAvailableRDNA()
        {
            return rdna.FirstOrDefault(item =>
                item.GetComponent<RDNABase>().phrase <= GamePhraseManager.Instance.currentPhrase);
        }
    }
}