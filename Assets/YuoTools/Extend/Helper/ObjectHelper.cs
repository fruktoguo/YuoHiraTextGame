﻿namespace YuoTools.Extend.Helper
{
    public static class ObjectHelper
    {
        public static void Swap<T>(ref T t1, ref T t2)
        {
            (t1, t2) = (t2, t1);
        }
    }
}