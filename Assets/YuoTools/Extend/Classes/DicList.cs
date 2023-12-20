using System;
using System.Collections.Generic;

namespace YuoTools
{
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
}