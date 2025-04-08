namespace Entity.Interface
{
    //可以攻击 一般可用于敌人或者陷阱等对象
    public interface IAttackable
    {
        void Attack(int damage);
    }
}