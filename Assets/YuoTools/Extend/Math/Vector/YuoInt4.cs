using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoInt4 : IEquatable<YuoInt4>, IFormattable
    {
        public int x, y, z, w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt4(int a, int b, int c, int d) => (x, y, z, w) = (a, b, c, d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoInt4 other) => x == other.x && y == other.y && z == other.z && w == other.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x},{y},{z},{w})";

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z + w * w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoInt4 a, YuoInt4 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator +(YuoInt4 a, YuoInt4 b) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator -(YuoInt4 a, YuoInt4 b) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator *(YuoInt4 a, int b) => new(a.x * b, a.y * b, a.z * b, a.w * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator /(
            YuoInt4 a, int b) => new(a.x / b, a.y / b, a.z / b, a.w / b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator +(YuoInt4 a, int b) => new(a.x + b, a.y + b, a.z + b, a.w + b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt4 operator -(YuoInt4 a, int b) => new(a.x - b, a.y - b, a.z - b, a.w - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat4(YuoInt4 a) => new(a.x, a.y, a.z, a.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(YuoInt4 a) => new(a.x, a.y, a.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt4(YuoInt3 a) => new(a.x, a.y, a.z, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4(YuoInt4 a) => new(a.x, a.y, a.z, a.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt4(Vector4 a) => new((int)a.x, (int)a.y, (int)a.z, (int)a.w);

        public static readonly YuoInt4
            Right = new(1, 0, 0, 0),
            Left = new(-1, 0, 0, 0),
            Up = new(0, 1, 0, 0),
            Down = new(0, -1, 0, 0),
            Forward = new(0, 0, 1, 0),
            Backward = new(0, 0, -1, 0),
            Zero = new(0, 0, 0, 0),
            One = new(1, 1, 1, 1);
    }
}