using System;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    public abstract class EntityBase : MonoBehaviour, IEntity
    {
        protected Vector3 TracePosition;
        [SerializeField] private int health;
        [SerializeField] private float speed;
        private float _speed;

        public int Health
        {
            get => health;
            set => health = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }


        protected virtual void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            EventManager.Instance.TriggerEvent("EntitySpawn", this);
        }

        protected virtual void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
            EventManager.Instance.TriggerEvent("EntityDie", this);
        }

        public virtual void Die()
        {
            //这玩意应该扔具体实现里
            /*if (Health <= 0)
                PoolManager.Release("EnemyPool",this);*/
        }

        public void GetHurt()
        {
            throw new NotImplementedException();
        }

        public void Attack(int damage)
        {
            throw new NotImplementedException();
        }

        [EventSubscribe("TracePositionChange")]
        public object TracePositionChange(Vector3 position)
        {
            TracePosition = position;
            return this;
        }

        // todo:此处参数应该为IAttack之类的可获取伤害的对象
        public int GetHurt(int damage)
        {
            Health -= damage;
            return Health;
        }

        public virtual void OnUpdate(Transform cameraTransform)
        {
            LookAt(cameraTransform);
            transform.position = transform.position.x - TracePosition.x > 0
                ? Vector3.MoveTowards(transform.position, TracePosition, Speed * 1.5f * Time.deltaTime)
                : Vector3.MoveTowards(transform.position, TracePosition, Speed * 0.5f * Time.deltaTime);
            Die();
        }

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }
    }
}