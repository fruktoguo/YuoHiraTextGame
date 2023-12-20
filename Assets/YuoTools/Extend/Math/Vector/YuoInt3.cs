using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoInt3 : IEquatable<YuoInt3>, IFormattable
    {
        public int x, y, z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt3(int x, int y, int z) => (this.x, this.y, this.z) = (x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoInt3 other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)},{z.ToString(format, formatProvider)})";

        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Sqrt(x * x + y * y + z * z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoInt3 a, YuoInt3 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator +(YuoInt3 a, YuoInt3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator -(YuoInt3 a, YuoInt3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator *(YuoInt3 a, int b) => new(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator /(YuoInt3 a, int b) => new(a.x / b, a.y / b, a.z / b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator +(YuoInt3 a, int b) => new(a.x + b, a.y + b, a.z + b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator -(YuoInt3 a, int b) => new(a.x - b, a.y - b, a.z - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(YuoInt3 a) => new(a.x, a.y, a.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt2(YuoInt3 a) => new(a.x, a.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(YuoInt2 a) => new(a.x, a.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(YuoInt3 a) => new(a.x, a.y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(Vector3 a) => new((int)a.x, (int)a.y, (int)a.z);

        public static readonly YuoInt3
            Right = new(1, 0, 0),
            Left = new(-1, 0, 0),
            Up = new(0, 1, 0),
            Down = new(0, -1, 0),
            Forward = new(0, 0, 1),
            Backward = new(0, 0, -1),
            Zero = new(0, 0, 0),
            One = new(1, 1, 1);
    }
}