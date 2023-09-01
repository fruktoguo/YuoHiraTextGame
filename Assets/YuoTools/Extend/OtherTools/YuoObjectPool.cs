using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace YuoTools.Extend.OtherTools
{
    public class YuoObjectPool<T> : ObjectPool<T> where T : class
    {
        public YuoObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck,
            defaultCapacity, maxSize)
        {
        }

        private HashSet<T> m_Active = new();

        public new T Get()
        {
            var value = base.Get();
            m_Active.Add(value);
            return value;
        }

        public void ReleaseAll()
        {
            foreach (var item in m_Active)
            {
                base.Release(item);
            }
            m_Active.Clear();
        }
    }
}