// Decompiled with JetBrains decompiler
// Type: NF.Mathematics.Int2
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable disable
namespace NF.Mathematics
{
  [Serializable]
  public struct Int2 : IEquatable<Int2>, IFormattable
  {
    public int X;
    public int Y;

    public Int2(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Int2 XY
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Int2(this.X, this.Y);
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Int2 YX
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Int2(this.Y, this.X);
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set
      {
        this.Y = value.X;
        this.X = value.Y;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator +(Int2 a, Int2 b)
    {
      return new Int2() { X = a.X + b.X, Y = a.Y + b.Y };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator -(Int2 a, Int2 b)
    {
      return new Int2() { X = a.X - b.X, Y = a.Y - b.Y };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator *(Int2 a, int val)
    {
      return new Int2() { X = a.X * val, Y = a.Y * val };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 operator /(Int2 a, int val)
    {
      return new Int2() { X = a.X / val, Y = a.Y / val };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Int2 a, Int2 b) => a.X == b.X && a.Y == b.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Int2 a, Int2 b) => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) => this.Equals((Int2) obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => base.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
      return string.Format("Int2({0}, {1})", (object) this.X, (object) this.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Int2 other) => this == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string format, IFormatProvider formatProvider)
    {
      return "Int2(" + this.X.ToString(format, formatProvider) + ", " + this.Y.ToString(format, formatProvider) + ")";
    }
  }
}
