// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.ExInt2
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public static class ExInt2
  {
    public static Int2 Foward(this Int2 x, EDirFlags dir) => x + DirFlags.ToPos(dir);

    public static Int2 Backward(this Int2 x, EDirFlags dir) => x - DirFlags.ToPos(dir);
  }
}
