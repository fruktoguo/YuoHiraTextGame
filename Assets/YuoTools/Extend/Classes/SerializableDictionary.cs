using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    [Serializable]
    public class SerializableDictionary<TK, TV> : IDictionary<TK, TV>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TK> mKeys;

        [SerializeField] private List<TV> mValues;

        private Dictionary<TK, TV> _mDictionary = new Dictionary<TK, TV>();
        private int _i;

        public void Foreach(Action<TK> keys, Action<TV> values)
        {
            for (_i = 0; _i < mKeys.Count; _i++)
            {
                keys?.Invoke(mKeys[_i]);
                values?.Invoke(mValues[_i]);
            }
        }

        public TV this[TK key]
        {
            get => _mDictionary[key];
            set => _mDictionary[key] = value;
        }

        public TK this[TV value]
        {
            get
            {
                mKeys = new List<TK>(_mDictionary.Keys);
                mValues = new List<TV>(_mDictionary.Values);
                var index = mValues.FindIndex(x => x.Equals(value));
                if (index < 0)
                    throw new KeyNotFoundException();
                return mKeys[index];
            }
        }

        public ICollection<TK> Keys => _mDictionary.Keys;

        public ICollection<TV> Values => _mDictionary.Values;

        public void Add(TK key, TV value)
        {
            _mDictionary.Add(key, value);
        }

        public bool ContainsKey(TK key)
        {
            return _mDictionary.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            return _mDictionary.Remove(key);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return _mDictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            _mDictionary.Clear();
        }

        public int Count => _mDictionary.Count;

        bool ICollection<KeyValuePair<TK, TV>>.IsReadOnly =>
            (_mDictionary as ICollection<KeyValuePair<TK, TV>>).IsReadOnly;

        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> item)
        {
            (_mDictionary as ICollection<KeyValuePair<TK, TV>>).Add(item);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> item)
        {
            return (_mDictionary as ICollection<KeyValuePair<TK, TV>>).Contains(item);
        }

        void ICollection<KeyValuePair<TK, TV>>.CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            (_mDictionary as ICollection<KeyValuePair<TK, TV>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> item)
        {
            return (_mDictionary as ICollection<KeyValuePair<TK, TV>>).Remove(item);
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return (_mDictionary as IEnumerable<KeyValuePair<TK, TV>>).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _mDictionary.GetEnumerator();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            mKeys = new List<TK>(_mDictionary.Keys);
            mValues = new List<TV>(_mDictionary.Values);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Debug.Assert(mKeys.Count == mValues.Count);
            Clear();
            for (var i = 0; i < mKeys.Count; ++i)
                Add(mKeys[i], mValues[i]);
        }
    }
}