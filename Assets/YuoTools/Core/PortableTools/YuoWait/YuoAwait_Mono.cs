using System;
using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using YuoTools;
using YuoTools.Main.Ecs;
using Object = UnityEngine.Object;

namespace YuoTools
{
    public class YuoAwait_Mono : SingletonMono<YuoAwait_Mono>
    {
        [SerializeField] private AwaitPools awaitPools = new AwaitPools();

        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            gameObject.name = "YuoAwait";
        }

        private void Update()
        {
            CheckTimerOut();
        }

        private List<AwaitItem> removeList = new List<AwaitItem>();

        private void CheckTimerOut()
        {
            if (awaitPools.ActiveCount > 0)
            {
                for (int i = 0; i < awaitPools.Actives.Count; i++)
                {
                    var item = awaitPools.Actives[i];

                    switch (item.timeType)
                    {
                        case TimeType.Normal:
                            if (Time.time < item.TargetTime) continue;
                            break;
                        case TimeType.Unscaled:
                            if (Time.unscaledTime < item.TargetTime) continue;
                            break;
                        case TimeType.Frame:
                            if (Time.frameCount < item.TargetFrame) continue;
                            break;
                    }

                    item.tcs.SetResult();
                    removeList.Add(item);
                }

                if (removeList.Count > 0)
                {
                    foreach (var item in removeList)
                    {
                        awaitPools.Remove(item);
                    }

                    removeList.Clear();
                }
            }
        }

        public async ETTask WaitUnscaledTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.timeType = TimeType.Unscaled;
            item.CreatTime = Time.unscaledTime;
            item.TargetTime = Time.unscaledTime + waitTime;
            await item.tcs;
        }

        public async ETTask WaitTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.timeType = TimeType.Normal;
            item.CreatTime = Time.time;
            waitTime.Clamp(0.00001f, float.MaxValue);
            item.TargetTime = Time.time + waitTime;
            await item.tcs;
        }

        public async ETTask WaitFrameAsync(int waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.timeType = TimeType.Frame;
            item.CreatFrame = Time.frameCount;
            waitTime.Clamp(1, int.MaxValue);
            item.TargetFrame = Time.frameCount + waitTime;
            await item.tcs;
        }

        public ETTask<T> ResourcesLoadAsync<T>(string path)
            where T : Object
        {
            ETTask<T> tcs = ETTask<T>.Create(true);
            Instance.StartCoroutine(LoadAsset(path, tcs));
            return tcs;
        }

        IEnumerator LoadAsset<T>(string path, ETTask<T> tcs)
            where T : Object
        {
            ResourceRequest asset = Resources.LoadAsync<T>(path);
            yield return asset;
            var go = asset.asset as T;
            tcs.SetResult(go);
        }

        private class AwaitItem
        {
            public ETTask tcs;

            /// <summary>
            /// 添加时间
            /// </summary>
            public float CreatTime = 0;

            /// <summary>
            /// 目标时间
            /// </summary>
            public float TargetTime = 0;

            public int TargetFrame = 0;

            public float CreatFrame = 0;

            public TimeType timeType = TimeType.Normal;

            public AwaitItem()
            {
                tcs = ETTask.Create(true);
            }
        }

        public enum TimeType
        {
            Normal = 0,
            Unscaled = 1,
            Frame = 2
        }

        private class AwaitPools : PoolsBase<AwaitItem>
        {
            public override AwaitItem CreatItem()
            {
                return new AwaitItem();
            }

            public override void OnDestroyItem(AwaitItem item)
            {
            }

            public override void OnResetItem(AwaitItem item)
            {
                item.timeType = TimeType.Normal;
                item.tcs = ETTask.Create(true);
            }
        }
    }
}