using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace YuoTools
{
    public class YuoBStarSearch2 : SerializedMonoBehaviour
    {
        public class YuoGrid
        {
            public int x;
            public int y;
            public bool CanMove;
            public int f = 0;
            public int g = 0;
            public int h = 0;
            public bool Tag;
            public int Tag1;
            public YuoGrid Parent;
            public bool IsMoved;

            public void InitGrid(YuoGrid parent)
            {
                if (parent == null)
                    return;
                IsMoved = true;
                Parent = parent;
                if (parent == this)
                {
                    this.Parent = null;
                }
                int wi = Mathf.Abs(x - parent.x);
                int he = Mathf.Abs(y - parent.y);
                if (wi == 1 && he == 1)
                {
                    g = parent.g + 14;
                }
                else
                    g = parent.g + 10;
                h = (int)Mathf.Sqrt(wi * wi + he * he) * 10;
                f = g + h;
            }
        }

        [HideInInspector]
        private int[,] _Map = new int[,] {
            {0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,1,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,0,0,1,0,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        };

        [TableMatrix(DrawElementMethod = "DrawElement", RowHeight = 30)]
        public YuoGrid[,] Map;

#if UNITY_EDITOR

        private static YuoGrid DrawElement(Rect rect, YuoGrid value)
        {
            if (!value.CanMove)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(0, 1, 1));
            }
            if (value.IsMoved)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 0, 0));
                UnityEditor.EditorGUI.TextArea(rect.SetHeight(20), (value.f).ToString());
            }
            if (value.Tag)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 1, 1));
            }
            UnityEditor.EditorGUI.TagField(rect.SetHeight(10), (value.Tag1).ToString());
            return value;
        }

