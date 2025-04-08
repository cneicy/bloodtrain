using System;
using System.Collections;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    /// <summary>
    ///敌人实体抽象类，继承实体接口，实体接口的继承见实体接口自身定义
    /// </summary>
    public abstract class EntityBase : MonoBehaviour, IEntity
    {
        protected Vector3 TracePosition;
        [SerializeField] private int health;
        [SerializeField] private float speed;
        [SerializeField] private float limitSpeed;
        private Rigidbody _rigidbody;
        private float _speed;
        private SpriteRenderer _spriteRenderer;
        private ConstantForce _force;
        /// <summary>
        /// 实体血量
        /// </summary>
        public int Health
        {
            get => health;
            set => health = value;
        }
        /// <summary>
        /// 实体速度
        /// </summary>
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }


        protected virtual void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _force =  GetComponent<ConstantForce>();
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);//处理[EventSubscribe()]特性标注的事件订阅
            EventManager.Instance.TriggerEvent("EntitySpawn", this);//触发实体生成事件
        }

        protected virtual void OnDisable()
        {
            _rigidbody.velocity = Vector3.zero;//修复回收后保留速度
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);//事件取消订阅
        }

        /// <summary>
        /// 实体死亡
        /// 在血量小于等于0时触发实体死亡事件并调用对象池的Release方法释放自身
        /// </summary>
        public virtual void Die()
        {
            if (Health <= 0)
            {
                EventManager.Instance.TriggerEvent("EntityDie", this);
                PoolManager.Release("Enemy", this);
            }
        }
        


        /// <summary>
        /// 实体攻击
        /// todo:实现实体攻击逻辑并触发实体攻击事件
        /// </summary>
        public void Attack(int damage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 实体追踪点变化
        /// 当追踪点位置改变事件触发时执行相应逻辑
        /// </summary>
        [EventSubscribe("TracePositionChange")]
        public object TracePositionChange(Vector3 position)
        {
            TracePosition = position;
            return this;
        }

        /// <summary>
        /// 实体被撞
        /// 当触发实体被撞事件时执行相应逻辑
        /// </summary>
        [EventSubscribe("EntityCrash")]
        public object OnEntityCrash(EntityBase entity)
        {
            if (entity == this)
            {
                Health=0;
            }
            return null;
        }

        /// <summary>
        /// 实体受击
        /// todo:此处参数应该为IAttack之类的可获取伤害的对象
        /// </summary>
        public int GetHurt(int damage)
        {
            Health -= damage;
            return Health;
        }
        
        /// <summary>
        /// 列车减速
        /// 因平衡性考虑，不为不同批次敌人同步自身所受常力
        /// </summary>
        [EventSubscribe("TrainSlowDown")]
        public object OnTrainSlowDown(Train sender)
        {
            StartCoroutine(Slow());
            return this;
        }

        /// <summary>
        /// 当列车发生减速时提高敌人限速持续1s
        /// 1s后恢复原本限速
        /// </summary>
        private IEnumerator Slow()
        {
            limitSpeed /= 0.9f;
            _force.force *= 0.9f;
            yield return new WaitForSeconds(1);
            _force.force /= 0.9f;
            limitSpeed *= 0.9f;
        }

        /// <summary>
        /// 实体每帧逻辑
        /// 此方法被<c>EntityManager</c>调用
        /// <param name="cameraTransform">相机位置，用于让2D敌人始终面向相机</param>
        /// </summary>
        public virtual void OnUpdate(Transform cameraTransform)
        {
            LookAt(cameraTransform);
            Run();
            Die();
        }

        /// <summary>
        /// 实体奔跑
        /// 使用<c>AddForce</c>对敌人加速
        /// 同时根据<c>limitSpeed</c>对敌人进行限速
        /// </summary>
        private void Run()
        {
            _rigidbody.AddForce((TracePosition - transform.position).normalized * (Speed * Time.deltaTime),ForceMode.VelocityChange);
            var temp =_rigidbody.velocity;
            temp.x = Mathf.Clamp(temp.x, -114514, limitSpeed);
            temp.z = Mathf.Clamp(temp.z, -limitSpeed, limitSpeed);
            _rigidbody.velocity = temp;
        }

        /// <summary>
        /// 实体始终朝向相机
        /// </summary>
        public void LookAt(Transform target)
        {
            _spriteRenderer.flipX = !(transform.position.x - TracePosition.x > 0);
            transform.LookAt(target);
        }
    }
}