// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlusMapBakerBlock
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Mathematics;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlusMapBakerBlock
  {
    public readonly int[] JumpDistances = new int[8];
    public readonly Int2 Pos;
    public EDirFlags JumpDirFlags = EDirFlags.NONE;

    public JPSPlusMapBakerBlock(in Int2 pos) => this.Pos = pos;

    public bool IsJumpable(EDirFlags dir) => (this.JumpDirFlags & dir) == dir;

    public void SetDistance(EDirFlags dir, int distance)
    {
      this.JumpDistances[DirFlags.ToArrayIndex(dir)] = distance;
    }

    public int GetDistance(EDirFlags dir) => this.JumpDistances[DirFlags.ToArrayIndex(dir)];
  }
}
