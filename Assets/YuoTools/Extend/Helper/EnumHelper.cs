using System;

namespace YuoTools.Extend.Helper
{
    public static class EnumHelper
    {
        public static int EnumIndex<T>(int value)
        {
            var i = 0;
            foreach (var v in Enum.GetValues(typeof(T)))
            {
                if ((int) v == value) return i;

                ++i;
            }

            return -1;
        }

        public static T FromString<T>(string str)
        {
            if (!Enum.IsDefined(typeof(T), str)) return default;

            return (T) Enum.Parse(typeof(T), str);
        }
        
        public static T[] GetValues<T>()
        {
            return (T[]) Enum.GetValues(typeof(T));
        }
    }
}