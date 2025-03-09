using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Utils.Pool
{
    public class SingletonObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region 单例

        private static SingletonObjectPool<T> _instance;

        public static SingletonObjectPool<T> Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindObjectOfType<SingletonObjectPool<T>>();
                if (_instance) return _instance;
                var obj = new GameObject(typeof(SingletonObjectPool<T>).Name);
                _instance = obj.AddComponent<SingletonObjectPool<T>>();
                return _instance;
            }
        }

        #endregion


        private ObjectPool<T> _pool;
        private T _prefab;
        private Queue<T> _activeObjects;
        private int _maxActiveObjects;

        public void Init(T prefab, int defaultCapacity = 1, int maxSize = 10)
        {
            if (prefab is null)
            {
                Debug.LogError("预制体为空。");
                return;
            }

            _prefab = prefab;
            _maxActiveObjects = maxSize;
            _pool = new ObjectPool<T>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true, defaultCapacity,
                maxSize);
            _activeObjects = new Queue<T>();
        }

        private T CreateFunc()
        {
            return Instantiate(_prefab);
        }

        private void ActionOnGet(T obj)
        {
            obj.gameObject.SetActive(true);
            _activeObjects.Enqueue(obj);
        }

        private void ActionOnRelease(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void ActionOnDestroy(T obj)
        {
            Destroy(obj.gameObject);
        }

        //供滴人死亡、子弹命中等调用
        public void Release(T obj)
        {
            if (_pool is null)
            {
                Debug.LogWarning(typeof(T).Name + " 对象池未实例化。");
                return;
            }

            _activeObjects.Dequeue();
            _pool.Release(obj);
        }

        public T Spawn(Transform parent)
        {
            if (_pool is null)
            {
                Debug.LogWarning(typeof(T).Name + " 对象池未实例化。");
                return null;
            }

            //检查是否超出容量
            if (_activeObjects.Count >= _maxActiveObjects)
            {
                var oldestObject = _activeObjects.Peek();
                Release(oldestObject);
            }

            var temp = _pool.Get();
            temp.transform.position = parent.position;
            return temp;
        }
    }
}