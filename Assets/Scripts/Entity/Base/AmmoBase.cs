using Entity.Interface;
using UnityEngine;

namespace Entity.Base
{
    public enum AmmoType
    {
        Normal = 0,
        Fire = 1,
    }
    public class AmmoBase : MonoBehaviour
    {
        [SerializeField] public float damage;
        [SerializeField] public AmmoType ammoType;
        [SerializeField] public float flySpeed;
        [SerializeField] public Vector3 direction;
        public void OnUpdate()
        {
            transform.Translate(direction * (flySpeed * Time.deltaTime), Space.World);
        }
    }
}