#endif

        public static Vector2Int ComputeDir(Vector2Int now, Vector2Int end)
        {
            var temp = (end - now);
            if (temp.x.RAbs() >= temp.y.RAbs())
            {
                if (temp.x >= 0)
                {
                    temp = Vector2Int.right;
                }
                else
                {
                    temp = Vector2Int.left;
                }
            }
            else
            {
                if (temp.y >= 0)
                {
                    temp = Vector2Int.up;
                }
                else
                {
                    temp = Vector2Int.down;
                }
            }
            return temp;
        }

        private void Init()
        {
            //_Map = new int[20, 20];
            MapSizeX = _Map.GetLength(0);
            MapSizeY = _Map.GetLength(1);
            // for (int x = 0; x < MapSizeX; x++)
            // {
            //     for (int y = 0; y < MapSizeY; y++)
            //     {
            //         //if (x < MapSizeX - 2 && y == MapSizeY / 2)
            //         if (x > 1 && y == MapSizeY / 2 + 2)
            //             _Map[x, y] = 1;
            //         else
            //             _Map[x, y] = 0;
            //     }
            // }
            Map = new YuoGrid[MapSizeX, MapSizeY];
            for (int x = 0; x < _Map.GetLength(0); x++)
            {
                for (int y = 0; y < _Map.GetLength(1); y++)
                {
                    var grid = new YuoGrid();
                    grid.x = x;
                    grid.y = y;
                    grid.CanMove = _Map[x, y] == 0;
                    Map[x, y] = grid;
                }
            }
        }

        /// <summary>
        /// 测试寻路次数
        /// </summary>
        public int num = 100;

        private void Start()
        {
            Init();
            for (int i = 0; i < num; i++)
            {
                Search(5, 1, MapSizeX - 1, MapSizeY - 1);
            }
            var now = Map[MapSizeX - 1, MapSizeY - 1];
            for (int i = 0; i < 1000; i++)
            {
                now.Tag = true;
                if (now.Parent != null)
                {
                    now = now.Parent;
                    now.Tag1 = i;
                }
                else
                {
                    break;
                }
            }
        }

        private void Search(int StartX, int StartY, int TargetX, int TargetY)
        {
            foreach (var item in Map)
            {
                item.f = 0;
                item.g = 0;
                item.h = 0;
                item.IsMoved = false;
                item.Tag = false;
                item.Parent = null;
            }
            SearchEnd = false;
            Move(StartX, StartY, StartX, StartY, TargetX, TargetY, 0);
        }

        private bool SearchEnd = false;

        private bool HasMoved(int x, int y)
        {
            return Map[x, y].IsMoved;
        }

        private bool isEnd(int NowX, int NowY, int EndX, int EndY)
        {
            if ((EndX == NowX) && (EndY == NowY)) return true;
            return false;
        }

        private void Move(int NowX, int NowY, int NextX, int NextY, int EndX, int EndY, int index)
        {
            //结束了
            if (SearchEnd) return;
            //超出范围的
            if (!(NextX).InRange(0, MapSizeX - 1) || !(NextY).InRange(0, MapSizeY - 1)) return;
            //已经走过的路径
            if (HasMoved(NextX, NextY)) return;
            //墙体,不能移动的
            if (!CanMove(NextX, NextY)) return;
            //设置一下值
            Map[NextX, NextY].InitGrid(Map[NowX, NowY]);
            Map[NowX, NowY].Tag1 = index;
            if (isEnd(NextX, NextY, EndX, EndY))
            {
                SearchEnd = true;
                return;
            }
            GoNext(NextX, NextY, EndX, EndY, index);
        }

        public int MapSizeX;
        public int MapSizeY;

        private void GoNext(int NowX, int NowY, int EndX, int EndY, int index)
        {
            print("b算法运行了一次");
            //校准方向
            int addx = EndX <= NowX ? -1 : 1;
            int addy = EndY <= NowY ? -1 : 1;
            if (CanMove(NowX + addx, NowY))
            {
                //默认为往右走,实际会根据方向校准
                //判断哪个方向近
                if ((EndY - NowY).RAbs() > (EndX - NowX).RAbs())
                    Move(NowX, NowY, NowX, NowY + addy, EndX, EndY, index);  //往下走
                else
                    Move(NowX, NowY, NowX + addx, NowY, EndX, EndY, index);  //往右走
                if (!CanMove(NowX, NowY + addy))
                {
                    //下方有障碍,左右分两支
                    Move(NowX, NowY, NowX + addx, NowY, EndX, EndY, index);//往右走
                    Move(NowX, NowY, NowX - addx, NowY, EndX, EndY, index);////往左走
                }
                if (!CanMove(NowX + addx, NowY + addy))
                {
                    //右下方有障碍
                    //右下分两支
                    Move(NowX, NowY, NowX, NowY + addy, EndX, EndY, index + 1);
                    Move(NowX, NowY, NowX + addx, NowY, EndX, EndY, index);
                }
                if (!CanMove(NowX - addx, NowY + addy))
                {
                    //左下方有障碍
                    //左下分两支
                    Move(NowX, NowY, NowX, NowY + addy, EndX, EndY, index + 1);
                    Move(NowX, NowY, NowX - addx, NowY, EndX, EndY, index + 1);
                }
                if (!CanMove(NowX + addx, NowY - addy))
                {
                    //右上方有障碍
                    //右上分两支
                    Move(NowX, NowY, NowX, NowY - addy, EndX, EndY, index + 1);
                    Move(NowX, NowY, NowX + addx, NowY, EndX, EndY, index + 1);
                }
                if (!CanMove(NowX - addx, NowY - addy))
                {
                    //左上方有障碍
                    //左上分两支
                    Move(NowX, NowY, NowX, NowY - addy, EndX, EndY, index + 1);
                    Move(NowX, NowY, NowX - addx, NowY, EndX, EndY, index + 1);
                }
            }
            else
            {
                //右方有障碍,上下分两支
                Move(NowX, NowY, NowX, NowY + addy, EndX, EndY, index + 1);
                Move(NowX, NowY, NowX, NowY - addy, EndX, EndY, index + 1);
            }
        }
        private bool CanMove(int x, int y)
        {
            if (!(x).InRange(0, MapSizeX - 1) || !(y).InRange(0, MapSizeY - 1))
                return false;
            return Map[x, y].CanMove;
        }
    }
}