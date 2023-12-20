using System.Collections;
using System.Collections.Generic;
using ET;
using Unity.Jobs;
using UnityEngine.Events;

namespace YuoTools.Deprecated
{
    public static class YuoWaitForJobs
    {
        public static void YuoSchedule<T>(this T jobData, UnityAction onCompleted, int arrayLength,
            int innerLoopBatchCount) where T : struct, IJobParallelFor
        {
            YuoAwait_Mono.Instance.StartCoroutine(
                YuoWaitDForJobEnumerator(jobData.Schedule(arrayLength, innerLoopBatchCount), onCompleted));
        }

        public static void YuoSchedule<T>(this T jobData, UnityAction onCompleted) where T : struct, IJob
        {
            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumerator(jobData.Schedule(), onCompleted));
        }

        public static async ETTask YuoScheduleAsync<T>(this T jobData) where T : struct, IJob
        {
            ETTask tcs = ETTask.Create();
            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumeratorAsync(jobData.Schedule(), tcs, null));
            await tcs;
        }

        public static ETTask YuoScheduleAsyncNoWait<T>(this T jobData, UnityAction onCompleted) where T : struct, IJob
        {
            ETTask tcs = ETTask.Create();
            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumeratorAsync(jobData.Schedule(), tcs, onCompleted));
            return tcs;
        }

        public static async ETTask AwaitAll(this List<ETTask> tasks)
        {
            foreach (var t in tasks)
            {
                await t;
            }
        }

        public static IEnumerator YuoWaitDForJobEnumeratorAsync(JobHandle handle, ETTask tcs, UnityAction onCompleted)
        {
            while (!handle.IsCompleted)
            {
                yield return null;
            }

            handle.Complete();
            onCompleted?.Invoke();
            tcs.SetResult();
        }

        public static IEnumerator YuoWaitDForJobEnumerator(JobHandle handle, UnityAction onCompleted)
        {
            while (!handle.IsCompleted)
            {
                yield return null;
            }

            handle.Complete();
            onCompleted?.Invoke();
        }

        public static void YuoSchedule(this List<JobHandle> jobData, UnityAction<int> onCompleted, UnityAction onEnd)
        {
            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumerator(jobData, onCompleted, onEnd));
        }

        public static void YuoSchedule<T>(this List<T> jobData, UnityAction<int> onCompleted, UnityAction onEnd)
            where T : struct, IJob
        {
            List<JobHandle> handles = new List<JobHandle>();
            foreach (var item in jobData)
            {
                handles.Add(item.Schedule());
            }

            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumerator(handles, onCompleted, onEnd));
        }

        public static void YuoSchedule<T>(this List<T> jobData, UnityAction<int> onCompleted, int length,
            UnityAction onEnd) where T : struct, IJobParallelFor
        {
            List<JobHandle> handles = new List<JobHandle>();
            foreach (var item in jobData)
            {
                handles.Add(item.Schedule(length, 0));
            }

            YuoAwait_Mono.Instance.StartCoroutine(YuoWaitDForJobEnumerator(handles, onCompleted, onEnd));
        }

        public static IEnumerator YuoWaitDForJobEnumerator(List<JobHandle> handles, UnityAction<int> onCompleted,
            UnityAction onEnd)
        {
            List<int> completedId = new List<int>();
            while (handles.Count > completedId.Count)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    if (!completedId.Contains(i) && handles[i].IsCompleted)
                    {
                        completedId.Add(i);
                        handles[i].Complete();
                        onCompleted?.Invoke(i);
                    }
                }

                yield return null;
            }

            onEnd?.Invoke();
            yield break;
        }
    }
}