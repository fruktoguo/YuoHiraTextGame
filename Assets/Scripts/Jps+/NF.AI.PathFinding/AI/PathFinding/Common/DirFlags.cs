// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.DirFlags
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;
using System.Collections.Generic;

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public static class DirFlags
  {
    private static Dictionary<EDirFlags, Int2> DirToPos = new Dictionary<EDirFlags, Int2>()
    {
      {
        EDirFlags.NORTH,
        new Int2(0, -1)
      },
      {
        EDirFlags.SOUTH,
        new Int2(0, 1)
      },
      {
        EDirFlags.EAST,
        new Int2(1, 0)
      },
      {
        EDirFlags.WEST,
        new Int2(-1, 0)
      },
      {
        EDirFlags.NORTHEAST,
        new Int2(1, -1)
      },
      {
        EDirFlags.NORTHWEST,
        new Int2(-1, -1)
      },
      {
        EDirFlags.SOUTHEAST,
        new Int2(1, 1)
      },
      {
        EDirFlags.SOUTHWEST,
        new Int2(-1, 1)
      }
    };

    public static int ToArrayIndex(EDirFlags dir)
    {
      switch (dir)
      {
        case EDirFlags.NORTH:
          return 1;
        case EDirFlags.SOUTH:
          return 6;
        case EDirFlags.EAST:
          return 4;
        case EDirFlags.WEST:
          return 3;
        case EDirFlags.NORTHEAST:
          return 2;
        case EDirFlags.NORTHWEST:
          return 0;
        case EDirFlags.SOUTHEAST:
          return 7;
        case EDirFlags.SOUTHWEST:
          return 5;
        default:
          return -1;
      }
    }

    public static bool IsStraight(EDirFlags dir)
    {
      return (dir & (EDirFlags.NORTH | EDirFlags.SOUTH | EDirFlags.EAST | EDirFlags.WEST)) != 0;
    }

    public static bool IsDiagonal(EDirFlags dir)
    {
      return (dir & (EDirFlags.NORTHEAST | EDirFlags.NORTHWEST | EDirFlags.SOUTHEAST | EDirFlags.SOUTHWEST)) != 0;
    }

    public static EDirFlags DiagonalToEastWest(EDirFlags dir)
    {
      if ((dir & (EDirFlags.NORTHEAST | EDirFlags.SOUTHEAST)) != 0)
        return EDirFlags.EAST;
      return (dir & (EDirFlags.NORTHWEST | EDirFlags.SOUTHWEST)) != 0 ? EDirFlags.WEST : EDirFlags.NONE;
    }

    public static EDirFlags DiagonalToNorthSouth(EDirFlags dir)
    {
      if ((dir & (EDirFlags.NORTHEAST | EDirFlags.NORTHWEST)) != 0)
        return EDirFlags.NORTH;
      return (dir & (EDirFlags.SOUTHEAST | EDirFlags.SOUTHWEST)) != 0 ? EDirFlags.SOUTH : EDirFlags.NONE;
    }

    public static Int2 ToPos(EDirFlags dir) => DirFlags.DirToPos[dir];
  }
}
