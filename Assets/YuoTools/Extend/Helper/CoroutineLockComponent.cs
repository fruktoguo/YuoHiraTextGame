using System.Collections.Generic;
using ET;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.Helper
{
    public class CoroutineLockComponent : YuoComponentGet<CoroutineLockComponent>
    {
        public Dictionary<int, CoroutineLockItem> CoroutineLocks = new();

        public Dictionary<int, Queue<CoroutineLock>> CoroutineLockInfos = new();

        public static async ETTask<YuoEntity> Wait(int coroutineLockType, int time = 600000)
        {
            var instance = Get;
            if (!instance. CoroutineLocks.ContainsKey(coroutineLockType))
            {
                instance. CoroutineLocks.TryAdd(coroutineLockType,  instance. Entity.AddChild<CoroutineLockItem>());
            }

            var coroutineLock =  instance. CoroutineLocks[coroutineLockType];

            var tcs = ETTask<YuoEntity>.Create();

            var queue = coroutineLock.Entity.GetComponent<CoroutineLockQueue>();


            if (queue.Count == 0)
            {
                return queue.Enqueue(tcs, time).Entity;
            }

            var dequeue = queue.TcsDequeue();
            var returnEntity = queue.Enqueue(tcs, time).Entity;
            await dequeue.Tcs;
            // Debug.Log($"return {returnEntity}");
            return returnEntity;
        }
    }

    public class CoroutineLockComponentAwakeSystem : YuoSystem<CoroutineLockComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Main;
        protected override void Run(CoroutineLockComponent self)
        {
            self.Entity.EntityName = "CoroutineLockComponent";
        }
    }

    public class CoroutineLockAwakeSystem : YuoSystem<CoroutineLockItem>, IAwake
    {
        private int index;
        public override string Group => SystemGroupConst.Main;

        protected override void Run(CoroutineLockItem self)
        {
            self.AddComponent<CoroutineLockQueue>();
            self.Entity.EntityName = $"CoroutineLock_{index++}";
        }
    }

    public class CoroutineLockItem : YuoComponent
    {
    }

    public class CoroutineLockItemDestroySystem : YuoSystem<CoroutineLock>, IDestroy
    {
        public override string Group => SystemGroupConst.Main;
        protected override void Run(CoroutineLock self)
        {
            // Debug.Log($"销毁了 {self.Entity.EntityName}");
            self.Entity.Parent.GetComponent<CoroutineLockQueue>().Execute();
        }
    }

    public class CoroutineLock : YuoComponent
    {
        public ETTask<YuoEntity> Tcs;
        public int Time;

        public void Execute()
        {
            Tcs.SetResult(Entity);
        }
    }

    public class CoroutineLockQueue : YuoComponent
    {
        public Queue<CoroutineLock> tcsQueue = new Queue<CoroutineLock>();
        public Queue<CoroutineLock> executeQueue = new Queue<CoroutineLock>();
        private int index;

        public CoroutineLock Enqueue(ETTask<YuoEntity> tcs, int time)
        {
            var item = Entity.AddChild<CoroutineLock>();
            item.Entity.EntityName = $"CoroutineLockItem_{index++}";
            item.Tcs = tcs;
            item.Time = time;
            executeQueue.Enqueue(item);
            tcsQueue.Enqueue(item);
            return item;
        }

        public CoroutineLock TcsDequeue()
        {
            return tcsQueue.Dequeue();
        }

        public bool Execute()
        {
            if (executeQueue.Count > 0)
            {
                executeQueue.Dequeue().Execute();
                
                if (executeQueue.Count == 0 && tcsQueue.Count > 0)
                    tcsQueue.Dequeue();
                return true;
            }

            return false;
        }

        public int Count => executeQueue.Count;
    }

    public static partial class CoroutineLockType
    {
        public const int None = 0;
        public const int Location = 1; // location进程上使用
        public const int ActorLocationSender = 2; // ActorLocationSender中队列消息 
        public const int Mailbox = 3; // Mailbox中队列
        public const int UnitId = 4; // Map服务器上线下线时使用
        public const int DB = 5;
        public const int Resources = 6;
        public const int ResourcesLoader = 7;

        public const int Max = 100; // 这个必须最大
    }
}