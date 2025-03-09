using System;
using System.Collections.Generic;
using Entity.Base;
using UnityEngine;
using Utils;

namespace Manager
{
    public class EntityManager : Singleton<EntityManager>
    {
        public List<EntityBase> Entities { get; set; } = new();

        private void FixedUpdate()
        {
            if (Entities.Count <= 0) return;
            try
            {
                Entities.ForEach(entity => entity.Die());
            }
            catch (Exception)
            {
                Debug.LogWarning("实体列表被修改");
            }
        }
    }
}