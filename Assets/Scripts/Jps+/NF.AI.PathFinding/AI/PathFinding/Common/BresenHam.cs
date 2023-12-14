// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.BresenHam
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;
using System;

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public class BresenHam
  {
    private int mDx;
    private int mSx;
    private int mDy;
    private int mSy;
    private int mErr;
    private Int2 mCurr;
    private Int2 mDest;

    public void Init(in Int2 src, in Int2 dst)
    {
      this.mDx = Math.Abs(dst.X - src.X);
      this.mDy = -Math.Abs(dst.Y - src.Y);
      this.mSx = src.X < dst.X ? 1 : -1;
      this.mSy = src.Y < dst.Y ? 1 : -1;
      this.mErr = this.mDx + this.mDy;
      this.mCurr = src;
      this.mDest = dst;
    }

    public bool TryGetNext(ref Int2 nextP)
    {
      if (this.mCurr == this.mDest)
        return false;
      int num = 2 * this.mErr;
      if (num >= this.mDy)
      {
        this.mErr += this.mDy;
        this.mCurr.X += this.mSx;
      }
      if (num <= this.mDx)
      {
        this.mErr += this.mDx;
        this.mCurr.Y += this.mSy;
      }
      nextP = this.mCurr;
      return true;
    }
  }
}
