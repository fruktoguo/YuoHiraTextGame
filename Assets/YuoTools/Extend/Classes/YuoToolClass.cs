using System;
using System.Collections.Generic;
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
        Dictionary<string, T> options = new();

        public void Set(string key, T value)
        {
            if (options.ContainsKey(key))
            {
                options[key] = value;
            }
            else
            {
                options.Add(key, value);
            }
        }

        public T Get(string key)
        {
            return options.GetValueOrDefault(key);
        }
    }

    /// <summary>
    /// 横纵表
    /// </summary>
    public class HvRulerTable
    {
        public class RulerTable<T> where T : IComparable<T>
        {
            public List<T> Ruler = new();

            public void Add(T v)
            {
                Ruler.Add(v);
                //排序
                Ruler.Sort();
            }

            public bool Remove(T v)
            {
                return Ruler.Remove(v);
            }

            public bool Contains(T v)
            {
                return Ruler.Contains(v);
            }
            
            public int IndexOf(T v)
            {
                return Ruler.IndexOf(v);
            }
        }
    }
}