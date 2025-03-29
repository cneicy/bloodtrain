using System.Collections;
using Entity.Base;
using Manager;
using UnityEngine;

namespace Entity
{
    public class Train : MonoBehaviour
    {
        [SerializeField] public GameObject[] cannonSlot = new GameObject[3];
        [SerializeField] public GameObject trainHead;
        [SerializeField] public GameObject trainBody;
        [SerializeField] public GameObject trainEnd;
        private bool _isLowFuel;
        [Header("火车属性")] 
        public float fuel;

        private void FixedUpdate()
        {
            if (!(fuel <= 0) || _isLowFuel) return;
            _isLowFuel = true;
            StartCoroutine(LowFuel());
        }
        
        private IEnumerator LowFuel()
        {
            while (_isLowFuel)
            {
                yield return new WaitForSeconds(0.5f);
                EventManager.Instance.TriggerEvent("TrainSlowDown", this);
                if (fuel > 0)
                {
                    _isLowFuel = false;
                }
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<TestEnemy>(out var entityBase)) return;
            EventManager.Instance.TriggerEvent("TrainSlowDown", this);
            EventManager.Instance.TriggerEvent("EntityCrash", entityBase);
        }
        [EventSubscribe("EntityDie")]
        public object OnEntityDie(EntityBase entity)
        {
            fuel += 10;
            return this;
        }
    }
}