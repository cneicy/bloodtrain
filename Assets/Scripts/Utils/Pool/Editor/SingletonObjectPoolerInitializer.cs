using UnityEditor;
using UnityEngine;

namespace Utils.Pool.Editor
{
    [InitializeOnLoad]
    public static class SingletonObjectPoolInitializer
    {
        static SingletonObjectPoolInitializer()
        {
            CreateSingletonInstances();
        }

        private static void CreateSingletonInstances()
        {
            var types = typeof(SingletonObjectPool<>).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsSubclassOf(typeof(MonoBehaviour)) || !type.BaseType.IsGenericType ||
                    type.BaseType.GetGenericTypeDefinition() != typeof(SingletonObjectPool<>)) continue;
                var existingInstances = Object.FindObjectsOfType(type);
                if (existingInstances.Length != 0) continue;
                var obj = new GameObject(type.Name);
                obj.AddComponent(type);
                Debug.Log(type.Name + " 单例已创建。");
            }
        }
    }
}