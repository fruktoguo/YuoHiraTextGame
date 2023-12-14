// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlusNode
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Mathematics;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlusNode : AStarNode
  {
    private int[] mJumpDistances;

    public JPSPlusNode(in Int2 p, int[] jumpDistances)
      : base(in p)
    {
      this.mJumpDistances = jumpDistances;
    }

    public int GetDistance(EDirFlags dir) => this.mJumpDistances[DirFlags.ToArrayIndex(dir)];

    internal void Refresh(int[] jumpDistances)
    {
      this.mJumpDistances = jumpDistances;
      this.Refresh();
    }
  }
}
