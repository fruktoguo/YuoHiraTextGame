using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public static class YuoComponentEx
    {
        /// <summary>
        /// 自动GetComponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="mono"></param>
        /// <returns></returns>
        public static T YuoGet<T>(this T component, MonoBehaviour mono) where T : Component
        {
            return component ? component : mono.GetComponent<T>();
        }
    }
}