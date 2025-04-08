namespace Entity.Interface
{
    //实体行为接口组合 可用于实体基类
    public interface IEntity : IDieable, IHurtable, IAttackable
    {
    }
}