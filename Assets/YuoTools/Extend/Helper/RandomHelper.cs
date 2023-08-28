using System;
using System.Collections.Generic;

namespace YuoTools.Extend.Helper
{
    public static class RandomHelper
    {
        private static readonly Random random = new(Guid.NewGuid().GetHashCode());

        private static readonly byte[] Byte8 = new byte[8];

        public static ulong RandUInt64()
        {
            random.NextBytes(Byte8);
            return BitConverter.ToUInt64(Byte8, 0);
        }

        public static int RandInt32()
        {
            return random.Next();
        }

        public static uint RandUInt32()
        {
            return (uint) random.Next();
        }

        public static long RandInt64()
        {
            random.NextBytes(Byte8);
            return BitConverter.ToInt64(Byte8, 0);
        }

        /// <summary>
        ///     获取lower与Upper之间的随机数,包含下限，不包含上限
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int RandomNumber(int lower, int upper)
        {
            var value = random.Next(lower, upper);
            return value;
        }

        public static long NextLong(long minValue, long maxValue)
        {
            if (minValue > maxValue) throw new ArgumentException("minValue is great than maxValue", nameof(minValue));

            var num = maxValue - minValue;
            return minValue + (long) (random.NextDouble() * num);
        }

        public static bool RandomBool()
        {
            return random.Next(2) == 0;
        }

        public static T RandomArray<T>(this T[] array)
        {
            return array[RandomNumber(0, array.Length)];
        }

        public static T RandomArray<T>(this List<T> array)
        {
            return array[RandomNumber(0, array.Count)];
        }

        /// <summary>
        ///     打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要打乱的数组</param>
        public static void BreakRank<T>(this List<T> arr)
        {
            if (arr == null || arr.Count < 2) return;

            for (var i = 0; i < arr.Count; i++)
            {
                var index = random.Next(0, arr.Count);
                (arr[index], arr[i]) = (arr[i], arr[index]);
            }
        }

        public static float RandFloat01()
        {
            var a = RandomNumber(0, 1000000);
            return a / 1000000f;
        }
    }
}