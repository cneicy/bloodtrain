using System.Collections;
using Entity.Base;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity
{
    public class Train : MonoBehaviour, IMusou
    {
        [SerializeField] public GameObject[] cannonSlot = new GameObject[3];
        [SerializeField] public GameObject trainHead;
        [SerializeField] public GameObject trainBody;
        [SerializeField] public GameObject trainEnd;
        private bool _isLowFuel;
        [SerializeField] public float musouFuelNeed;
        [Header("火车属性")] public float fuel;

        public float fuelCostPreTime;


        private void FixedUpdate()
        {
            if (fuel > 0 || _isLowFuel) return;
            _isLowFuel = true;
            StartCoroutine(LowFuel());
        }

        private IEnumerator CostFuel()
        {
            yield return new WaitForSeconds(0.5f);
            if (fuel > 0)
                fuel -= fuelCostPreTime;
            StartCoroutine(CostFuel());
        }

        private IEnumerator LowFuel()
        {
            while (_isLowFuel)
            {
                print("Low fuel:"+fuel);
                yield return new WaitForSeconds(0.5f);
                EventManager.Instance.TriggerEvent("TrainLowFuel", this);
                if (fuel > 0)
                {
                    _isLowFuel = false;
                    EventManager.Instance.TriggerEvent("TrainEnoughFuel", this);
                }
            }
        }

        [EventSubscribe("GameStart")]
        public object OnGameStart(Object obj)
        {
            StartCoroutine(CostFuel());
            return null;
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
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

        public void Musou()
        {
            if (fuel < musouFuelNeed) return;
            fuel -= musouFuelNeed;
            EventManager.Instance.TriggerEvent("TrainMusou", this);
        }
    }
}