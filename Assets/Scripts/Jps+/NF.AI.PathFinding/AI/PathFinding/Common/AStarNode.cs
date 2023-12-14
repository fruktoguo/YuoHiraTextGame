// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.Common.AStarNode
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.Mathematics;

#nullable disable
namespace NF.AI.PathFinding.Common
{
  public class AStarNode
  {
    public Int2 Position { get; private set; }

    public int G { get; internal set; } = 0;

    public int H { get; internal set; } = 0;

    public long F => (long) (this.G + this.H);

    public AStarNode Parent { get; internal set; }

    public AStarNode(in Int2 p)
    {
      this.G = 0;
      this.H = 0;
      this.Position = p;
    }

    public AStarNode(int x, int y)
      : this(new Int2() { X = x, Y = y })
    {
    }

    public void Refresh()
    {
      this.G = 0;
      this.H = 0;
      this.Parent = (AStarNode) null;
    }
  }
}
