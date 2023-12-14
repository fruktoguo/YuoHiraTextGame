// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlusBakedMap
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlusBakedMap
  {
    public readonly int[,] BlockLUT;
    public readonly JPSPlusBakedMap.JPSPlusBakedMapBlock[] Blocks;

    public int Width => this.BlockLUT.GetLength(1);

    public int Height => this.BlockLUT.GetLength(0);

    public JPSPlusBakedMap(int[,] blockLUT, JPSPlusBakedMap.JPSPlusBakedMapBlock[] blocks)
    {
      this.BlockLUT = blockLUT;
      this.Blocks = blocks;
    }

    public class JPSPlusBakedMapBlock
    {
      public readonly int[] JumpDistances;
      public readonly Int2 Pos;

      public JPSPlusBakedMapBlock(in Int2 pos, int[] jumpDistances)
      {
        this.JumpDistances = jumpDistances;
        this.Pos = pos;
      }
    }
  }
}
