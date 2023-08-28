using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public static class YuoLog
    {
        /// <summary>
        /// 关闭后所有debug失效
        /// </summary>
        public static bool OpenDebug = true;

        public static bool ShowTime = false;

        public const string ExtensionTagContains = "[yuolog]";
        public const string ExtensionTag = "[YuoLog]";

        static string _mergeLog = "";

        private static string ColorRGBTo16(this Color color)
        {
            return $"#{color.r.ColorFloat10To16()}{color.g.ColorFloat10To16()}{color.b.ColorFloat10To16()}";
        }

        private static string ColorFloat10To16(this float f)
        {
            if (f * 255 <= 16)
                return $"0{System.Convert.ToString(Mathf.Clamp((int)(f * 255), 0, 255), 16)}";
            else
                return System.Convert.ToString(Mathf.Clamp((int)(f * 255), 0, 255), 16);
        }

        private static string ColorInt10To16(this int i)
        {
            if (i * 255 <= 16)
                return $"0{System.Convert.ToString(i, 16)}";
            else
                return System.Convert.ToString(i, 16);
        }

        /// <summary>
        /// 组合Log,不输出,输出请调用MergeLogOutput
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T MergeLog<T>(this T obj)
        {
            if (!OpenDebug) return obj;
            _mergeLog += obj;
            return obj;
        }

        /// <summary>
        /// 组合Log输出
        /// </summary>
        public static void MergeLogOutput()
        {
            if (!OpenDebug) return;
            _mergeLog.Log();
            _mergeLog = "";
        }

        public static T Log<T>(this T obj)
        {
            if (!OpenDebug) return obj;
            Debug.Log(ShowTime
                ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                : "" + obj);
            return obj;
        }

        public static T LogError<T>(this T obj)
        {
            if (!OpenDebug) return obj;
            Debug.LogError(ShowTime
                ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                : "" + obj);
            return obj;
        }

        public static T Log<T>(this T obj, string Color)
        {
            if (!OpenDebug) return obj;
            Debug.Log(ShowTime
                ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                : "" +
                  $"{obj.ToString().LogSetBold().LogSetColor(Color)}");
            return obj;
        }

        public static List<T> LogAll<T>(this List<T> objs)
        {
            if (!OpenDebug) return objs;
            $"该 {typeof(T).Name.LogSetColor(YuoColorText.钢蓝)} 集合 的长度为 [{objs.Count.ToString().LogSetColor(YuoColorText.深粉色)}]"
                .Log();
            for (int i = 0; i < objs.Count; i++)
            {
                $"第 [{(i + 1).ToString().LogSetColor(YuoColorText.深粉色)}] 个元素为 {objs[i].ToString().LogSetColor(YuoColorText.紫罗兰红色)}"
                    .Log();
            }

            return objs;
        }

        public static T[] LogAll<T>(this T[] objs)
        {
            if (!OpenDebug) return objs;
            $"该 {typeof(T).ToString().LogSetColor(YuoColorText.钢蓝)} 数组 的长度为 [{objs.Length.ToString().LogSetColor(YuoColorText.深粉色)}]"
                .Log();
            for (int i = 0; i < objs.Length; i++)
            {
                $"第 [{(i + 1).ToString().LogSetColor(YuoColorText.深粉色)}] 个元素为 {objs[i].ToString().LogSetColor(YuoColorText.耐火砖)}"
                    .Log();
            }

            return objs;
        }

        public static Dictionary<T1, T2> LogAll<T1, T2>(this Dictionary<T1, T2> objs)
        {
            if (!OpenDebug) return objs;
            $"该 【Key :{typeof(T1).ToString().LogSetColor(YuoColorText.钢蓝)} , Value : {typeof(T2).ToString().LogSetColor(YuoColorText.钢蓝)} 】 字典 共有[{objs.Count.ToString().LogSetColor(YuoColorText.深粉色)}] 个元素 "
                .Log();
            int i = 0;
            foreach (var item in objs)
            {
                $"第 [{(i + 1).ToString().LogSetColor(YuoColorText.深粉色)}] 个元素为 {item.ToString().LogSetColor(YuoColorText.耐火砖)}"
                    .Log();
                i++;
            }

            return objs;
        }

        /// <summary>
        /// 更改控制台输出字体的颜色,请使用YuoColor
        /// </summary>
        /// <param name="color">请使用YuoColor</param>
        /// <returns></returns>
        public static string LogSetColor(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }

        public static string LogSetColor(this string str, Color color)
        {
            return $"<color={color.ColorRGBTo16()}>{str}</color>";
        }

        public static string LogSetBold(this string str)
        {
            return $"<b>{str}</b>";
        }

        public static string LogSetLtalic(this string str)
        {
            return $"<i>{str}</i>";
        }

        public static string LogSetSize(this string str, int size)
        {
            return $"<size={size}>{str}</size>";
        }

        public static string LogSetAlpha(this string str, int alpha)
        {
            alpha = Mathf.Clamp(alpha, 0, 255);
            return $"{str}{alpha.ColorInt10To16()}";
        }
    }
}