using Entity.Base;

namespace Entity
{
    //dddd
    public class TestEnemy : EntityBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Health = 100;
            Speed = 0.5f;
        }
    }
}