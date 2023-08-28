using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace YuoTools
{
    public class YuoEvent<T>
    {
        public T Value { get; private set; }
        private List<UnityAction<T>> actions = new List<UnityAction<T>>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="single">同一个事件仅能添加一次</param>
        public void AddAction(UnityAction<T> action, bool single = false)
        {
            if (!single || !actions.Contains(action))
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="action"></param>
        public void RemoveAction(UnityAction<T> action)
        {
            if (actions.Contains(action))
            {
                actions.Remove(action);
            }
        }

        public YuoEvent(T v)
        {
            Value = v;
        }

        /// <summary>
        /// 统一执行事件
        /// </summary>
        public void Invoke()
        {
            foreach (var item in actions)
            {
                item?.Invoke(Value);
            }
        }
    }
}