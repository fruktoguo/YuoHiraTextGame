using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoFloat4 : IEquatable<YuoFloat4>, IFormattable
    {
        public float x;
        public float y;
        public float z;
        public float w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoFloat4(float x, float y, float z, float w) => (this.x, this.y, this.z, this.w) = (x, y, z, w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoFloat4 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return
                $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)},{z.ToString(format, formatProvider)},{w.ToString(format, formatProvider)})";
        }

        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Sqrt(x * x + y * y + z * z + w * w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoFloat4 a, YuoFloat4 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator +(YuoFloat4 a, YuoFloat4 b) =>
            new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator -(YuoFloat4 a, YuoFloat4 b) =>
            new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator *(YuoFloat4 a, float b) => new(a.x * b, a.y * b, a.z * b, a.w * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator /(YuoFloat4 a, float b) => new(a.x / b, a.y / b, a.z / b, a.w / b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator +(YuoFloat4 a, float b) => new(a.x + b, a.y + b, a.z + b, a.w + b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat4 operator -(YuoFloat4 a, float b) => new(a.x - b, a.y - b, a.z - b, a.w - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt4(YuoFloat4 a) => new((int)a.x, (int)a.y, (int)a.z, (int)a.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(YuoFloat4 a) => new(a.x, a.y, a.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat4(YuoFloat3 a) => new(a.x, a.y, a.z, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4(YuoFloat4 a) => new(a.x, a.y, a.z, a.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat4(Vector4 a) => new(a.x, a.y, a.z, a.w);

        public static readonly YuoFloat4
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