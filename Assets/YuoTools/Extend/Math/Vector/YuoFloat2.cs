using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoFloat2 : IEquatable<YuoFloat2>, IFormattable
    {
        public float x, y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoFloat2(float x, float y) => (this.x, this.y) = (x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoFloat2 other) => x.Equals(other.x) && y.Equals(other.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is YuoFloat2 other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)})";

        public float Magnitude => Mathf.Sqrt(x * x + y * y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoFloat2 a, YuoFloat2 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(YuoFloat2 a, YuoFloat2 b) =>
            !a.x.ApEqual(b.x, 0.00001f) || !a.y.ApEqual(b.y, 0.00001f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(YuoFloat2 a, YuoFloat2 b) =>
            a.x.ApEqual(b.x, 0.00001f) && a.y.ApEqual(b.y, 0.00001f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator +(YuoFloat2 a, YuoFloat2 b) => new(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator -(YuoFloat2 a, YuoFloat2 b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator *(YuoFloat2 a, float b) => new(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator /(YuoFloat2 a, float b) => new(a.x / b, a.y / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator +(YuoFloat2 a, float b) => new(a.x + b, a.y + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat2 operator -(YuoFloat2 a, float b) => new(a.x - b, a.y - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt2(YuoFloat2 a) => new((int)a.x, (int)a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(YuoFloat2 a) => new(a.x, a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(YuoFloat2 a) => new(a.x, a.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat2(Vector2 a) => new(a.x, a.y);

        public static readonly YuoFloat2 
            Right = new(1, 0),
            Left = new(-1, 0),
            Up = new(0, 1),
            Down = new(0, -1),
            Zero = new(0, 0),
            One = new(1, 1);
    }
}