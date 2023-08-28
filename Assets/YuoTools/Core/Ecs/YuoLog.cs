using System;

namespace YuoTools.Main.Ecs
{
    public static class YuoLog
    {
        /// <summary>
        /// 关闭后所有debug失效
        /// </summary>
        static bool openDebug = true;

        static bool showTime = false;
        public static bool IsEditor = true;

        public static void Open(LogComponent log)
        {
            logComponent = log;
            openDebug = true;
        }

        public static void Close()
        {
            openDebug = false;
            logComponent = null;
        }

        private static string mergeLog;

        public static void MergeLog<T>(T obj)
        {
            if (!openDebug) return;
            mergeLog += obj;
        }

        public static void MergeLogOutput()
        {
            if (!openDebug) return;
            mergeLog.Log();
            mergeLog = "";
        }

        static LogComponent logComponent;

        public abstract class LogComponent
        {
            public abstract T Log<T>(T msg);
            public abstract T Error<T>(T msg);
        }

        internal static T Log<T>(this T obj)
        {
            if (!openDebug) return obj;
            if (IsEditor)
            {
                UnityEngine.Debug.Log((showTime
                    ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                    : "") + obj);
                return obj;
            }

            logComponent?.Log(obj);
            return obj;
        }

        internal static T LogError<T>(this T obj)
        {
            if (!openDebug) return obj;
            if (IsEditor)
            {
                UnityEngine.Debug.LogError((showTime
                    ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                    : "") + obj);
                return obj;
            }

            logComponent?.Error(obj);
            return obj;
        }
    }
}