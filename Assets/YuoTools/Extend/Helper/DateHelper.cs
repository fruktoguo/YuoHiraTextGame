using System;

namespace YuoTools.Extend.Helper
{
    [Obsolete("Obsolete")]
    public class DateHelper
    {
        public static DateTime Parse(float second)
        {
            TimeSpan span = new TimeSpan((long)second * 10000000);
            DateTime baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime resultTime = baseTime.Add(span);
            return resultTime;
        }

        public static DateTime Parse(double second)
        {
            TimeSpan span = new TimeSpan((long)second * 10000000);
            DateTime baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime resultTime = baseTime.Add(span);
            return resultTime;
        }

        public static DateTime Parse(long second)
        {
            TimeSpan span = new TimeSpan(second * 10000000);
            DateTime baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime resultTime = baseTime.Add(span);
            return resultTime;
        }

        /// <summary>
        ///  转换成生肖时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToChineseHour(DateTime time) => time.Hour switch
        {
            >= 23 =>
                //子时（23 - 1点）：半夜
                "子时",
            < 1 =>
                //子时（23 - 1点）：半夜
                "子时",
            >= 1 and < 3 =>
                //丑时（1 - 3点）：凌晨
                "丑时",
            >= 3 and < 5 =>
                //寅时（3 - 5点）：黎明
                "寅时",
            >= 5 and < 7 =>
                //卯时（5 - 7点）：清晨
                "卯时",
            >= 7 and < 9 =>
                //辰时（7 - 9点）：早上
                "辰时",
            >= 9 and < 11 =>
                //巳时（9 - 11点）：上午
                "巳时",
            >= 11 and < 13 =>
                //午时（11 - 13点）：中午
                "午时",
            >= 13 and < 15 =>
                //未时（13 - 15点）：午后
                "未时",
            >= 15 and < 17 =>
                //申时（15 - 17点）：下午
                "申时",
            >= 17 and < 19 =>
                //酉时（17 - 19点）：傍晚
                "酉时",
            >= 19 and < 21 =>
                //戌时（19 - 21点）：晚上
                "戌时",
            >= 21 and < 23 =>
                //亥时（21 - 23点）：深夜
                "亥时"
        };

        public static bool IsToday(DateTime date)
        {
            DateTime today = DateTime.Now;
            if (today.Year == date.Year && today.Month == date.Month && today.Day == date.Day)
                return true;
            return false;
        }

        public static bool IsYesterday(DateTime date)
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            if (yesterday.Year == date.Year && yesterday.Month == date.Month && yesterday.Day == date.Day)
                return true;
            return false;
        }

        /// <summary>
        /// 当月有多少天
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int MonthDays(int year, int month)
        {
            int days = 1;
            try
            {
                DateTime begin = new DateTime(year, month, 1);
                DateTime end = begin.AddMonths(1);
                days = (int)(end - begin).TotalDays;
            }
            catch
            {
            }

            return days;
        }
    }
}