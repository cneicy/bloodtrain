using Entity.Base;
using Manager;
using UnityEngine;

namespace Entity
{
    public class TestEnemy : EntityBase
    {
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Health = 100;
            Speed = 0.5f;
        }

        public override void OnUpdate(Transform cameraTransform)
        {
            base.OnUpdate(cameraTransform);
            _spriteRenderer.flipX = !(transform.position.x - TracePosition.x > 0);
        }

        public override void Die()
        {
            base.Die();
            if (Health > 0) return;
            PoolManager.Release("Enemy", this);
            print("Die");
        }
    }
}