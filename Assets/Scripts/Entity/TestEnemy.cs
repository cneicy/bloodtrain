using Entity.Base;
using Manager;

namespace Entity
{
    public class TestEnemy : EntityBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Health = 100;
            Speed = 0.5f;
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