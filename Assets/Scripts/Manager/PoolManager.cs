﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Manager
{
    public interface IObjectPool
    {
        Type ObjectType { get; }
        MonoBehaviour Get(Transform parent);
        void Release(MonoBehaviour obj);
    }

    public class GameObjectPool<T> : IObjectPool where T : MonoBehaviour
    {
        private readonly List<T> _activeObjects = new();
        private readonly int _maxActiveObjects;
        private readonly ObjectPool<T> _pool;
        private readonly Transform _poolRoot;
        private readonly T _prefab;

        public GameObjectPool(T prefab, Transform poolRoot, int defaultCapacity = 10, int maxSize = 20)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (poolRoot == null) throw new ArgumentNullException(nameof(poolRoot));

            _prefab = prefab;
            _poolRoot = poolRoot;
            _maxActiveObjects = maxSize;

            _pool = new ObjectPool<T>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                defaultCapacity,
                maxSize
            );
        }


        public Type ObjectType => typeof(T);

        MonoBehaviour IObjectPool.Get(Transform parent)
        {
            return Get(parent);
        }

        void IObjectPool.Release(MonoBehaviour obj)
        {
            Release(obj as T);
        }

        private T CreatePooledItem()
        {
            var instance = Object.Instantiate(_prefab, _poolRoot);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnTakeFromPool(T obj)
        {
            obj.gameObject.SetActive(true);
            _activeObjects.Add(obj);
            MaintainActiveObjectsLimit();
        }

        private void OnReturnedToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolRoot);
            _activeObjects.Remove(obj);
        }

        private void OnDestroyPoolObject(T obj)
        {
            if (obj != null) Object.Destroy(obj.gameObject);
        }

        private void MaintainActiveObjectsLimit()
        {
            while (_activeObjects.Count > _maxActiveObjects)
            {
                var oldest = _activeObjects[0];
                _activeObjects.RemoveAt(0);
                _pool.Release(oldest);
            }
        }

        public T Get(Transform parent)
        {
            var item = _pool.Get();
            item.transform.SetParent(parent);
            item.transform.SetPositionAndRotation(parent.position, parent.rotation);
            return item;
        }

        public void Release(T obj)
        {
            _pool.Release(obj);
        }
    }

    public static class PoolManager
    {
        private static readonly Dictionary<string, IObjectPool> _pools = new();
        private static Transform _poolRoot;

        private static Transform PoolRoot
        {
            get
            {
                if (_poolRoot != null) return _poolRoot;
                _poolRoot = new GameObject("PoolManager").transform;
                Object.DontDestroyOnLoad(_poolRoot);
                return _poolRoot;
            }
        }

        public static void CreatePool<T>(string poolId, T prefab, int defaultCapacity = 10, int maxSize = 20)
            where T : MonoBehaviour
        {
            if (string.IsNullOrEmpty(poolId))
            {
                Debug.LogError("池ID不能为空。");
                return;
            }

            if (_pools.ContainsKey(poolId))
            {
                Debug.LogWarning($"对象池 '{poolId}' 已存在。");
                return;
            }

            var container = new GameObject($"{poolId}_Container").transform;
            container.SetParent(PoolRoot);

            try
            {
                var pool = new GameObjectPool<T>(prefab, container, defaultCapacity, maxSize);
                _pools.Add(poolId, pool);
            }
            catch (Exception ex)
            {
                Debug.LogError($"创建对象池失败: {ex.Message}。");
                Object.Destroy(container.gameObject);
            }
        }

        public static T Get<T>(string poolId, Transform parent = null) where T : MonoBehaviour
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                Debug.LogError($"对象池 '{poolId}' 不存在。");
                return null;
            }

            if (pool.ObjectType == typeof(T)) return pool.Get(parent ?? PoolRoot) as T;
            Debug.LogError($"类型不匹配: 请求的 {typeof(T)} 但池包含 {pool.ObjectType}。");
            return null;
        }

        public static void Release<T>(string poolId, T obj) where T : MonoBehaviour
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                Debug.LogError($"对象池 '{poolId}' 不存在。");
                return;
            }

            if (pool.ObjectType != typeof(T))
            {
                Debug.LogError($"类型不匹配: 尝试释放 {typeof(T)} 到包含 {pool.ObjectType} 的池。");
                return;
            }

            pool.Release(obj);
        }

        public static void DisposePool(string poolId)
        {
            if (!_pools.TryGetValue(poolId, out var pool)) return;

            try
            {
                if (pool is IDisposable disposablePool) disposablePool.Dispose();

                var container = PoolRoot.Find($"{poolId}_Container");
                if (container != null) Object.Destroy(container.gameObject);
            }
            finally
            {
                _pools.Remove(poolId);
            }
        }

        public static void DisposeAllPools()
        {
            foreach (var poolId in _pools.Keys.ToArray()) DisposePool(poolId);
        }
    }
}