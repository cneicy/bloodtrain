using Entity.Interface;
using UnityEngine;

namespace Entity.Base
{
    /// <summary>
    ///炮塔抽象类
    /// </summary>
    public abstract class CannonBase : MonoBehaviour, IShootable
    {
        public int ammoCount;

        //公有方法 射击
        public void Shoot()
        {
        }
    }
}