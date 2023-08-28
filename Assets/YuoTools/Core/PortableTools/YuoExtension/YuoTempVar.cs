using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace YuoTools
{
    public static class Temp
    {
        public static bool Bool;
        public static int Int;
        public static float Float;
        public static StringBuilder StrB = new StringBuilder();

        public static string Str
        {
            get
            {
                return StrB.ToString();
            }
            set
            {
                StrB.Set(value);
            }
        }

        public static AnimationClip AnimaClip;
        public static Vector3 V3;
        public static Vector2 V2;
        public static Vector2Int V2Int;
        public static Vector3Int V3Int;
        public static Color color;
        public static Text text;
        public static Transform tran;
        public static GameObject go;
        public static Dictionary<int, int> a;
    }

    public static class TempVar<T> where T : struct
    {
        private static Dictionary<string, T> values = new Dictionary<string, T>();
        public static T def;

        public static void SetTemp(string key, T t)
        {
            if (!values.ContainsKey(key))
            {
                values.Add(key, t);
            }
            values[key] = t;
        }

        public static T GetTemp(string key)
        {
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            return def;
        }

        public static void Clear()
        {
            values.Clear();
        }
    }

    public static class ExTemp
    {
        public static string GSet(this StringBuilder str, string s)
        {
            str.Clear();
            str.Append(s);
            return str.ToString();
        }

        public static void Set(this StringBuilder str, string s)
        {
            str.Clear();
            str.Append(s);
        }
    }
}