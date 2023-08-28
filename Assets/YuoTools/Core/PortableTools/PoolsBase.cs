using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------------------------
--  对象池基类
--  提供对象池的基本操作(添加、移除、获取)
--  ItemPrefab 可以在创建对象池的时候设置预设体,用于创建一个新item时调用
--  GetItem() 获取一个item,如果池中没有可用的item,则创建一个新的item
--  Remove() 回收一个item,但这个item比如是由对象池创建的,否则不会回收
----------------------------------------------------------------------------*/

namespace YuoTools
{
    public abstract class PoolsBase<T>
    {
        //public List<T> Actives { get; private set; } = new List<T>();
        public List<T> Actives = new List<T>();
        [SerializeField] private Queue<T> Pools = new Queue<T>();
        [SerializeField] private List<T> removeList = new List<T>();

        public void Remove(T item)
        {
            if (Actives.Contains(item))
            {
                Actives.Remove(item);
                if (MaxPoolCount >= 0 && Pools.Count > MaxPoolCount)
                    OnDestroyItem(item);
                else
                    Pools.Enqueue(item);
                OnRemoveItem(item);
            }
            else
            {
                $"要移除的物体 [{item}] 不属于该对象池".Log("#ff0000");
            }
        }

        public void UpdateRemove()
        {
            if (removeList.Count > 0)
            {
                foreach (var item in removeList)
                {
                    Remove(item);
                }

                removeList.Clear();
            }
        }

        public void RemoveWait(T item)
        {
            removeList.Add(item);
        }

        public int ActiveCount
        {
            get => Actives.Count;
        }

        /// <summary>
        /// 池子最大数量，负数则为无限制
        /// </summary>
        public int MaxPoolCount = -1;

        /// <summary>
        /// 创建物体的预设体
        /// </summary>
        public T ItemPrefab;

        /// <summary>
        /// 临时变量
        /// </summary>
        private T ItemTemp;

        /// <summary>
        /// 获取一个item
        /// </summary>
        /// <returns></returns>
        public T GetItem()
        {
            if (Pools.Count > 0)
            {
                ItemTemp = Pools.Dequeue();
            }
            else
            {
                ItemTemp = CreatItem();
            }

            OnResetItem(ItemTemp);

            Actives.Add(ItemTemp);
            return ItemTemp;
        }

        /// <summary>
        /// 创建新的item
        /// </summary>
        /// <returns></returns>
        public abstract T CreatItem();

        /// <summary>
        /// 获取一个item时自动重置
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnResetItem(T item);

        public virtual void OnRemoveItem(T item)
        {
            
        }

        /// <summary>
        /// 超出最大池子容量时销毁
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnDestroyItem(T item);
    }
}