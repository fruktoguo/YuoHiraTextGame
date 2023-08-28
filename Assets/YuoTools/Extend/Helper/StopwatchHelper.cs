using System.Diagnostics;

namespace YuoTools.Extend.Helper
{
    public class StopwatchHelper
    {
        private static readonly Stopwatch StopWatch = new();

        public static void Start()
        {
            StopWatch.Restart();
        }
        
        /// <summary>
        ///  停止并返回毫秒数
        /// </summary>
        /// <returns></returns>
        public static double Stop()
        {
            StopWatch.Stop();
            return StopWatch.Elapsed.TotalMilliseconds;
        }
    }
}