// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSOrthogonal.JPSOrthogonal
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Collections.Generic;
using NF.Mathematics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace NF.AI.PathFinding.JPSOrthogonal
{
  public class JPSOrthogonal
  {
    private AStarNode mStart = (AStarNode) null;
    private AStarNode mGoal = (AStarNode) null;
    private readonly Dictionary<Int2, AStarNode> mCreateNodes = new Dictionary<Int2, AStarNode>();
    private readonly PriorityQueue<(AStarNode AStarNode, EDirFlags Dir)> mOpenList = new PriorityQueue<(AStarNode, EDirFlags)>();
    private readonly HashSet<AStarNode> mCloseList = new HashSet<AStarNode>();
    private bool[,] mWalls = (bool[,]) null;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Int2 StartP => this.mStart.Position;

    public Int2 GoalP => this.mGoal.Position;

    public JPSOrthogonal()
    {
    }

    public JPSOrthogonal(int width, int height) => this.Init(new bool[height, width]);

    public JPSOrthogonal(bool[,] walls) => this.Init(walls);

    public void Init(bool[,] walls)
    {
      this.Width = walls.GetLength(1);
      this.Height = walls.GetLength(0);
      this.mWalls = walls;
    }

    public bool StepAll(int stepCount = 2147483647)
    {
      this.mOpenList.Clear();
      this.mCloseList.Clear();
      foreach (AStarNode astarNode in this.mCreateNodes.Values)
        astarNode.Refresh();
      this.mOpenList.Enqueue((this.mStart, EDirFlags.ALL), this.mStart.F);
      return this.Step(stepCount);
    }

    public bool Step(int stepCount)
    {
      int num1 = stepCount;
      Int2 p = new Int2(0, 0);
      while (true)
      {
        if (num1 > 0 && this.mOpenList.Count != 0)
        {
          (AStarNode astarNode, EDirFlags edirFlags1) = this.mOpenList.Dequeue();
          this.mCloseList.Add(astarNode);
          Int2 position = astarNode.Position;
          Int2 goal = this.mGoal.Position;
          if (!(position == goal))
          {
            EDirFlags edirFlags2 = this.SuccesorsDir(in position, edirFlags1);
            for (int index = 128; index > 0; index >>= 1)
            {
              EDirFlags dir = (EDirFlags) index;
              if ((dir & edirFlags2) != EDirFlags.NONE && this.TryJump(in position, dir, in goal, ref p))
              {
                AStarNode node = this.GetOrCreateNode(in p);
                if (!this.mCloseList.Contains(node))
                {
                  int num2 = NF.AI.PathFinding.JPSOrthogonal.JPSOrthogonal.G(astarNode, node);
                  (AStarNode, EDirFlags) valueTuple = (node, dir);
                  if (!this.mOpenList.Contains(valueTuple))
                  {
                    node.Parent = astarNode;
                    node.G = num2;
                    node.H = NF.AI.PathFinding.JPSOrthogonal.JPSOrthogonal.H(node, this.mGoal);
                    this.mOpenList.Enqueue(valueTuple, node.F);
                  }
                  else if (num2 < node.G)
                  {
                    node.Parent = astarNode;
                    node.G = num2;
                    node.H = NF.AI.PathFinding.JPSOrthogonal.JPSOrthogonal.H(node, this.mGoal);
                    this.mOpenList.UpdatePriority(valueTuple, node.F);
                  }
                }
              }
            }
            --num1;
          }
          else
            goto label_3;
        }
        else
          break;
      }
      return false;
label_3:
      return true;
    }

    public bool SetStart(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return false;
      this.mStart = this.GetOrCreateNode(in p);
      return true;
    }

    public bool SetGoal(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return false;
      this.mGoal = this.GetOrCreateNode(in p);
      return true;
    }

    public void SetWall(in Int2 p, bool isWall)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = isWall;
    }

    public bool ToggleWall(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return false;
      this.mWalls[p.Y, p.X] = !this.mWalls[p.Y, p.X];
      return true;
    }

    public IReadOnlyList<AStarNode> GetPaths()
    {
      List<AStarNode> paths = new List<AStarNode>();
      for (AStarNode astarNode = this.mGoal; astarNode != null; astarNode = astarNode.Parent)
        paths.Add(astarNode);
      paths.Reverse();
      return (IReadOnlyList<AStarNode>) paths;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AStarNode GetStart() => this.mStart;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AStarNode GetGoal() => this.mGoal;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PriorityQueue<(AStarNode, EDirFlags)> GetOpenList() => this.mOpenList;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashSet<AStarNode> GetCloseList() => this.mCloseList;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool[,] GetWalls() => this.mWalls;

    private AStarNode GetOrCreateNode(in Int2 p)
    {
      AStarNode node1;
      if (this.mCreateNodes.TryGetValue(p, out node1))
        return node1;
      AStarNode node2 = new AStarNode(in p);
      this.mCreateNodes.Add(p, node2);
      return node2;
    }

    public bool IsWalkable(in Int2 p) => this.IsInBoundary(in p) && !this.mWalls[p.Y, p.X];

    private bool IsInBoundary(in Int2 p)
    {
      return 0 <= p.X && p.X < this.Width && 0 <= p.Y && p.Y < this.Height;
    }

    internal EDirFlags NeighbourDir(in Int2 pos)
    {
      EDirFlags edirFlags = EDirFlags.NONE;
      for (int index = 128; index > 0; index >>= 1)
      {
        EDirFlags dir = (EDirFlags) index;
        Int2 p = pos.Foward(dir);
        if (this.IsWalkable(in p))
        {
          if (DirFlags.IsDiagonal(dir))
          {
            Int2 pos1 = DirFlags.ToPos(dir);
            p = new Int2(pos.X + pos1.X, pos.Y);
            if (!this.IsWalkable(in p) || !this.IsWalkable(new Int2(pos.X, pos.Y + pos1.Y)))
              continue;
          }
          edirFlags |= dir;
        }
      }
      return edirFlags;
    }

    internal EDirFlags OrthogonalNeighbourDir(in Int2 pos)
    {
      EDirFlags edirFlags = EDirFlags.NONE;
      for (int index = 8; index > 0; index >>= 1)
      {
        EDirFlags dir = (EDirFlags) index;
        if (this.IsWalkable(pos.Foward(dir)))
          edirFlags |= dir;
      }
      return edirFlags;
    }

    internal EDirFlags OrthogonalForcedNeighbourDir(in Int2 n, EDirFlags dir)
    {
      if (dir == EDirFlags.ALL)
        return EDirFlags.ALL;
      EDirFlags edirFlags = EDirFlags.NONE;
      Int2 x = new Int2(0, 0);
      if (DirFlags.IsStraight(dir))
        x = n.Backward(dir);
      switch (dir)
      {
        case EDirFlags.NORTH:
          if (!this.IsWalkable(x.Foward(EDirFlags.EAST)))
            edirFlags |= EDirFlags.EAST | EDirFlags.NORTHEAST;
          if (!this.IsWalkable(x.Foward(EDirFlags.WEST)))
          {
            edirFlags |= EDirFlags.WEST | EDirFlags.NORTHWEST;
            goto case EDirFlags.NORTHEAST;
          }
          else
            goto case EDirFlags.NORTHEAST;
        case EDirFlags.SOUTH:
          if (!this.IsWalkable(x.Foward(EDirFlags.EAST)))
            edirFlags |= EDirFlags.EAST | EDirFlags.SOUTHEAST;
          if (!this.IsWalkable(x.Foward(EDirFlags.WEST)))
          {
            edirFlags |= EDirFlags.WEST | EDirFlags.SOUTHWEST;
            goto case EDirFlags.NORTHEAST;
          }
          else
            goto case EDirFlags.NORTHEAST;
        case EDirFlags.EAST:
          if (!this.IsWalkable(x.Foward(EDirFlags.NORTH)))
            edirFlags |= EDirFlags.NORTH | EDirFlags.NORTHEAST;
          if (!this.IsWalkable(x.Foward(EDirFlags.SOUTH)))
          {
            edirFlags |= EDirFlags.SOUTH | EDirFlags.SOUTHEAST;
            goto case EDirFlags.NORTHEAST;
          }
          else
            goto case EDirFlags.NORTHEAST;
        case EDirFlags.WEST:
          if (!this.IsWalkable(x.Foward(EDirFlags.NORTH)))
            edirFlags |= EDirFlags.NORTH | EDirFlags.NORTHWEST;
          if (!this.IsWalkable(x.Foward(EDirFlags.SOUTH)))
          {
            edirFlags |= EDirFlags.SOUTH | EDirFlags.SOUTHWEST;
            goto case EDirFlags.NORTHEAST;
          }
          else
            goto case EDirFlags.NORTHEAST;
        case EDirFlags.NORTHEAST:
        case EDirFlags.NORTHWEST:
        case EDirFlags.SOUTHEAST:
        case EDirFlags.SOUTHWEST:
          return edirFlags;
        case EDirFlags.ALL:
          edirFlags |= EDirFlags.ALL;
          goto case EDirFlags.NORTHEAST;
        default:
          throw new ArgumentOutOfRangeException(string.Format("[ForcedNeighbourDir] invalid dir - {0}", (object) dir));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EDirFlags SuccesorsDir(in Int2 pos, EDirFlags dir)
    {
      return this.NeighbourDir(in pos) & (NF.AI.PathFinding.JPSOrthogonal.JPSOrthogonal.NaturalNeighbours(dir) | this.OrthogonalForcedNeighbourDir(in pos, dir));
    }

    internal bool TryJump(in Int2 p, EDirFlags dir, in Int2 goal, ref Int2 outJumped)
    {
      Int2 pos = p;
      Int2 x = pos.Foward(dir);
      while (true)
      {
        if (this.IsWalkable(in x) && (dir & this.NeighbourDir(in pos)) != EDirFlags.NONE)
        {
          if (!(x == goal))
          {
            if ((this.OrthogonalForcedNeighbourDir(in x, dir) & this.OrthogonalNeighbourDir(in x)) == 0)
            {
              if (DirFlags.IsDiagonal(dir))
              {
                if (!this.TryJump(in x, DirFlags.DiagonalToEastWest(dir), in goal, ref outJumped))
                {
                  if (this.TryJump(in x, DirFlags.DiagonalToNorthSouth(dir), in goal, ref outJumped))
                    goto label_10;
                }
                else
                  goto label_8;
              }
              pos = x;
              x = x.Foward(dir);
            }
            else
              goto label_5;
          }
          else
            goto label_3;
        }
        else
          break;
      }
      return false;
label_3:
      outJumped = x;
      return true;
label_5:
      outJumped = x;
      return true;
label_8:
      outJumped = x;
      return true;
label_10:
      outJumped = x;
      return true;
    }

    private static EDirFlags NaturalNeighbours(EDirFlags dir)
    {
      switch (dir)
      {
        case EDirFlags.NORTHEAST:
          return EDirFlags.NORTH | EDirFlags.EAST | EDirFlags.NORTHEAST;
        case EDirFlags.NORTHWEST:
          return EDirFlags.NORTH | EDirFlags.WEST | EDirFlags.NORTHWEST;
        case EDirFlags.SOUTHEAST:
          return EDirFlags.SOUTH | EDirFlags.EAST | EDirFlags.SOUTHEAST;
        case EDirFlags.SOUTHWEST:
          return EDirFlags.SOUTH | EDirFlags.WEST | EDirFlags.SOUTHWEST;
        default:
          return dir;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int G(AStarNode from, AStarNode adjacent)
    {
      Int2 int2 = from.Position - adjacent.Position;
      return int2.X == 0 || int2.Y == 0 ? from.G + Math.Max(Math.Abs(int2.X), Math.Abs(int2.Y)) * 10 : from.G + Math.Max(Math.Abs(int2.X), Math.Abs(int2.Y)) * 14;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int H(AStarNode n, AStarNode goal)
    {
      return (Math.Abs(goal.Position.X - n.Position.X) + Math.Abs(goal.Position.Y - n.Position.Y)) * 10;
    }
  }
}
