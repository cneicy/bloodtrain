namespace Entity.Interface
{
    /// <summary>
    /// 可受到攻击
    /// </summary>
    public interface IHurtable
    {
        int GetHurt(int damage);
    }
}