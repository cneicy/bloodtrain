using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    public class AmmoShooter : MonoBehaviour
    {
        [SerializeField]public AmmoBase ammoPrefabs;
        public List<AmmoBase> Ammo { get; set; } = new();
        public void Start()
        {
            PoolManager.CreatePool(ammoPrefabs.name,ammoPrefabs,20,50);
        }
        private void FixedUpdate()
        {
            if (Ammo.Count <= 0) return;
            try
            {
                Ammo.ForEach(ammo => ammo.OnUpdate());
            }
            catch (Exception)
            {
                Debug.LogWarning("实体列表被修改");
            }
        }
    }
}