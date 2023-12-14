// Decompiled with JetBrains decompiler
// Type: NF.AI.PathFinding.JPSPlus.JPSPlusMapBaker
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using NF.AI.PathFinding.Common;
using NF.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace NF.AI.PathFinding.JPSPlus
{
  public class JPSPlusMapBaker
  {
    public int[,] BlockLUT;
    public JPSPlusMapBakerBlock[] Blocks;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public JPSPlusMapBaker()
    {
    }

    public JPSPlusMapBaker(bool[,] walls) => this.Init(walls);

    public void Init(bool[,] walls)
    {
      this.Width = walls.GetLength(1);
      this.Height = walls.GetLength(0);
      this.BlockLUT = new int[this.Height, this.Width];
      List<JPSPlusMapBakerBlock> plusMapBakerBlockList = new List<JPSPlusMapBakerBlock>();
      int num = 0;
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
        {
          if (walls[y, x])
          {
            this.BlockLUT[y, x] = -1;
          }
          else
          {
            this.BlockLUT[y, x] = num;
            plusMapBakerBlockList.Add(new JPSPlusMapBakerBlock(new Int2(x, y)));
            ++num;
          }
        }
      }
      this.Blocks = plusMapBakerBlockList.ToArray();
    }

    public JPSPlusBakedMap Bake()
    {
      this.MarkPrimary();
      this.MarkStraight();
      this.MarkDiagonal();
      return new JPSPlusBakedMap(this.BlockLUT, ((IEnumerable<JPSPlusMapBakerBlock>) this.Blocks).Select<JPSPlusMapBakerBlock, JPSPlusBakedMap.JPSPlusBakedMapBlock>((Func<JPSPlusMapBakerBlock, JPSPlusBakedMap.JPSPlusBakedMapBlock>) (x => new JPSPlusBakedMap.JPSPlusBakedMapBlock(in x.Pos, x.JumpDistances))).ToArray<JPSPlusBakedMap.JPSPlusBakedMapBlock>());
    }

    private void MarkPrimary()
    {
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
        {
          Int2 p1 = new Int2(x, y);
          if (!this.IsWalkable(in p1))
          {
            for (int index = 128; index > 15; index >>= 1)
            {
              EDirFlags dir = (EDirFlags) index;
              JPSPlusMapBakerBlock blockOrNull = this.GetBlockOrNull(p1.Foward(dir));
              if (blockOrNull != null)
              {
                switch (dir)
                {
                  case EDirFlags.NORTHEAST:
                    Int2 p2 = p1.Foward(EDirFlags.NORTH);
                    Int2 p3 = p1.Foward(EDirFlags.EAST);
                    if (this.IsWalkable(in p2) && this.IsWalkable(in p3))
                    {
                      blockOrNull.JumpDirFlags |= EDirFlags.SOUTH | EDirFlags.WEST;
                      break;
                    }
                    break;
                  case EDirFlags.NORTHWEST:
                    Int2 p4 = p1.Foward(EDirFlags.NORTH);
                    Int2 p5 = p1.Foward(EDirFlags.WEST);
                    if (this.IsWalkable(in p4) && this.IsWalkable(in p5))
                    {
                      blockOrNull.JumpDirFlags |= EDirFlags.SOUTH | EDirFlags.EAST;
                      break;
                    }
                    break;
                  case EDirFlags.SOUTHEAST:
                    Int2 p6 = p1.Foward(EDirFlags.SOUTH);
                    Int2 p7 = p1.Foward(EDirFlags.EAST);
                    if (this.IsWalkable(in p6) && this.IsWalkable(in p7))
                    {
                      blockOrNull.JumpDirFlags |= EDirFlags.NORTH | EDirFlags.WEST;
                      break;
                    }
                    break;
                  case EDirFlags.SOUTHWEST:
                    Int2 p8 = p1.Foward(EDirFlags.SOUTH);
                    Int2 p9 = p1.Foward(EDirFlags.WEST);
                    if (this.IsWalkable(in p8) && this.IsWalkable(in p9))
                    {
                      blockOrNull.JumpDirFlags |= EDirFlags.NORTH | EDirFlags.EAST;
                      break;
                    }
                    break;
                  default:
                    throw new ArgumentException();
                }
              }
            }
          }
        }
      }
    }

    private void MarkStraight()
    {
      for (int y = 0; y < this.Height; ++y)
      {
        bool flag = false;
        int distance = -1;
        for (int x = 0; x < this.Width; ++x)
        {
          Int2 p = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull = this.GetBlockOrNull(in p);
          if (blockOrNull == null)
          {
            distance = -1;
            flag = false;
          }
          else
          {
            ++distance;
            if (flag)
              blockOrNull.SetDistance(EDirFlags.WEST, distance);
            else
              blockOrNull.SetDistance(EDirFlags.WEST, -distance);
            if (blockOrNull.IsJumpable(EDirFlags.EAST))
            {
              distance = 0;
              flag = true;
            }
          }
        }
      }
      for (int y = 0; y < this.Height; ++y)
      {
        bool flag = false;
        int distance = -1;
        for (int x = this.Width - 1; x >= 0; --x)
        {
          Int2 p = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull = this.GetBlockOrNull(in p);
          if (blockOrNull == null)
          {
            distance = -1;
            flag = false;
          }
          else
          {
            ++distance;
            if (flag)
              blockOrNull.SetDistance(EDirFlags.EAST, distance);
            else
              blockOrNull.SetDistance(EDirFlags.EAST, -distance);
            if (blockOrNull.IsJumpable(EDirFlags.WEST))
            {
              distance = 0;
              flag = true;
            }
          }
        }
      }
      for (int x = 0; x < this.Width; ++x)
      {
        bool flag = false;
        int distance = -1;
        for (int y = this.Height - 1; y >= 0; --y)
        {
          Int2 p = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull = this.GetBlockOrNull(in p);
          if (blockOrNull == null)
          {
            distance = -1;
            flag = false;
          }
          else
          {
            ++distance;
            if (flag)
              blockOrNull.SetDistance(EDirFlags.SOUTH, distance);
            else
              blockOrNull.SetDistance(EDirFlags.SOUTH, -distance);
            if (blockOrNull.IsJumpable(EDirFlags.NORTH))
            {
              distance = 0;
              flag = true;
            }
          }
        }
      }
      for (int x = 0; x < this.Width; ++x)
      {
        bool flag = false;
        int distance = -1;
        for (int y = 0; y < this.Height; ++y)
        {
          Int2 p = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull = this.GetBlockOrNull(in p);
          if (blockOrNull == null)
          {
            distance = -1;
            flag = false;
          }
          else
          {
            ++distance;
            if (flag)
              blockOrNull.SetDistance(EDirFlags.NORTH, distance);
            else
              blockOrNull.SetDistance(EDirFlags.NORTH, -distance);
            if (blockOrNull.IsJumpable(EDirFlags.SOUTH))
            {
              distance = 0;
              flag = true;
            }
          }
        }
      }
    }

    private void MarkDiagonal()
    {
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = 0; x < this.Width; ++x)
        {
          Int2 p1 = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull1 = this.GetBlockOrNull(in p1);
          if (blockOrNull1 != null)
          {
            if (x == 0 || y == 0)
            {
              blockOrNull1.SetDistance(EDirFlags.NORTHWEST, 0);
            }
            else
            {
              Int2 p2 = p1.Foward(EDirFlags.NORTH);
              Int2 p3 = p1.Foward(EDirFlags.NORTHWEST);
              Int2 p4 = p1.Foward(EDirFlags.WEST);
              bool flag1 = this.IsWalkable(in p2);
              bool flag2 = this.IsWalkable(in p4);
              if (!flag1 || !this.IsWalkable(in p3) || !flag2)
              {
                blockOrNull1.SetDistance(EDirFlags.NORTHWEST, 0);
              }
              else
              {
                JPSPlusMapBakerBlock blockOrNull2 = this.GetBlockOrNull(in p3);
                if (flag1 & flag2 && (blockOrNull2.GetDistance(EDirFlags.NORTH) > 0 || blockOrNull2.GetDistance(EDirFlags.WEST) > 0))
                {
                  blockOrNull1.SetDistance(EDirFlags.NORTHWEST, 1);
                }
                else
                {
                  int distance = blockOrNull2.GetDistance(EDirFlags.NORTHWEST);
                  if (distance > 0)
                    blockOrNull1.SetDistance(EDirFlags.NORTHWEST, distance + 1);
                  else
                    blockOrNull1.SetDistance(EDirFlags.NORTHWEST, distance - 1);
                }
              }
            }
          }
        }
      }
      for (int y = 0; y < this.Height; ++y)
      {
        for (int x = this.Width - 1; x >= 0; --x)
        {
          Int2 p5 = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull3 = this.GetBlockOrNull(in p5);
          if (blockOrNull3 != null)
          {
            if (x == this.Width - 1 || y == 0)
            {
              blockOrNull3.SetDistance(EDirFlags.NORTHEAST, 0);
            }
            else
            {
              Int2 p6 = p5.Foward(EDirFlags.NORTH);
              Int2 p7 = p5.Foward(EDirFlags.NORTHEAST);
              Int2 p8 = p5.Foward(EDirFlags.EAST);
              bool flag3 = this.IsWalkable(in p6);
              bool flag4 = this.IsWalkable(in p8);
              if (!flag3 || !this.IsWalkable(in p7) || !flag4)
              {
                blockOrNull3.SetDistance(EDirFlags.NORTHEAST, 0);
              }
              else
              {
                JPSPlusMapBakerBlock blockOrNull4 = this.GetBlockOrNull(in p7);
                if (flag3 & flag4 && (blockOrNull4.GetDistance(EDirFlags.NORTH) > 0 || blockOrNull4.GetDistance(EDirFlags.EAST) > 0))
                {
                  blockOrNull3.SetDistance(EDirFlags.NORTHEAST, 1);
                }
                else
                {
                  int distance = blockOrNull4.GetDistance(EDirFlags.NORTHEAST);
                  if (distance > 0)
                    blockOrNull3.SetDistance(EDirFlags.NORTHEAST, distance + 1);
                  else
                    blockOrNull3.SetDistance(EDirFlags.NORTHEAST, distance - 1);
                }
              }
            }
          }
        }
      }
      for (int y = this.Height - 1; y >= 0; --y)
      {
        for (int x = 0; x < this.Width; ++x)
        {
          Int2 p9 = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull5 = this.GetBlockOrNull(in p9);
          if (blockOrNull5 != null)
          {
            if (x == 0 || y == this.Height - 1)
            {
              blockOrNull5.SetDistance(EDirFlags.SOUTHWEST, 0);
            }
            else
            {
              Int2 p10 = p9.Foward(EDirFlags.SOUTH);
              Int2 p11 = p9.Foward(EDirFlags.SOUTHWEST);
              Int2 p12 = p9.Foward(EDirFlags.WEST);
              bool flag5 = this.IsWalkable(in p10);
              bool flag6 = this.IsWalkable(in p12);
              if (!flag5 || !this.IsWalkable(in p11) || !flag6)
              {
                blockOrNull5.SetDistance(EDirFlags.SOUTHWEST, 0);
              }
              else
              {
                JPSPlusMapBakerBlock blockOrNull6 = this.GetBlockOrNull(in p11);
                if (flag5 & flag6 && (blockOrNull6.GetDistance(EDirFlags.SOUTH) > 0 || blockOrNull6.GetDistance(EDirFlags.WEST) > 0))
                {
                  blockOrNull5.SetDistance(EDirFlags.SOUTHWEST, 1);
                }
                else
                {
                  int distance = blockOrNull6.GetDistance(EDirFlags.SOUTHWEST);
                  if (distance > 0)
                    blockOrNull5.SetDistance(EDirFlags.SOUTHWEST, distance + 1);
                  else
                    blockOrNull5.SetDistance(EDirFlags.SOUTHWEST, distance - 1);
                }
              }
            }
          }
        }
      }
      for (int y = this.Height - 1; y >= 0; --y)
      {
        for (int x = this.Width - 1; x >= 0; --x)
        {
          Int2 p13 = new Int2(x, y);
          JPSPlusMapBakerBlock blockOrNull7 = this.GetBlockOrNull(in p13);
          if (blockOrNull7 != null)
          {
            if (x == this.Width - 1 || y == this.Height - 1)
            {
              blockOrNull7.SetDistance(EDirFlags.SOUTHEAST, 0);
            }
            else
            {
              Int2 p14 = p13.Foward(EDirFlags.SOUTH);
              Int2 p15 = p13.Foward(EDirFlags.SOUTHEAST);
              Int2 p16 = p13.Foward(EDirFlags.EAST);
              bool flag7 = this.IsWalkable(in p14);
              bool flag8 = this.IsWalkable(in p16);
              if (!flag7 || !this.IsWalkable(in p15) || !flag8)
              {
                blockOrNull7.SetDistance(EDirFlags.SOUTHEAST, 0);
              }
              else
              {
                JPSPlusMapBakerBlock blockOrNull8 = this.GetBlockOrNull(in p15);
                if (flag7 & flag8 && (blockOrNull8.GetDistance(EDirFlags.SOUTH) > 0 || blockOrNull8.GetDistance(EDirFlags.EAST) > 0))
                {
                  blockOrNull7.SetDistance(EDirFlags.SOUTHEAST, 1);
                }
                else
                {
                  int distance = blockOrNull8.GetDistance(EDirFlags.SOUTHEAST);
                  if (distance > 0)
                    blockOrNull7.SetDistance(EDirFlags.SOUTHEAST, distance + 1);
                  else
                    blockOrNull7.SetDistance(EDirFlags.SOUTHEAST, distance - 1);
                }
              }
            }
          }
        }
      }
    }

    private bool IsWalkable(in Int2 p) => this.IsInBoundary(in p) && this.BlockLUT[p.Y, p.X] >= 0;

    private bool IsInBoundary(in Int2 p)
    {
      return 0 <= p.X && p.X < this.Width && 0 <= p.Y && p.Y < this.Height;
    }

    private JPSPlusMapBakerBlock GetBlockOrNull(in Int2 p)
    {
      if (!this.IsInBoundary(in p))
        return (JPSPlusMapBakerBlock) null;
      int index = this.BlockLUT[p.Y, p.X];
      return index < 0 ? (JPSPlusMapBakerBlock) null : this.Blocks[index];
    }
  }
}
