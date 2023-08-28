using System;
using System.Collections.Generic;

namespace YuoTools.Main.Ecs
{
    /// <summary>
    /// dic嵌套list
    /// </summary>
    /// <typeparam name="Tk"></typeparam>
    /// <typeparam name="Tv"></typeparam>
    [Serializable]
    public class MultiMap<Tk, Tv> : Dictionary<Tk, List<Tv>>
    {
        private readonly List<Tv> _empty = new List<Tv>();

        public Tv[] Copy(Tk t)
        {
            TryGetValue(t, out var list);
            return list == null ? Array.Empty<Tv>() : list.ToArray();
        }

        public new List<Tv> this[Tk t] => TryGetValue(t, out var list) ? list : _empty;

        public void AddItem(Tk t, Tv k)
        {
            if (!TryGetValue(t, out var list))
            {
                list = new List<Tv>();
                Add(t, list);
            }

            list.Add(k);
        }

        public bool RemoveItem(Tk t, Tv k)
        {
            if (!TryGetValue(t, out var list))
            {
                Add(t, new List<Tv>());
                return false;
            }

            return list.Remove(k);
        }

        public List<Tv> GetAll()
        {
            List<Tv> values = new();
            foreach (var item in this)
            {
                values.AddRange(item.Value);
            }

            return values;
        }

        public new bool Remove(Tk t)
        {
            if (!ContainsKey(t))
            {
                return false;
            }

            Remove(t);
            return true;
        }
    }

    /// <summary>
    ///  dic嵌套dic
    /// </summary>
    public class MultiDicMap<Tk, Tvk, Tv> : Dictionary<Tk, Dictionary<Tvk, Tv>>
    {
        private readonly Dictionary<Tvk, Tv> _empty = new Dictionary<Tvk, Tv>();

        public Tv[] Copy(Tk t)
        {
            TryGetValue(t, out var dic);
            return dic == null ? Array.Empty<Tv>() : new List<Tv>(dic.Values).ToArray();
        }

        public Tv[] Copy(Tk t, Tvk tvk)
        {
            TryGetValue(t, out var dic);
            if (dic == null)
                return Array.Empty<Tv>();
            if (dic.TryGetValue(tvk, out var tv))
                return new[] { tv };
            return Array.Empty<Tv>();
        }

        public new Dictionary<Tvk, Tv> this[Tk t] => TryGetValue(t, out var dic) ? dic : _empty;

        public void AddItem(Tk t, Tvk tvk, Tv tv)
        {
            if (!TryGetValue(t, out var dic))
            {
                dic = new Dictionary<Tvk, Tv>();
                Add(t, dic);
            }

            dic.Add(tvk, tv);
        }

        public bool RemoveItem(Tk t, Tvk tvk)
        {
            if (!TryGetValue(t, out var dic))
            {
                Add(t, new Dictionary<Tvk, Tv>());
                return false;
            }

            if (dic.ContainsKey(tvk))
            {
                dic.Remove(tvk);
                return true;
            }

            return false;
        }

        public Tv[] GetAll()
        {
            List<Tv> values = new();
            foreach (var item in this)
            {
                values.AddRange(item.Value.Values);
            }

            return values.ToArray();
        }

        public new bool Remove(Tk t)
        {
            if (!ContainsKey(t))
            {
                return false;
            }

            Remove(t);
            return true;
        }
    }

    /// <summary>
    ///  dic嵌套hashset
    /// </summary>
    /// <typeparam name="Tk"></typeparam>
    /// <typeparam name="Tv"></typeparam>
    public class MultiHashSetMap<Tk, Tv> : Dictionary<Tk, HashSet<Tv>>
    {
        private readonly HashSet<Tv> _empty = new();

        public Tv[] Copy(Tk t)
        {
            TryGetValue(t, out var list);
            return list == null ? Array.Empty<Tv>() : new List<Tv>(list).ToArray();
        }

        public new HashSet<Tv> this[Tk t] => TryGetValue(t, out var list) ? list : _empty;

        public void AddItem(Tk t, Tv tv)
        {
            if (!TryGetValue(t, out var list))
            {
                list = new HashSet<Tv>();
                Add(t, list);
            }

            list.Add(tv);
        }

        public bool RemoveItem(Tk t, Tv tv)
        {
            if (!TryGetValue(t, out var list))
            {
                Add(t, new HashSet<Tv>());
                return false;
            }

            return list.Remove(tv);
        }

        public Tv[] GetAll()
        {
            List<Tv> values = new();
            foreach (var item in this)
            {
                values.AddRange(item.Value);
            }

            return values.ToArray();
        }

        public new bool Remove(Tk t)
        {
            if (!ContainsKey(t))
            {
                return false;
            }

            Remove(t);
            return true;
        }
        
        public Tv GetFirst(Tk t)
        {
            if (!TryGetValue(t, out var list))
            {
                return default;
            }

            foreach (var item in list)
            {
                return item;
            }

            return default;
        }
        
        public bool TryGetFirst(Tk t,out Tv tv)
        {
            if (!TryGetValue(t, out var list))
            {
                tv = default;
                return false;
            }

            foreach (var item in list)
            {
                tv = item;
                return true;
            }

            tv = default;
            return false;
        }
    }
}