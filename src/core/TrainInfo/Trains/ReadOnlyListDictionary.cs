using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace TrainInfo.Trains
{
    public class ReadOnlyListDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyList<KeyValuePair<TKey, TValue>>
    {
        public ReadOnlyListDictionary(Dictionary<TKey, TValue> keyValuePairs)
        {
            KeyList = new List<TKey>();
            ValueList = new List<TValue>();

            foreach (var kvp in keyValuePairs)
            {
                KeyList.Add(kvp.Key);
                ValueList.Add(kvp.Value);
            }
        }

        public ReadOnlyListDictionary(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            KeyList = new List<TKey>(Keys);
            ValueList = new List<TValue>(values);
        }

        public List<TKey> KeyList { get; }

        public List<TValue> ValueList { get; }

        public TValue this[TKey key] => ValueList[KeyList.IndexOf(key)];

        public IEnumerable<TKey> Keys => KeyList;

        public IEnumerable<TValue> Values => ValueList;

        public int Count => KeyList.Count;

        public KeyValuePair<TKey, TValue> this[int index] => new KeyValuePair<TKey, TValue>(KeyList[index], ValueList[index]);

        public bool ContainsKey(TKey key)
        {
            return KeyList.Contains(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetKeyValuePairs().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (KeyList.Contains(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs()
        {
            for (var i = 0; i < KeyList.Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(KeyList[i], ValueList[i]);
            }
        }
    }
}
