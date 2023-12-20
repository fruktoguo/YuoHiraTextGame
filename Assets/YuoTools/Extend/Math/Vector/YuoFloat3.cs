using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoFloat3 : IEquatable<YuoFloat3>, IFormattable
    {
        public float x, y, z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoFloat3(float x, float y, float z) => (this.x, this.y, this.z) = (x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoFloat3 other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)},{z.ToString(format, formatProvider)})";

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoFloat3 a, YuoFloat3 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator +(YuoFloat3 a, YuoFloat3 b) => new YuoFloat3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator -(YuoFloat3 a, YuoFloat3 b) => new YuoFloat3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator *(YuoFloat3 a, float b) => new YuoFloat3(a.x * b, a.y * b, a.z * b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator /(YuoFloat3 a, float b) => new YuoFloat3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator +(YuoFloat3 a, float b) => new YuoFloat3(a.x + b, a.y + b, a.z + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator -(YuoFloat3 a, float b) => new YuoFloat3(a.x - b, a.y - b, a.z - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(YuoFloat3 a) => new((int)a.x, (int)a.y, (int)a.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat2(YuoFloat3 a) => new(a.x, a.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(YuoFloat2 a) => new(a.x, a.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(YuoFloat3 a) => new(a.x, a.y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(Vector3 a) => new(a.x, a.y, a.z);

        public static readonly YuoFloat3 
            Right = new(1, 0, 0),
            Left = new(-1, 0, 0),
            Up = new(0, 1, 0),
            Down = new(0, -1, 0),
            Forward = new(0, 0, 1),
            Backward = new(0, 0, -1),
            Zero = new(0, 0, 0);
    }
}