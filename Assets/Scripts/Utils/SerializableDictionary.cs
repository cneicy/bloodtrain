namespace Utils
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] private List<KeyValuePair<TKey, TValue>> entries = new();
        private Dictionary<TKey, TValue> dictionary = new();

        public void SyncDictionary()
        {
            dictionary.Clear();
            foreach (var entry in entries)
            {
                dictionary[entry.Key] = entry.Value;
            }
        }

        public TValue this[TKey key] => dictionary[key];
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
    }
}