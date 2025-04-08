using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Manager
{
    /// <summary>
    /// 对象池基础接口
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// 获取池管理的对象类型
        /// </summary>
        Type ObjectType { get; }
        /// <summary>
        /// 从对象池获取对象实例
        /// <para>⚠️ 请使用泛型版本 Get<T> 确保类型安全</para>
        /// </summary>
        /// <param name="parent">指定父级变换（null 时使用池根节点）</param>
        MonoBehaviour Get(Transform parent);
        /// <summary>
        /// 将对象回收到对象池
        /// <para>⚠️ 请确保回收对象类型与池类型匹配</para>
        /// </summary>
        /// <param name="obj">要回收的对象实例</param>
        void Release(MonoBehaviour obj);
    }

    public class GameObjectPool<T> : IObjectPool where T : MonoBehaviour
    {
        private readonly List<T> _activeObjects = new();
        private readonly int _maxActiveObjects;
        private readonly ObjectPool<T> _pool;
        private readonly Transform _poolRoot;
        private readonly T _prefab;
        
        /// <summary>
        /// 创建新的对象池实例
        /// </summary>
        /// <param name="prefab">对象预制体（不可为空）</param>
        /// <param name="poolRoot">池对象的根节点变换（不可为空）</param>
        /// <param name="defaultCapacity">对象池默认容量（建议设置为常用数量）</param>
        /// <param name="maxSize">最大对象数量限制（包含活跃和闲置对象）</param>
        /// <exception cref="ArgumentNullException">当 prefab 或 poolRoot 为 null 时抛出</exception>
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

        /// <summary>
        /// 获取此对象池管理的类型
        /// </summary>
        public Type ObjectType => typeof(T);
        
        
        MonoBehaviour IObjectPool.Get(Transform parent)
        {
            return Get(parent);
        }
        
        void IObjectPool.Release(MonoBehaviour obj)
        {
            if (obj is T target)
            {
                Release(target);
            }
            else
            {
                Debug.LogError($"无法将类型 {obj.GetType()} 转换为 {typeof(T)}");
            }
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

        /// <summary>
        /// 从对象池获取对象实例（泛型安全版本）
        /// <para>自动设置对象位置和旋转到父节点</para>
        /// </summary>
        /// <param name="parent">指定父级变换（null 时使用池根节点）</param>
        /// <returns>激活状态的对象实例</returns>
        public T Get(Transform parent)
        {
            var item = _pool.Get();
            item.transform.SetParent(parent);
            item.transform.SetPositionAndRotation(parent.position, parent.rotation);
            return item;
        }

        /// <summary>
        /// 将对象回收到对象池
        /// <para>⚠️ 回收后对象会重置到池根节点</para>
        /// </summary>
        /// <param name="obj">要回收的有效对象实例</param>
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

        /// <summary>
        /// 创建新的对象池
        /// <para>⚠️ 重复创建相同ID的池会触发警告</para>
        /// </summary>
        /// <typeparam name="T">MonoBehaviour派生类型</typeparam>
        /// <param name="poolId">唯一池标识符（区分大小写）</param>
        /// <param name="prefab">对象预制体</param>
        /// <param name="defaultCapacity">初始容量（建议设置为常用数量）</param>
        /// <param name="maxSize">最大对象数量限制</param>
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

        /// <summary>
        /// 从指定对象池获取对象实例
        /// <para>自动激活并定位到父节点</para>
        /// </summary>
        /// <typeparam name="T">请求的对象类型</typeparam>
        /// <param name="poolId">目标池标识符</param>
        /// <param name="parent">指定父节点（null 时使用池根节点）</param>
        /// <returns>激活状态的对象或 null（池不存在/类型不匹配时）</returns>
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

        /// <summary>
        /// 将对象回收到指定池
        /// <para>⚠️ 错误回收会导致对象被销毁</para>
        /// </summary>
        /// <param name="poolId">目标池标识符</param>
        /// <param name="obj">要回收的有效对象实例</param>
        public static void Release(string poolId, MonoBehaviour obj)
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                Debug.LogError($"对象池 '{poolId}' 不存在。");
                return;
            }
            if (obj.GetType() != pool.ObjectType)
            {
                Debug.LogError($"类型不匹配: 尝试释放 {obj.GetType()} 到包含 {pool.ObjectType} 的池");
                return;
            }

            pool.Release(obj);
        }

        /// <summary>
        /// 销毁指定对象池及其管理的所有对象
        /// <para>⚠️ 立即释放所有资源，谨慎使用</para>
        /// </summary>
        /// <param name="poolId">要销毁的池标识符</param>
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

        /// <summary>
        /// 强制销毁所有对象池
        /// <para>通常在场景切换时调用</para>
        /// </summary>
        public static void DisposeAllPools()
        {
            foreach (var poolId in _pools.Keys.ToArray()) DisposePool(poolId);
        }
    }
}