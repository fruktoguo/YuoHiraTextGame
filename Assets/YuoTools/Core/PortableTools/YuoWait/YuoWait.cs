using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ET;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuoTools
{
    public class YuoWait
    {
        private static Dictionary<int, WaitForSeconds> waits = new Dictionary<int, WaitForSeconds>();

        /// <summary>
        /// 返回一个WaitForSeconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSeconds WaitTime(float time = 1)
        {
            Temp.Int = (int)(time * 1000);
            if (!waits.ContainsKey(Temp.Int))
            {
                waits.Add(Temp.Int, new WaitForSeconds(time));
            }

            return waits[Temp.Int];
        }

        private static Dictionary<int, WaitForSecondsRealtime> waitsRealtime =
            new Dictionary<int, WaitForSecondsRealtime>();

        /// <summary>
        /// 返回一个不受TimeScale影响的WaitForSeconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSecondsRealtime WaitUnscaledTime(float time = 1)
        {
            Temp.Int = (int)(time * 1000);
            if (!waitsRealtime.ContainsKey(Temp.Int))
            {
                waitsRealtime.Add(Temp.Int, new WaitForSecondsRealtime(time));
            }

            return waitsRealtime[Temp.Int];
        }

        /// <summary>
        /// 用于await延迟---带缩放
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static async ETTask WaitTimeAsync(float waitTime)
        {
            await YuoAwait_Mono.Instance.WaitTimeAsync(waitTime);
        }

        public static async ETTask WaitFrameAsync(int frame = 1)
        {
            await YuoAwait_Mono.Instance.WaitFrameAsync(frame);
        }
        

        /// <summary>
        /// 用于await延迟--->无缩放
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static async ETTask WaitUnscaledTimeAsync(float waitTime)
        {
            await YuoAwait_Mono.Instance.WaitUnscaledTimeAsync(waitTime);
        }

        public static ETTask<T> ResourcesLoadAsync<T>(string path) where T : Object
        {
            return YuoAwait_Mono.Instance.ResourcesLoadAsync<T>(path);
        }
    }
}