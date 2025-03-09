using Entity.Interface;
using UnityEngine;

namespace Entity.Base
{
    public abstract class CannonBase : MonoBehaviour, IShootable
    {
        [SerializeField] private int ammoCount;

        public void Shoot()
        {
        }
    }
}