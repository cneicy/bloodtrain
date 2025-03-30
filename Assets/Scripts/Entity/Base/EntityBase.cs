using System;
using System.Collections;
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
        [SerializeField] private float limitSpeed;
        private Rigidbody _rigidbody;
        private float _speed;
        private SpriteRenderer _spriteRenderer;
        private ConstantForce _force;
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
            _rigidbody = GetComponent<Rigidbody>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _force =  GetComponent<ConstantForce>();
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            EventManager.Instance.TriggerEvent("EntitySpawn", this);
        }

        protected virtual void OnDisable()
        {
            //修复回收后保留速度
            _rigidbody.velocity = Vector3.zero;
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        public virtual void Die()
        {
            if (Health <= 0)
            {
                EventManager.Instance.TriggerEvent("EntityDie", this);
                PoolManager.Release("Enemy", this);
            }
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

        [EventSubscribe("EntityCrash")]
        public object OnEntityCrash(EntityBase entity)
        {
            if (entity == this)
            {
                Health=0;
            }
            return null;
        }

        // todo:此处参数应该为IAttack之类的可获取伤害的对象
        public int GetHurt(int damage)
        {
            Health -= damage;
            return Health;
        }

        // 因平衡性考虑，不为不同批次敌人同步自身所受常力
        [EventSubscribe("TrainSlowDown")]
        public object OnTrainSlowDown(Train sender)
        {
            StartCoroutine(Slow());
            return this;
        }

        private IEnumerator Slow()
        {
            limitSpeed /= 0.9f;
            _force.force *= 0.9f;
            yield return new WaitForSeconds(1);
            _force.force /= 0.9f;
            limitSpeed *= 0.9f;
        }

        public virtual void OnUpdate(Transform cameraTransform)
        {
            LookAt(cameraTransform);
            Run();
            Die();
        }

        private void Run()
        {
            _rigidbody.AddForce((TracePosition - transform.position).normalized * (Speed * Time.deltaTime),ForceMode.VelocityChange);
            var temp =_rigidbody.velocity;
            temp.x = Mathf.Clamp(temp.x, -114514, limitSpeed);
            temp.z = Mathf.Clamp(temp.z, -limitSpeed, limitSpeed);
            _rigidbody.velocity = temp;
        }

        public void LookAt(Transform target)
        {
            _spriteRenderer.flipX = !(transform.position.x - TracePosition.x > 0);
            transform.LookAt(target);
        }
    }
}