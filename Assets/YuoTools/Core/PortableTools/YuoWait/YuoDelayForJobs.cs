using System.Collections;
using System.Collections.Generic;

using Unity.Jobs;

using UnityEngine;
using UnityEngine.Events;

namespace YuoTools.Deprecated
{
    public static class YuoDelayForJobsEx
    {
        public static void YuoSchedule<T>(this T jobData, UnityAction OnCompleted, int arrayLength, int innerloopBatchCount) where T : struct, IJobParallelFor
        {
            YuoAwait_Mono.Instance.StartCoroutine(IYuoDelayDForJob(jobData.Schedule(arrayLength, innerloopBatchCount), OnCompleted));
        }

        public static void YuoSchedule<T>(this T jobData, UnityAction OnCompleted) where T : struct, IJob
        {
            YuoAwait_Mono.Instance.StartCoroutine(IYuoDelayDForJob(jobData.Schedule(), OnCompleted));
        }

        public static IEnumerator IYuoDelayDForJob(JobHandle handle, UnityAction OnCompleted)
        {
            long time = System.DateTime.Now.ToFileTimeUtc();
            while (!handle.IsCompleted)
            {
                yield return null;
            }
            handle.Complete();
            OnCompleted?.Invoke();
            yield break;
        }

        public static void YuoSchedule(this List<JobHandle> jobData, UnityAction<int> OnCompleted, UnityAction OnEnd)
        {
            YuoAwait_Mono.Instance.StartCoroutine(IYuoDelayDForJob(jobData, OnCompleted, OnEnd));
        }

        public static void YuoSchedule<T>(this List<T> jobData, UnityAction<int> OnCompleted, UnityAction OnEnd) where T : struct, IJob
        {
            List<JobHandle> handles = new List<JobHandle>();
            foreach (var item in jobData)
            {
                handles.Add(item.Schedule());
            }
            YuoAwait_Mono.Instance.StartCoroutine(IYuoDelayDForJob(handles, OnCompleted, OnEnd));
        }

        public static void YuoSchedule<T>(this List<T> jobData, UnityAction<int> OnCompleted, int lenth, UnityAction OnEnd) where T : struct, IJobParallelFor
        {
            List<JobHandle> handles = new List<JobHandle>();
            foreach (var item in jobData)
            {
                handles.Add(item.Schedule(lenth, 0));
            }
            YuoAwait_Mono.Instance.StartCoroutine(IYuoDelayDForJob(handles, OnCompleted, OnEnd));
        }

        public static IEnumerator IYuoDelayDForJob(List<JobHandle> handles, UnityAction<int> OnCompleted, UnityAction OnEnd)
        {
            List<int> completedId = new List<int>();
            long time = System.DateTime.Now.ToFileTimeUtc();
            while (handles.Count > completedId.Count)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    if (!completedId.Contains(i) && handles[i].IsCompleted)
                    {
                        completedId.Add(i);
                        handles[i].Complete();
                        OnCompleted?.Invoke(i);
                    }
                }
                yield return null;
            }
            OnEnd?.Invoke();
            Debug.Log($"����ִ��Job{handles.Count}��,�ܼ�����ʱ {(System.DateTime.Now.ToFileTimeUtc() - time) / 10000f} ����");
            yield break;
        }
    }
}