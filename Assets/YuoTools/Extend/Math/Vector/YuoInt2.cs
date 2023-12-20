using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    [Serializable]
    public struct YuoInt2 : IEquatable<YuoInt2>, IFormattable
    {
        public int x;
        public int y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoInt2 other) => x == other.x && y == other.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is YuoInt2 other && Equals(other);

        public override string ToString() => $"({x},{y})";

        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)})";

        public override int GetHashCode() => HashCode.Combine(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => Mathf.Sqrt(x * x + y * y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SqrMagnitude() => x * x + y * y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoInt2 a, YuoInt2 b) => (a - b).Magnitude();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistance(YuoInt2 a, YuoInt2 b) => (a - b).SqrMagnitude();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(YuoInt2 a, YuoInt2 b) => a.x != b.x || a.y != b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(YuoInt2 a, YuoInt2 b) => a.x == b.x && a.y == b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator +(YuoInt2 a, YuoInt2 b) => new(a.x + b.x, a.y + b.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator -(YuoInt2 a, YuoInt2 b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator +(YuoInt2 a, int b) => new(a.x + b, a.y + b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator -(YuoInt2 a, int b) => new(a.x - b, a.y - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator *(YuoInt2 a, int b) => new(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt2 operator /(YuoInt2 a, int b) => new(a.x / b, a.y / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt2 Set(int x, int y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt2 Add(int x, int y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt2(Vector2 value) => new((int)value.x, (int)value.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(YuoInt2 value) => new(value.x, value.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt2(Vector2Int value) => new(value.x, value.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Int(YuoInt2 value) => new(value.x, value.y);

        public static readonly YuoFloat2 Right = new(1, 0),
            Left = new(-1, 0),
            Up = new(0, 1),
            Down = new(0, -1),
            Zero = new(0, 0),
            One = new(1, 1);
    }
}