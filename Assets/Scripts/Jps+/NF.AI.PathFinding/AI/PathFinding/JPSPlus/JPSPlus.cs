// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlus
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Collections.Generic;
using NF.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlus
  {
    private JPSPlusNode mStart = (JPSPlusNode) null;
    private JPSPlusNode mGoal = (JPSPlusNode) null;
    private readonly Dictionary<Int2, JPSPlusNode> mCreatedNodes = new Dictionary<Int2, JPSPlusNode>();
    private readonly PriorityQueue<(JPSPlusNode Node, EDirFlags Dir)> mOpenList = new PriorityQueue<(JPSPlusNode, EDirFlags)>();
    private readonly HashSet<JPSPlusNode> mCloseList = new HashSet<JPSPlusNode>();
    private JPSPlusBakedMap mBakedMap;

    public void Init(JPSPlusBakedMap bakedMap) => this.mBakedMap = bakedMap;

    public bool StepAll(int stepCount = 2147483647)
    {
      this.mOpenList.Clear();
      this.mCloseList.Clear();
      foreach (JPSPlusNode jpsPlusNode in this.mCreatedNodes.Values.ToList<JPSPlusNode>())
      {
        int index = this.mBakedMap.BlockLUT[jpsPlusNode.Position.Y, jpsPlusNode.Position.X];
        if (index < 0)
          this.mCreatedNodes.Remove(jpsPlusNode.Position);
        else
          jpsPlusNode.Refresh(this.mBakedMap.Blocks[index].JumpDistances);
      }
      this.mOpenList.Enqueue((this.mStart, EDirFlags.ALL), this.mStart.F);
      return this.Step(stepCount);
    }

    public bool Step(int stepCount)
    {
      int num1 = stepCount;
      while (true)
      {
        if (num1 > 0 && this.mOpenList.Count != 0)
        {
          (JPSPlusNode jpsPlusNode, EDirFlags edirFlags1) = this.mOpenList.Dequeue();
          this.mCloseList.Add(jpsPlusNode);
          Int2 curr = jpsPlusNode.Position;
          Int2 goal = this.mGoal.Position;
          if (!(curr == goal))
          {
            EDirFlags edirFlags2 = this.ValidLookUPTable(edirFlags1);
            for (int index = 128; index > 0; index >>= 1)
            {
              EDirFlags edirFlags3 = (EDirFlags) index;
              if ((edirFlags3 & edirFlags2) != EDirFlags.NONE)
              {
                bool flag = DirFlags.IsDiagonal(edirFlags3);
                int distance = jpsPlusNode.GetDistance(edirFlags3);
                int val1 = this.RowDiff(jpsPlusNode, this.mGoal);
                int val2 = this.ColDiff(jpsPlusNode, this.mGoal);
                JPSPlusNode n;
                int num2;
                if (!flag && this.IsGoalInExactDirection(in curr, edirFlags3, in goal) && Math.Max(val1, val2) <= Math.Abs(distance))
                {
                  n = this.mGoal;
                  num2 = jpsPlusNode.G + Math.Max(val1, val2) * 10;
                }
                else if (flag && this.IsGoalInGeneralDirection(in curr, edirFlags3, in goal) && (val1 <= Math.Abs(distance) || val2 <= Math.Abs(distance)))
                {
                  int dist = Math.Min(val1, val2);
                  n = this.GetNode(jpsPlusNode, dist, edirFlags3);
                  num2 = jpsPlusNode.G + Math.Max(val1, val2) * 14;
                }
                else if (distance > 0)
                {
                  n = this.GetNode(jpsPlusNode, edirFlags3);
                  num2 = !flag ? jpsPlusNode.G + Math.Max(val1, val2) * 10 : jpsPlusNode.G + Math.Max(val1, val2) * 14;
                }
                else
                  continue;
                (JPSPlusNode, EDirFlags) valueTuple = (n, edirFlags3);
                if (!this.mOpenList.Contains(valueTuple) && !this.mCloseList.Contains(n))
                {
                  n.Parent = (AStarNode) jpsPlusNode;
                  n.G = num2;
                  n.H = NF.AI.PathFinding.JPSPlus.JPSPlus.H(n, this.mGoal);
                  this.mOpenList.Enqueue(valueTuple, n.F);
                }
                else if (num2 < n.G)
                {
                  n.Parent = (AStarNode) jpsPlusNode;
                  n.G = num2;
                  n.H = NF.AI.PathFinding.JPSPlus.JPSPlus.H(n, this.mGoal);
                  this.mOpenList.UpdatePriority(valueTuple, n.F);
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

    public bool IsWalkable(in Int2 p)
    {
      return this.mBakedMap != null && this.IsInBoundary(in p) && this.mBakedMap.BlockLUT[p.Y, p.X] >= 0;
    }

    public bool SetStart(in Int2 p)
    {
      if (this.mBakedMap == null || !this.IsInBoundary(in p))
        return false;
      this.mStart = this.GetOrCreatedNode(in p);
      return true;
    }

    public bool SetGoal(in Int2 p)
    {
      if (this.mBakedMap == null || !this.IsInBoundary(in p))
        return false;
      this.mGoal = this.GetOrCreatedNode(in p);
      return true;
    }

    public JPSPlusNode GetJPSPlusNode(in Int2 p)
    {
      return new JPSPlusNode(in p, this.mBakedMap.Blocks[this.mBakedMap.BlockLUT[p.Y, p.X]].JumpDistances);
    }

    public IReadOnlyList<AStarNode> GetPaths()
    {
      List<AStarNode> paths = new List<AStarNode>();
      for (AStarNode astarNode = (AStarNode) this.mGoal; astarNode != null; astarNode = astarNode.Parent)
        paths.Add(astarNode);
      paths.Reverse();
      return (IReadOnlyList<AStarNode>) paths;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JPSPlusNode GetStart() => this.mStart;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JPSPlusNode GetGoal() => this.mGoal;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PriorityQueue<(JPSPlusNode, EDirFlags)> GetOpenList() => this.mOpenList;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashSet<JPSPlusNode> GetCloseList() => this.mCloseList;

    public int Width => this.mBakedMap.Width;

    public int Height => this.mBakedMap.Height;

    private JPSPlusNode GetOrCreatedNode(in Int2 p)
    {
      JPSPlusNode orCreatedNode;
      if (this.mCreatedNodes.TryGetValue(p, out orCreatedNode))
        return orCreatedNode;
      JPSPlusNode jpsPlusNode = this.GetJPSPlusNode(in p);
      this.mCreatedNodes.Add(p, jpsPlusNode);
      return jpsPlusNode;
    }

    private bool IsInBoundary(in Int2 p)
    {
      return 0 <= p.X && p.X < this.Width && 0 <= p.Y && p.Y < this.Height;
    }

    private EDirFlags ValidLookUPTable(EDirFlags dir)
    {
      switch (dir)
      {
        case EDirFlags.NORTH:
          return EDirFlags.NORTH | EDirFlags.EAST | EDirFlags.WEST | EDirFlags.NORTHEAST | EDirFlags.NORTHWEST;
        case EDirFlags.SOUTH:
          return EDirFlags.SOUTH | EDirFlags.EAST | EDirFlags.WEST | EDirFlags.SOUTHEAST | EDirFlags.SOUTHWEST;
        case EDirFlags.EAST:
          return EDirFlags.NORTH | EDirFlags.SOUTH | EDirFlags.EAST | EDirFlags.NORTHEAST | EDirFlags.SOUTHEAST;
        case EDirFlags.WEST:
          return EDirFlags.NORTH | EDirFlags.SOUTH | EDirFlags.WEST | EDirFlags.NORTHWEST | EDirFlags.SOUTHWEST;
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

    private bool IsGoalInExactDirection(in Int2 curr, EDirFlags processDir, in Int2 goal)
    {
      int num1 = goal.X - curr.X;
      int num2 = goal.Y - curr.Y;
      switch (processDir)
      {
        case EDirFlags.NORTH:
          return num1 == 0 && num2 < 0;
        case EDirFlags.SOUTH:
          return num1 == 0 && num2 > 0;
        case EDirFlags.EAST:
          return num1 > 0 && num2 == 0;
        case EDirFlags.WEST:
          return num1 < 0 && num2 == 0;
        case EDirFlags.NORTHEAST:
          return num1 > 0 && num2 < 0 && Math.Abs(num1) == Math.Abs(num2);
        case EDirFlags.NORTHWEST:
          return num1 < 0 && num2 < 0 && Math.Abs(num1) == Math.Abs(num2);
        case EDirFlags.SOUTHEAST:
          return num1 > 0 && num2 > 0 && Math.Abs(num1) == Math.Abs(num2);
        case EDirFlags.SOUTHWEST:
          return num1 < 0 && num2 > 0 && Math.Abs(num1) == Math.Abs(num2);
        default:
          return false;
      }
    }

    private bool IsGoalInGeneralDirection(in Int2 curr, EDirFlags processDir, in Int2 goal)
    {
      int num1 = goal.X - curr.X;
      int num2 = goal.Y - curr.Y;
      switch (processDir)
      {
        case EDirFlags.NORTH:
          return num1 == 0 && num2 < 0;
        case EDirFlags.SOUTH:
          return num1 == 0 && num2 > 0;
        case EDirFlags.EAST:
          return num1 > 0 && num2 == 0;
        case EDirFlags.WEST:
          return num1 < 0 && num2 == 0;
        case EDirFlags.NORTHEAST:
          return num1 > 0 && num2 < 0;
        case EDirFlags.NORTHWEST:
          return num1 < 0 && num2 < 0;
        case EDirFlags.SOUTHEAST:
          return num1 > 0 && num2 > 0;
        case EDirFlags.SOUTHWEST:
          return num1 < 0 && num2 > 0;
        default:
          return false;
      }
    }

    private JPSPlusNode GetNode(JPSPlusNode node, EDirFlags dir)
    {
      return this.GetOrCreatedNode(node.Position + DirFlags.ToPos(dir) * node.GetDistance(dir));
    }

    private JPSPlusNode GetNode(JPSPlusNode node, int dist, EDirFlags dir)
    {
      return this.GetOrCreatedNode(node.Position + DirFlags.ToPos(dir) * dist);
    }

    private int ColDiff(JPSPlusNode currNode, JPSPlusNode goalNode)
    {
      return Math.Abs(goalNode.Position.X - currNode.Position.X);
    }

    private int RowDiff(JPSPlusNode currNode, JPSPlusNode goalNode)
    {
      return Math.Abs(goalNode.Position.Y - currNode.Position.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int H(JPSPlusNode n, JPSPlusNode goal)
    {
      return (Math.Abs(goal.Position.X - n.Position.X) + Math.Abs(goal.Position.Y - n.Position.Y)) * 10;
    }
  }
}
