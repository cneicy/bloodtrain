namespace Entity.Interface
{
    public interface IEnemy
    {
        void Attack();
        void Die();
        int GetHurt(int damage);
    }
}