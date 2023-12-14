// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.BresenHamPathSmoother
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;
using System.Collections.Generic;

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public class BresenHamPathSmoother
  {
    private BresenHam mBresenHam = new BresenHam();

    public List<Int2> SmoothPath(List<Int2> path, BresenHamPathSmoother.DelIsWalkable fnIsWalkable)
    {
      List<Int2> int2List = new List<Int2>();
      if (path.Count < 2)
        return int2List;
      Int2 p = new Int2();
      Int2 int2 = new Int2();
      Int2 src = path[0];
      Int2 dst = path[1];
      int index = 1;
      this.mBresenHam.Init(in src, in dst);
      int2List.Add(src);
      while (true)
      {
        if (!this.mBresenHam.TryGetNext(ref p))
        {
          int2 = dst;
          ++index;
          if (index != path.Count)
          {
            dst = path[index];
            this.mBresenHam.Init(in src, in dst);
          }
          else
            break;
        }
        else if (!fnIsWalkable(in p))
        {
          int2List.Add(int2);
          src = int2;
          this.mBresenHam.Init(in src, in dst);
        }
      }
      int2List.Add(dst);
      return int2List;
    }

    public delegate bool DelIsWalkable(in Int2 p);
  }
}
