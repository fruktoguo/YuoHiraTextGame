// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPS.JPS
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
namespace NF.AI.PathFinding.JPS
{
  public class JPS
  {
    private AStarNode mStart = (AStarNode) null;
    private AStarNode mGoal = (AStarNode) null;
    private readonly Dictionary<Int2, AStarNode> mCreatedNodes = new Dictionary<Int2, AStarNode>();
    private readonly PriorityQueue<(AStarNode Node, EDirFlags Dir)> mOpenList = new PriorityQueue<(AStarNode, EDirFlags)>();
    private readonly HashSet<AStarNode> mCloseList = new HashSet<AStarNode>();
    private bool[,] mWalls = (bool[,]) null;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Int2 StartP => this.mStart.Position;

    public Int2 GoalP => this.mGoal.Position;

    public JPS()
    {
    }

    public JPS(int width, int height) => this.Init(new bool[height, width]);

    public JPS(bool[,] walls) => this.Init(walls);

    public void Init(bool[,] walls)
    {
      this.mWalls = walls;
      this.Width = walls.GetLength(1);
      this.Height = walls.GetLength(0);
    }

    public bool StepAll(int stepCount = 2147483647)
    {
      this.mOpenList.Clear();
      this.mCloseList.Clear();
      foreach (AStarNode astarNode in this.mCreatedNodes.Values)
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
                  int num2 = NF.AI.PathFinding.JPS.JPS.G(astarNode, node);
                  (AStarNode, EDirFlags) valueTuple = (node, dir);
                  if (!this.mOpenList.Contains(valueTuple))
                  {
                    node.Parent = astarNode;
                    node.G = num2;
                    node.H = NF.AI.PathFinding.JPS.JPS.H(node, this.mGoal);
                    this.mOpenList.Enqueue(valueTuple, node.F);
                  }
                  else if (num2 < node.G)
                  {
                    node.Parent = astarNode;
                    node.G = num2;
                    node.H = NF.AI.PathFinding.JPS.JPS.H(node, this.mGoal);
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

    public void SetStart(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mStart = this.GetOrCreateNode(in p);
    }

    public void SetGoal(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mGoal = this.GetOrCreateNode(in p);
    }

    public void SetWall(in Int2 p, bool isWall)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = isWall;
    }

    public void ToggleWall(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = !this.mWalls[p.Y, p.X];
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

    public bool IsWalkable(in Int2 p) => this.IsInBoundary(in p) && !this.mWalls[p.Y, p.X];

    private AStarNode GetOrCreateNode(in Int2 p)
    {
      AStarNode node1;
      if (this.mCreatedNodes.TryGetValue(p, out node1))
        return node1;
      AStarNode node2 = new AStarNode(in p);
      this.mCreatedNodes.Add(p, node2);
      return node2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (!this.IsWalkable(in p) && !this.IsWalkable(new Int2(pos.X, pos.Y + pos1.Y)))
              continue;
          }
          edirFlags |= dir;
        }
      }
      return edirFlags;
    }

