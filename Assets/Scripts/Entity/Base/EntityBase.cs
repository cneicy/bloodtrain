using System;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    public abstract class EntityBase : MonoBehaviour, IEntity
    {
        public int Health { get; set; }
        public float Speed { get; set; }
        private Vector3 _tracePosition;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            //EntityManager.Instance.Entities.Add(this);
            EventManager.Instance.TriggerEvent("EntitySpawn", this);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterAllEventsForObject(this);
            //EntityManager.Instance.Entities.Remove(this);
            EventManager.Instance.TriggerEvent("EntityDie", this);
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
            Die();
        }

        public virtual void Die()
        {
            //这玩意应该扔具体实现里
            /*if (Health <= 0)
                PoolManager.Release("EnemyPool",this);*/
        }

        public void LookAt(Transform target)
        {
            transform.LookAt(target);
        }

        public void GetHurt()
        {
            throw new NotImplementedException();
        }

        public void Attack(int damage)
        {
            throw new NotImplementedException();
        }
    }
}