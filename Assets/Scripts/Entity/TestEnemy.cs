using Entity.Base;
using Manager;

namespace Entity
{
    public class TestEnemy : EntityBase
    {
        public override void Die()
        {
            base.Die();
            if (Health > 0) return;
            PoolManager.Release("EnemyPool", this);
            print("Die");
        }
    }
}