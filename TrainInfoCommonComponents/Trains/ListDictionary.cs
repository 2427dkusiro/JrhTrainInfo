using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TrainInfo.Trains
{
    public class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
    {
        public ListDictionary()
        {
            KeyList = new List<TKey>();
            ValueList = new List<TValue>();
        }

        public ListDictionary(Dictionary<TKey, TValue> keyValuePairs)
        {
            KeyList = new List<TKey>();
            ValueList = new List<TValue>();

            foreach (var kvp in keyValuePairs)
            {
                KeyList.Add(kvp.Key);
                ValueList.Add(kvp.Value);
            }
        }

        public ListDictionary(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            KeyList = new List<TKey>(Keys);
            ValueList = new List<TValue>(values);
        }

        public void Sort()
        {
            
        }

        public List<TKey> KeyList { get; }

        public List<TValue> ValueList { get; }

        public TValue this[TKey key] => ValueList[KeyList.IndexOf(key)];

        public IEnumerable<TKey> Keys => KeyList;

        public IEnumerable<TValue> Values => ValueList;

        public int Count => KeyList.Count;

        public bool IsReadOnly => false;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => KeyList;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => ValueList;

        TValue IDictionary<TKey, TValue>.this[TKey key] { get => ValueList[KeyList.IndexOf(key)]; set => ValueList[KeyList.IndexOf(key)] = value; }

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get => new KeyValuePair<TKey, TValue>(KeyList[index], ValueList[index]);
            set
            {
                KeyList[index] = value.Key;
                ValueList[index] = value.Value;
            }
        }

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

        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            return KeyList.IndexOf(item.Key);
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            KeyList.Insert(index, item.Key);
            ValueList.Insert(index, item.Value);
        }

        public void RemoveAt(int index)
        {
            KeyList.RemoveAt(index);
            ValueList.RemoveAt(index);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            KeyList.Add(item.Key);
            ValueList.Add(item.Value);
        }

        public void Clear()
        {
            KeyList.Clear();
            ValueList.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return KeyList.Contains(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < KeyList.Count; i++)
            {
                array[arrayIndex + 1] = new KeyValuePair<TKey, TValue>(KeyList[i], ValueList[i]);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return KeyList.Remove(item.Key) & ValueList.Remove(item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            KeyList.Add(key);
            ValueList.Add(value);
        }

        public bool Remove(TKey key)
        {
            var index = KeyList.IndexOf(key);
            if (index == -1)
            {
                return false;
            }
            KeyList.RemoveAt(index);
            ValueList.RemoveAt(index);
            return true;
        }
    }
}
