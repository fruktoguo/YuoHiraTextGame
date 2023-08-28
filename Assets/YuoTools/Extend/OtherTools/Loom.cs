using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace YuoTools
{
    public class Loom : SingletonMono<Loom>
    {
        /// <summary>
        /// 最大同时运行线程数
        /// </summary>
        [Header("最大同时运行线程数")]
        public int maxThreads = 7;

        /// <summary>
        /// 当前运行线程数
        /// </summary>
        [ReadOnly] public int numThreads;

        public override void Awake()
        {
            base.Awake();
        }

        private List<Action> _actions = new List<Action>();

        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        private Dictionary<int, bool> dic = new Dictionary<int, bool>();
        private Dictionary<int, LoomData> datas = new Dictionary<int, LoomData>();

        public List<int> dicTemps = new List<int>();
        public List<int> datasTemps = new List<int>();

        public List<int> Removes = new List<int>();

        public class LoomData
        {
            public int ID;
            public Action callBack;
        }

        public LoomPools Pools = new LoomPools();

        public static int GetNewID
        {
            get
            {
                _newID++;
                if (_newID > 1000000)
                {
                    _newID = 0;
                }

                return _newID;
            }
        }

        private static int _newID = -1;

        public class LoomPools : PoolsBase<LoomData>
        {
            public override LoomData CreatItem()
            {
                return new LoomData();
            }

            public override void OnDestroyItem(LoomData item)
            {
                throw new NotImplementedException();
            }

            public override void OnResetItem(LoomData item)
            {
                item.ID = 0;
            }
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (Instance._delayed)
                {
                    Instance._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (Instance._actions)
                {
                    Instance._actions.Add(action);
                }
            }
        }

        /// <summary>
        /// 异步调用一个方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static LoomData RunAsync(Action action)
        {
            return Instance.PrivateRunAsync(action);
        }

        private LoomData data;

        private LoomData PrivateRunAsync(Action action)
        {
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }

            Interlocked.Increment(ref numThreads);
            data = Pools.GetItem();
            data.ID = GetNewID;
            dic.Add(data.ID, false);
            datas.Add(data.ID, data);
            action += () => { dic[data.ID] = true; };

            ThreadPool.QueueUserWorkItem(RunAction, action);
            return data;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref Instance.numThreads);
            }
        }

        private List<Action> _currentActions = new List<Action>();

        private void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }

            foreach (var item in _currentActions)
            {
                item?.Invoke();
            }

            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (var item in _currentDelayed)
                {
                    _delayed.Remove(item);
                }
            }

            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }

            dicTemps.Clear();
            foreach (var item in dic)
            {
                dicTemps.Add(item.Key);
            }

            datasTemps.Clear();
            foreach (var item in datas)
            {
                datasTemps.Add(item.Key);
            }

            Removes.Clear();
            foreach (var item in dic)
            {
                if (item.Value)
                {
                    Removes.Add(item.Key);
                }
            }

            foreach (var item in Removes)
            {
                if (!datas.ContainsKey(item))
                {
                    continue;
                }

                dic.Remove(item);
                datas[item].callBack?.Invoke();
                Pools.Remove(datas[item]);
                datas.Remove(item);
            }

            Removes.Clear();
        }
    }
}