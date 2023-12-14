// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlusRunner
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Mathematics;
using System.Collections.Generic;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlusRunner
  {
    private NF.AI.PathFinding.JPSPlus.JPSPlus mJpsPlus = new NF.AI.PathFinding.JPSPlus.JPSPlus();
    private JPSPlusMapBaker mBaker = new JPSPlusMapBaker();
    private bool[,] mWalls = (bool[,]) null;
    private bool mIsWallChanged = true;
    private Int2? mStartP = new Int2?();
    private Int2? mGoalP = new Int2?();

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Int2 StartP => this.mStartP.Value;

    public Int2 GoalP => this.mGoalP.Value;

    public JPSPlusRunner(int width, int height) => this.Init(new bool[height, width]);

    public JPSPlusRunner(bool[,] walls) => this.Init(walls);

    public void Init(bool[,] walls)
    {
      this.mWalls = walls;
      this.Width = walls.GetLength(1);
      this.Height = walls.GetLength(0);
    }

    public void ToggleWall(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = !this.mWalls[p.Y, p.X];
      this.mIsWallChanged = true;
    }

    public void SetStart(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mStartP = new Int2?(p);
    }

    public void SetGoal(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mGoalP = new Int2?(p);
    }

    public bool IsWalkable(in Int2 p) => this.IsInBoundary(in p) && !this.mWalls[p.Y, p.X];

    public bool StepAll(int stepCount = 2147483647)
    {
      if (this.mIsWallChanged)
      {
        this.mBaker.Init(this.mWalls);
        this.mJpsPlus.Init(this.mBaker.Bake());
        this.mIsWallChanged = false;
      }
      NF.AI.PathFinding.JPSPlus.JPSPlus mJpsPlus1 = this.mJpsPlus;
      Int2 int2 = this.mStartP.Value;
      ref Int2 local1 = ref int2;
      mJpsPlus1.SetStart(in local1);
      NF.AI.PathFinding.JPSPlus.JPSPlus mJpsPlus2 = this.mJpsPlus;
      int2 = this.mGoalP.Value;
      ref Int2 local2 = ref int2;
      mJpsPlus2.SetGoal(in local2);
      return this.mJpsPlus.StepAll(stepCount);
    }

    public void SetWall(in Int2 p, bool isWall)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = isWall;
      this.mIsWallChanged = true;
    }

    public bool[,] GetWalls() => this.mWalls;

    public IReadOnlyList<AStarNode> GetPaths() => this.mJpsPlus.GetPaths();

    private bool IsInBoundary(in Int2 p)
    {
      return 0 <= p.X && p.X < this.Width && 0 <= p.Y && p.Y < this.Height;
    }
  }
}
