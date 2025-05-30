﻿using System;
using System.Collections.Generic;
using Entity.Base;
using UnityEngine;
using Utils;

namespace Manager
{
    /// <summary>
    /// 实体管理器
    /// 原理是模仿ECS统一对实体行为进行调用管理
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private GameObject _cameraobj;
        public List<EntityBase> Entities { get; set; } = new();

        private void FixedUpdate()
        {
            if (Entities.Count <= 0) return;
            try
            {
                Entities.ForEach(entity => entity.OnUpdate(_cameraobj.transform));
            }
            catch (Exception)
            {
                Debug.LogWarning("实体列表被修改");
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            _cameraobj = Camera.main.gameObject;
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        [EventSubscribe("GameFail")]
        public object OnGameFail(float speed)
        {
            PoolManager.DisposePool("Enemy");
            return null;
        }

        [EventSubscribe("EntitySpawn")]
        public object AddToEntityList(EntityBase entity)
        {
            Entities.Add(entity);
            return null;
        }

        [EventSubscribe("EntityDie")]
        public object RemoveFromEntityList(EntityBase entity)
        {
            Entities.Remove(entity);
            return null;
        }
    }
}