// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.EDirFlags
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public enum EDirFlags
  {
    NONE = 0,
    NORTH = 1,
    SOUTH = 2,
    EAST = 4,
    WEST = 8,
    NORTHEAST = 16, // 0x00000010
    NORTHWEST = 32, // 0x00000020
    SOUTHEAST = 64, // 0x00000040
    SOUTHWEST = 128, // 0x00000080
    ALL = 255, // 0x000000FF
  }
}
