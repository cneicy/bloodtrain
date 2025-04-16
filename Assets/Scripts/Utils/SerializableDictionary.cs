namespace Utils
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    [Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] private List<KeyValuePair<TKey, TValue>> entries = new();
        private Dictionary<TKey, TValue> _dictionary = new();

        /// <summary>
        /// 将 entries 数据同步到字典（需在修改 entries 后手动调用）
        /// </summary>
        public void SyncDictionary()
        {
            _dictionary.Clear();
            foreach (var entry in entries)
            {
                // 如果遇到重复键，后面的值会覆盖前面的
                _dictionary[entry.key] = entry.value;
            }
        }

        /// <summary>
        /// 添加键值对（若键已存在会抛出异常）
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"键 '{key}' 已经存在");
            }

            // 直接操作 entries 并同步到字典
            entries.Add(new KeyValuePair<TKey, TValue>(key, value));
            _dictionary.Add(key, value); // 直接更新字典避免重复 SyncDictionary
        }

        /// <summary>
        /// 移除指定键的条目
        /// </summary>
        public bool Remove(TKey key)
        {
            // 从 entries 中移除
            var index = entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.key, key));
            if (index < 0) return false;
            entries.RemoveAt(index);
            _dictionary.Remove(key); // 直接更新字典
            return true;
        }
        
        public void Clear()
        {
            entries.Clear();
            _dictionary.Clear();
        }

        /// <summary>
        /// 索引器访问
        /// </summary>
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                // 更新或添加条目
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                    // 同步更新 entries
                    var index = entries.FindIndex(e => EqualityComparer<TKey>.Default.Equals(e.key, key));
                    entries[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    entries.Add(new KeyValuePair<TKey, TValue>(key, value));
                    _dictionary.Add(key, value);
                }
            }
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// 安全获取值（避免 KeyNotFoundException）
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
    }
}