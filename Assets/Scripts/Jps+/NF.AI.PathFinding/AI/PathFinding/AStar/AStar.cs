// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.AStar.AStar
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Collections.Generic;
using NF.Mathematics;
using System;
using System.Collections.Generic;

#nullable disable
namespace NF.AI.PathFinding.AStar
{
  public class AStar
  {
    private AStarNode mStart = (AStarNode) null;
    private AStarNode mGoal = (AStarNode) null;
    private readonly AStarNode[,] mNodes;
    private readonly PriorityQueue<AStarNode> mOpenList = new PriorityQueue<AStarNode>();
    private readonly HashSet<AStarNode> mCloseList = new HashSet<AStarNode>();
    private readonly bool[,] mWalls;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Int2 StartP => this.mStart.Position;

    public Int2 GoalP => this.mGoal.Position;

    public AStar(int width, int height)
    {
      this.Width = width;
      this.Height = height;
      this.mNodes = new AStarNode[this.Height, this.Width];
      this.mWalls = new bool[this.Height, this.Width];
    }

    public AStar(bool[,] walls)
    {
      this.Height = walls.GetLength(0);
      this.Width = walls.GetLength(1);
      this.mNodes = new AStarNode[this.Height, this.Width];
      this.mWalls = walls;
    }

    public bool StepAll()
    {
      this.mOpenList.Clear();
      this.mCloseList.Clear();
      this.mOpenList.Enqueue(this.mStart, this.mStart.F);
      this.mGoal.Parent = (AStarNode) null;
      return this.Step(int.MaxValue);
    }

    public bool Step(int stepCount)
    {
      for (int index1 = stepCount; index1 > 0 && this.mOpenList.Count != 0; --index1)
      {
        AStarNode from = this.mOpenList.Dequeue();
        if (from == this.mGoal)
          return true;
        this.mCloseList.Add(from);
        for (int index2 = 128; index2 > 0; index2 >>= 1)
        {
          EDirFlags dir = (EDirFlags) index2;
          Int2 pos1 = DirFlags.ToPos(dir);
          Int2 pos2 = from.Position + pos1;
          AStarNode nodeOrNull = this.GetNodeOrNull(in pos2);
          if (nodeOrNull != null)
          {
            pos2 = nodeOrNull.Position;
            if (!this.IsWall(in pos2))
            {
              if (DirFlags.IsDiagonal(dir))
              {
                pos2 = from.Position + new Int2(pos1.X, 0);
                if (this.IsWall(in pos2) || this.IsWall(from.Position + new Int2(0, pos1.Y)))
                  continue;
              }
              if (!this.mCloseList.Contains(nodeOrNull))
              {
                int num = NF.AI.PathFinding.AStar.AStar.G(from, nodeOrNull);
                if (!this.mOpenList.Contains(nodeOrNull))
                {
                  nodeOrNull.Parent = from;
                  nodeOrNull.G = num;
                  nodeOrNull.H = NF.AI.PathFinding.AStar.AStar.H(nodeOrNull, this.mGoal);
                  this.mOpenList.Enqueue(nodeOrNull, nodeOrNull.F);
                }
                else if (num < nodeOrNull.G)
                {
                  nodeOrNull.Parent = from;
                  nodeOrNull.G = num;
                  nodeOrNull.H = NF.AI.PathFinding.AStar.AStar.H(nodeOrNull, this.mGoal);
                  this.mOpenList.UpdatePriority(nodeOrNull, nodeOrNull.F);
                }
              }
            }
          }
        }
      }
      return false;
    }

    public bool SetStart(in Int2 pos)
    {
      if (!this.IsInBoundary(in pos))
        return false;
      this.mStart = this.GetNodeOrNull(in pos);
      return true;
    }

    public bool SetGoal(in Int2 pos)
    {
      if (!this.IsInBoundary(in pos))
        return false;
      this.mGoal = this.GetNodeOrNull(in pos);
      return true;
    }

    public void SetWall(in Int2 p, bool isWall)
    {
      if (!this.IsInBoundary(in p))
        return;
      this.mWalls[p.Y, p.X] = isWall;
    }

    public bool ToggleWall(in Int2 pos)
    {
      if (!this.IsInBoundary(in pos))
        return false;
      this.mWalls[pos.Y, pos.X] = !this.mWalls[pos.Y, pos.X];
      return true;
    }

    public List<AStarNode> GetPaths()
    {
      List<AStarNode> paths = new List<AStarNode>();
      for (AStarNode astarNode = this.mGoal; astarNode != null; astarNode = astarNode.Parent)
        paths.Add(astarNode);
      paths.Reverse();
      return paths;
    }

    public AStarNode GetStart() => this.mStart;

    public AStarNode GetGoal() => this.mGoal;

    public PriorityQueue<AStarNode> GetOpenList() => this.mOpenList;

    public HashSet<AStarNode> GetCloseList() => this.mCloseList;

    public bool[,] GetWalls() => this.mWalls;

    public bool IsWalkable(in Int2 p) => this.IsInBoundary(in p) && !this.IsWall(in p);

    private bool IsInBoundary(in Int2 pos) => this.IsInBoundary(pos.X, pos.Y);

    private bool IsInBoundary(int x, int y)
    {
      return 0 <= x && x < this.Width && 0 <= y && y < this.Height;
    }

    private AStarNode GetNodeOrNull(in Int2 pos)
    {
      int x = pos.X;
      int y = pos.Y;
      if (!this.IsInBoundary(x, y))
        return (AStarNode) null;
      AStarNode mNode = this.mNodes[y, x];
      if (mNode != null)
        return mNode;
      AStarNode nodeOrNull = new AStarNode(x, y);
      this.mNodes[y, x] = nodeOrNull;
      return nodeOrNull;
    }

    private bool IsWall(in Int2 pos) => this.mWalls[pos.Y, pos.X];

    private static int G(AStarNode from, AStarNode adjacent)
    {
      Int2 int2 = from.Position - adjacent.Position;
      return int2.X == 0 || int2.Y == 0 ? from.G + 10 : from.G + 14;
    }

    internal static int H(AStarNode n, AStarNode goal)
    {
      return Math.Abs(goal.Position.X - n.Position.X) + Math.Abs(goal.Position.Y - n.Position.Y) * 10;
    }
  }
}
