using System;
using UnityEngine;

namespace YuoTools.Extend.YuoMathf
{
    [Serializable]
    public struct YuoVector2Int
    {
        public int x;
        public int y;

        public YuoVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(YuoVector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is YuoVector2Int other && Equals(other);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public float Magnitude() => Mathf.Sqrt(x * x + y * y);

        public static float Distance(YuoVector2Int a, YuoVector2Int b)
        {
            return (a - b).Magnitude();
        }

        public static bool operator !=(YuoVector2Int a, YuoVector2Int b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(YuoVector2Int a, YuoVector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static YuoVector2Int operator +(YuoVector2Int a, YuoVector2Int b)
        {
            return new YuoVector2Int(a.x + b.x, a.y + b.y);
        }

        public static YuoVector2Int operator -(YuoVector2Int a, YuoVector2Int b)
        {
            return new YuoVector2Int(a.x - b.x, a.y - b.y);
        }

        public static YuoVector2Int operator *(YuoVector2Int a, int b)
        {
            return new YuoVector2Int(a.x * b, a.y * b);
        }

        public static YuoVector2Int operator /(YuoVector2Int a, int b)
        {
            return new YuoVector2Int(a.x / b, a.y / b);
        }

        public YuoVector2Int Set(int x, int y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public YuoVector2Int Add(int x, int y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        public static implicit operator YuoVector2Int(Vector2 value) => new((int)value.x, (int)value.y);

        public static implicit operator Vector2(YuoVector2Int value) => new(value.x, value.y);
    }

    public struct YuoVector2
    {
        public bool Equals(YuoVector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj)
        {
            return obj is YuoVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public float x;
        public float y;

        public YuoVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y);

        public static float Distance(YuoVector2 a, YuoVector2 b)
        {
            return (a - b).Magnitude;
        }

        public static bool operator !=(YuoVector2 a, YuoVector2 b)
        {
            return !a.x.ApEqual(b.x, 0.00001f) || !a.y.ApEqual(b.y, 0.00001f);
        }

        public static bool operator ==(YuoVector2 a, YuoVector2 b)
        {
            return a.x.ApEqual(b.x, 0.00001f) && a.y.ApEqual(b.y, 0.00001f);
        }

        public static YuoVector2 operator +(YuoVector2 a, YuoVector2 b)
        {
            return new YuoVector2(a.x + b.x, a.y + b.y);
        }

        public static YuoVector2 operator -(YuoVector2 a, YuoVector2 b)
        {
            return new YuoVector2(a.x - b.x, a.y - b.y);
        }

        public static YuoVector2 operator *(YuoVector2 a, float b)
        {
            return new YuoVector2(a.x * b, a.y * b);
        }

        public static YuoVector2 operator /(YuoVector2 a, float b)
        {
            return new YuoVector2(a.x / b, a.y / b);
        }

        public static implicit operator YuoVector2Int(YuoVector2 a) => new((int)a.x, (int)a.y);

        public static implicit operator Vector2(YuoVector2 a) => new(a.x, a.y);

        public static implicit operator YuoVector2(Vector2 a) => new(a.x, a.y);
    }

    public struct YuoVector3
    {
        public float x;
        public float y;
        public float z;

        public YuoVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        public static float Distance(YuoVector3 a, YuoVector3 b)
        {
            return (a - b).Magnitude;
        }

        public static YuoVector3 operator +(YuoVector3 a, YuoVector3 b)
        {
            return new YuoVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static YuoVector3 operator -(YuoVector3 a, YuoVector3 b)
        {
            return new YuoVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static YuoVector3 operator *(YuoVector3 a, float b)
        {
            return new YuoVector3(a.x * b, a.y * b, a.z * b);
        }

        public static YuoVector3 operator /(YuoVector3 a, float b)
        {
            return new YuoVector3(a.x / b, a.y / b, a.z / b);
        }

        public static implicit operator YuoVector3Int(YuoVector3 a) => new((int)a.x, (int)a.y, (int)a.z);

        public static implicit operator Vector3(YuoVector3 a) => new(a.x, a.y, a.z);

        public static implicit operator YuoVector3(Vector3 a) => new(a.x, a.y, a.z);
    }

    public struct YuoVector3Int
    {
        public int x;
        public int y;
        public int z;

        public YuoVector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        public static float Distance(YuoVector3Int a, YuoVector3Int b)
        {
            return (a - b).Magnitude;
        }

        public static YuoVector3Int operator +(YuoVector3Int a, YuoVector3Int b)
        {
            return new YuoVector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static YuoVector3Int operator -(YuoVector3Int a, YuoVector3Int b)
        {
            return new YuoVector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static YuoVector3Int operator *(YuoVector3Int a, int b)
        {
            return new YuoVector3Int(a.x * b, a.y * b, a.z * b);
        }

        public static YuoVector3Int operator /(YuoVector3Int a, int b)
        {
            return new YuoVector3Int(a.x / b, a.y / b, a.z / b);
        }

        public static implicit operator YuoVector3(YuoVector3Int a) => new(a.x, a.y, a.z);

        public static implicit operator Vector3(YuoVector3Int a) => new(a.x, a.y, a.z);

        public static implicit operator YuoVector3Int(Vector3 a) => new((int)a.x, (int)a.y, (int)a.z);
    }

    public struct YuoVector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public YuoVector4(float x, float y, float z, float w) => (this.x, this.y, this.z, this.w) = (x, y, z, w);

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z + w * w);

        public static float Distance(YuoVector4 a, YuoVector4 b) => (a - b).Magnitude;

        public static YuoVector4 operator +(YuoVector4 a, YuoVector4 b) =>
            new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        public static YuoVector4 operator -(YuoVector4 a, YuoVector4 b) =>
            new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        public static YuoVector4 operator *(YuoVector4 a, float b) => new(a.x * b, a.y * b, a.z * b, a.w * b);

        public static YuoVector4 operator /(YuoVector4 a, float b) => new(a.x / b, a.y / b, a.z / b, a.w / b);

        public static implicit operator YuoVector4Int(YuoVector4 a) => new((int)a.x, (int)a.y, (int)a.z, (int)a.w);

        public static implicit operator Vector4(YuoVector4 a) => new(a.x, a.y, a.z, a.w);

        public static implicit operator YuoVector4(Vector4 a) => new(a.x, a.y, a.z, a.w);
    }

    public struct YuoVector4Int
    {
        public int x;
        public int y;
        public int z;
        public int w;

        public YuoVector4Int(int x, int y, int z, int w) => (this.x, this.y, this.z, this.w) = (x, y, z, w);

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z + w * w);

        public static float Distance(YuoVector4Int a, YuoVector4Int b) => (a - b).Magnitude;

        public static YuoVector4Int operator +(YuoVector4Int a, YuoVector4Int b) =>
            new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        public static YuoVector4Int operator -(YuoVector4Int a, YuoVector4Int b) =>
            new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        public static YuoVector4Int operator *(YuoVector4Int a, int b) => new(a.x * b, a.y * b, a.z * b, a.w * b);

        public static YuoVector4Int operator /(YuoVector4Int a, int b) => new(a.x / b, a.y / b, a.z / b, a.w / b);

        public static implicit operator YuoVector4(YuoVector4Int a) => new(a.x, a.y, a.z, a.w);

        public static implicit operator Vector4(YuoVector4Int a) => new(a.x, a.y, a.z, a.w);
        
        public static implicit operator YuoVector4Int(Vector4 a) => new((int)a.x, (int)a.y, (int)a.z, (int)a.w);
    }
}