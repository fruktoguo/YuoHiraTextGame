using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace YuoTools
{
    #region Delegate

    public delegate bool BoolAction();

    public delegate float FloatAction();

    public delegate int IntAction();

    public delegate string StringAction();

    public delegate bool BoolAction<T>(T t);

    public delegate float FloatAction<T>(T t);

    public delegate double DoubleAction<T>(T t);

    public delegate decimal DecimalAction<T>(T t);

    public delegate long LongAction<T>(T t);

    public delegate int IntAction<T>(T t);

    public delegate string StringAction<T>(T t);

    #endregion

    public class YuoOption<T>
    {
        Dictionary<string, T> Options = new();

        public void Set(string key, T value)
        {
            if (Options.ContainsKey(key))
            {
                Options[key] = value;
            }
            else
            {
                Options.Add(key, value);
            }
        }

        public T Get(string key)
        {
            if (Options.TryGetValue(key, out var option))
            {
                return option;
            }
            else
            {
                return default;
            }
        }
    }

    /// <summary>
    /// dic嵌套list
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    [Serializable]
    public class DicList<TK, TV> : Dictionary<TK, List<TV>>
    {
        private readonly List<TV> _empty = new List<TV>();

        public TV[] Copy(TK t)
        {
            TryGetValue(t, out var list);
            return list == null ? Array.Empty<TV>() : list.ToArray();
        }

        public new List<TV> this[TK t] => TryGetValue(t, out var list) ? list : _empty;

        public void AddItem(TK t, TV k)
        {
            if (!TryGetValue(t, out var list))
            {
                list = new List<TV>();
                Add(t, list);
            }

            list.Add(k);
        }

        public bool RemoveItem(TK t, TV k)
        {
            if (!TryGetValue(t, out var list))
            {
                Add(t, new List<TV>());
                return false;
            }

            if (list.Contains(k))
            {
                list.Remove(k);
                return true;
            }

            return false;
        }

        public List<TV> GetAll()
        {
            List<TV> values = new();
            foreach (var item in this)
            {
                values.AddRange(item.Value);
            }

            return values;
        }

        public new bool Remove(TK t)
        {
            if (!ContainsKey(t))
            {
                return false;
            }

            Remove(t);
            return true;
        }
    }

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

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private T[] queue;
        private int count;
        private Func<T, T, bool> cmp;

        public PriorityQueue(int capacity = 100, Func<T, T, bool> cmp = null)
        {
            this.queue = new T[capacity];
            this.count = 0;
            this.cmp = cmp ?? ((x, y) => x.CompareTo(y) < 0);
        }

        public int Count => count;

        public T Peek()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("队列为空");
            }

            return queue[1];
        }

        public void Enqueue(T val)
        {
            count++;
            if (count == queue.Length)
            {
                T[] tmp = new T[queue.Length * 2];
                for (int i = 0; i < count; i++)
                {
                    tmp[i] = queue[i];
                }

                queue = tmp;
            }

            queue[count] = val;
            Swim(count);
        }

        public T Dequeue()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("队列为空");
            }

            T val = queue[1];
            queue[1] = queue[count];
            queue[count] = default(T);
            count--;
            Sinking(1);
            return val;
        }

        private void Swim(int index)
        {
            while (index > 1 && cmp(queue[index], queue[index / 2]))
            {
                Swap(index, index / 2);
                index /= 2;
            }
        }

        private void Sinking(int index)
        {
            while (index * 2 <= count)
            {
                int child = index * 2;
                if (child + 1 <= count && cmp(queue[child + 1], queue[child]))
                {
                    child++;
                }

                if (!cmp(queue[index], queue[child]))
                {
                    break;
                }

                Swap(index, child);
                index = child;
            }
        }

        private void Swap(int x, int y)
        {
            (queue[x], queue[y]) = (queue[y], queue[x]);
        }
    }
}