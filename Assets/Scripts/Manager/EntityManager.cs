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

        [EventSubscribe("EntitySpawn")]
        public object AddToEntityList(EntityBase entity)
        {
            Entities.Add(entity);
            print("Added");
            return null;
        }
        [EventSubscribe("EntityDie")]
        public object RemoveFromEntityList(EntityBase entity)
        {
            Entities.Remove(entity);
            print("Removed");
            return null;
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

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