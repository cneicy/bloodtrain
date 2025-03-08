using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object LockObj = new();

        // 是否在销毁过程中
        private static bool _isShuttingDown;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is already destroyed. Returning null.");
                    return null;
                }

                lock (LockObj)
                {
                    if (_instance != null) return _instance;
                    // 尝试查找已有的实例
                    _instance = FindObjectOfType<T>();

                    // 如果没有找到，则新建一个实例
                    if (_instance != null) return _instance;
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            // 确保只有一个实例存在
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        private void OnDestroy()
        {
            _isShuttingDown = true;
        }
    }
}