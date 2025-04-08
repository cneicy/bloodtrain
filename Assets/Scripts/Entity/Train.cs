using System.Collections;
using Entity.Base;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// 列车
    /// </summary>
    public class Train : MonoBehaviour, IMusou
    {
        [SerializeField] public GameObject[] cannonSlot = new GameObject[3];//todo:将槽位更改为网格
        [SerializeField] public GameObject trainHead;//火车头
        [SerializeField] public GameObject trainBody;//火车身
        [SerializeField] public GameObject trainEnd;//火车尾
        private bool _isLowFuel;//当前是否燃料不足
        [SerializeField] public float musouFuelNeed;//触发超频所需燃料
        [Header("火车属性")] public float fuel;//燃料

        public float fuelCostPreTime;//每0.5s消耗燃料数


        private void FixedUpdate()
        {
            if (fuel > 0 || _isLowFuel) return;
            _isLowFuel = true;
            StartCoroutine(LowFuel());
        }

        /// <summary>
        /// 消耗燃料，每0.5秒消耗一次
        /// </summary>
        private IEnumerator CostFuel()
        {
            yield return new WaitForSeconds(0.5f);
            if (fuel > 0)
                fuel -= fuelCostPreTime;
            StartCoroutine(CostFuel());
        }

        /// <summary>
        /// 燃料不足
        /// 当<c>FixedUpdate</c>检测到燃料不足(<c>_isLowFuel = true</c>)时
        /// 每0.5s触发一次燃料不足事件
        /// 当燃料大于0时将<c>_isLowFuel</c>设为<c>false</c>，并触发燃料充足事件
        /// </summary>
        private IEnumerator LowFuel()
        {
            while (_isLowFuel)
            {
                yield return new WaitForSeconds(0.5f);
                EventManager.Instance.TriggerEvent("TrainLowFuel", this);
                if (fuel > 0)
                {
                    _isLowFuel = false;
                    EventManager.Instance.TriggerEvent("TrainEnoughFuel", this);
                }
            }
        }

        /// <summary>
        /// 当游戏开始事件触发时启动消耗燃料协程
        /// </summary>
        [EventSubscribe("GameStart")]
        public object OnGameStart(Object obj)
        {
            StartCoroutine(CostFuel());
            return null;
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);//处理[EventSubscribe()]特性标注的事件订阅
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);//事件取消订阅
        }

        /// <summary>
        /// 当冲撞到敌人时执行列车减速和敌人被撞事件
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<TestEnemy>(out var entityBase)) return;
            EventManager.Instance.TriggerEvent("TrainSlowDown", this);
            EventManager.Instance.TriggerEvent("EntityCrash", entityBase);
        }

        /// <summary>
        /// 当敌人死亡后燃料+10
        /// </summary>
        [EventSubscribe("EntityDie")]
        public object OnEntityDie(EntityBase entity)
        {
            fuel += 10;
            return this;
        }

        /// <summary>
        /// 超频 当燃料足够<c>musouFuelNeed</c>后消耗
        /// 并触发列车超频事件
        /// </summary>
        public void Musou()
        {
            if (fuel < musouFuelNeed) return;
            fuel -= musouFuelNeed;
            EventManager.Instance.TriggerEvent("TrainMusou", this);
        }
    }
}