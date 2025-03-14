using System;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    public abstract class EntityBase : MonoBehaviour, IEntity
    {
        private Vector3 _tracePosition;
        [SerializeField] private int health;
        [SerializeField] private float speed;

        public int Health
        {
            get => health;
            set => health = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out SpeedArea speedArea))
            {
            }
        }

        protected virtual void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            EventManager.Instance.TriggerEvent("EntitySpawn", this);
        }

        protected virtual void OnDisable()
        {
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
            _tracePosition = position;
            return this;
        }

        // todo:此处参数应该为IAttack之类的可获取伤害的对象
        public int GetHurt(int damage)
        {
            Health -= damage;
            return Health;
        }

        public void OnUpdate(Transform cameraTransform)
        {
            LookAt(cameraTransform);
            transform.position = Vector3.MoveTowards(transform.position, _tracePosition, Speed * Time.deltaTime);
            Debug.Log(_tracePosition);
            Die();
        }

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }
    }
}