    internal EDirFlags ForcedNeighbourDir(in Int2 n, EDirFlags dir)
    {
      EDirFlags edirFlags = EDirFlags.NONE;
      switch (dir)
      {
        case EDirFlags.NORTH:
          Int2 p1 = n.Foward(EDirFlags.EAST);
          if (!this.IsWalkable(in p1))
            edirFlags |= EDirFlags.NORTHEAST;
          p1 = n.Foward(EDirFlags.WEST);
          if (!this.IsWalkable(in p1))
          {
            edirFlags |= EDirFlags.NORTHWEST;
            break;
          }
          break;
        case EDirFlags.SOUTH:
          Int2 p2 = n.Foward(EDirFlags.EAST);
          if (!this.IsWalkable(in p2))
            edirFlags |= EDirFlags.SOUTHEAST;
          p2 = n.Foward(EDirFlags.WEST);
          if (!this.IsWalkable(in p2))
          {
            edirFlags |= EDirFlags.SOUTHWEST;
            break;
          }
          break;
        case EDirFlags.EAST:
          Int2 p3 = n.Foward(EDirFlags.NORTH);
          if (!this.IsWalkable(in p3))
            edirFlags |= EDirFlags.NORTHEAST;
          p3 = n.Foward(EDirFlags.SOUTH);
          if (!this.IsWalkable(in p3))
          {
            edirFlags |= EDirFlags.SOUTHEAST;
            break;
          }
          break;
        case EDirFlags.WEST:
          Int2 p4 = n.Foward(EDirFlags.NORTH);
          if (!this.IsWalkable(in p4))
            edirFlags |= EDirFlags.NORTHWEST;
          p4 = n.Foward(EDirFlags.SOUTH);
          if (!this.IsWalkable(in p4))
          {
            edirFlags |= EDirFlags.SOUTHWEST;
            break;
          }
          break;
        case EDirFlags.NORTHEAST:
          Int2 p5 = n.Foward(EDirFlags.NORTH);
          Int2 p6;
          int num1;
          if (this.IsWalkable(in p5))
          {
            p6 = n.Foward(EDirFlags.WEST);
            num1 = !this.IsWalkable(in p6) ? 1 : 0;
          }
          else
            num1 = 0;
          if (num1 != 0)
            edirFlags |= EDirFlags.NORTHWEST;
          p5 = n.Foward(EDirFlags.SOUTH);
          int num2;
          if (!this.IsWalkable(in p5))
          {
            p6 = n.Foward(EDirFlags.EAST);
            num2 = this.IsWalkable(in p6) ? 1 : 0;
          }
          else
            num2 = 0;
          if (num2 != 0)
          {
            edirFlags |= EDirFlags.SOUTHEAST;
            break;
          }
          break;
        case EDirFlags.NORTHWEST:
          Int2 p7 = n.Foward(EDirFlags.NORTH);
          Int2 p8;
          int num3;
          if (this.IsWalkable(in p7))
          {
            p8 = n.Foward(EDirFlags.EAST);
            num3 = !this.IsWalkable(in p8) ? 1 : 0;
          }
          else
            num3 = 0;
          if (num3 != 0)
            edirFlags |= EDirFlags.NORTHEAST;
          p7 = n.Foward(EDirFlags.SOUTH);
          int num4;
          if (!this.IsWalkable(in p7))
          {
            p8 = n.Foward(EDirFlags.WEST);
            num4 = this.IsWalkable(in p8) ? 1 : 0;
          }
          else
            num4 = 0;
          if (num4 != 0)
          {
            edirFlags |= EDirFlags.SOUTHWEST;
            break;
          }
          break;
        case EDirFlags.SOUTHEAST:
          Int2 p9 = n.Foward(EDirFlags.SOUTH);
          Int2 p10;
          int num5;
          if (this.IsWalkable(in p9))
          {
            p10 = n.Foward(EDirFlags.WEST);
            num5 = !this.IsWalkable(in p10) ? 1 : 0;
          }
          else
            num5 = 0;
          if (num5 != 0)
            edirFlags |= EDirFlags.SOUTHWEST;
          p9 = n.Foward(EDirFlags.NORTH);
          int num6;
          if (!this.IsWalkable(in p9))
          {
            p10 = n.Foward(EDirFlags.EAST);
            num6 = this.IsWalkable(in p10) ? 1 : 0;
          }
          else
            num6 = 0;
          if (num6 != 0)
          {
            edirFlags |= EDirFlags.NORTHEAST;
            break;
          }
          break;
        case EDirFlags.SOUTHWEST:
          Int2 p11 = n.Foward(EDirFlags.SOUTH);
          Int2 p12;
          int num7;
          if (this.IsWalkable(in p11))
          {
            p12 = n.Foward(EDirFlags.EAST);
            num7 = !this.IsWalkable(in p12) ? 1 : 0;
          }
          else
            num7 = 0;
          if (num7 != 0)
            edirFlags |= EDirFlags.SOUTHEAST;
          p11 = n.Foward(EDirFlags.NORTH);
          int num8;
          if (!this.IsWalkable(in p11))
          {
            p12 = n.Foward(EDirFlags.WEST);
            num8 = this.IsWalkable(in p12) ? 1 : 0;
          }
          else
            num8 = 0;
          if (num8 != 0)
          {
            edirFlags |= EDirFlags.NORTHWEST;
            break;
          }
          break;
        case EDirFlags.ALL:
          edirFlags |= EDirFlags.ALL;
          break;
        default:
          throw new ArgumentOutOfRangeException(string.Format("[ForcedNeighbourDir] invalid dir - {0}", (object) dir));
      }
      return edirFlags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EDirFlags SuccesorsDir(in Int2 pos, EDirFlags dir)
    {
      return this.NeighbourDir(in pos) & (NF.AI.PathFinding.JPS.JPS.NaturalNeighbours(dir) | this.ForcedNeighbourDir(in pos, dir));
    }

    internal bool TryJump(in Int2 p, EDirFlags dir, in Int2 goal, ref Int2 outJumped)
    {
      return DirFlags.IsStraight(dir) ? this.TryJumpStraight(in p, dir, in goal, ref outJumped) : this.TryJumpDiagonal(in p, dir, in goal, ref outJumped);
    }

    internal bool TryJumpStraight(in Int2 p, EDirFlags dir, in Int2 goal, ref Int2 outJumped)
    {
      Int2 pos = p;
      Int2 int2 = pos.Foward(dir);
      while (true)
      {
        if (this.IsWalkable(in int2) && (dir & this.NeighbourDir(in pos)) != EDirFlags.NONE)
        {
          if (!(int2 == goal))
          {
            if ((this.ForcedNeighbourDir(in int2, dir) & this.NeighbourDir(in int2)) == 0)
            {
              pos = int2;
              int2 = pos.Foward(dir);
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
      outJumped = int2;
      return true;
label_5:
      outJumped = int2;
      return true;
    }

    internal bool TryJumpDiagonal(in Int2 p, EDirFlags dir, in Int2 goal, ref Int2 outJumped)
    {
      Int2 pos = p;
      Int2 int2 = pos.Foward(dir);
      while (true)
      {
        if (this.IsWalkable(in int2) && (dir & this.NeighbourDir(in pos)) != EDirFlags.NONE)
        {
          if (!(int2 == goal))
          {
            if ((this.ForcedNeighbourDir(in int2, dir) & this.NeighbourDir(in int2)) == 0)
            {
              if (!this.TryJumpStraight(in int2, DirFlags.DiagonalToEastWest(dir), in goal, ref outJumped))
              {
                if (!this.TryJumpStraight(in int2, DirFlags.DiagonalToNorthSouth(dir), in goal, ref outJumped))
                {
                  pos = int2;
                  int2 = pos.Foward(dir);
                }
                else
                  goto label_9;
              }
              else
                goto label_7;
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
      outJumped = int2;
      return true;
label_5:
      outJumped = int2;
      return true;
label_7:
      outJumped = int2;
      return true;
label_9:
      outJumped = int2;
      return true;
    }

    internal static EDirFlags NaturalNeighbours(EDirFlags dir)
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
