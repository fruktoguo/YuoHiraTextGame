using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace YuoTools
{
    public class YuoBStarSearch : SerializedMonoBehaviour
    {
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
            {0,0,1,0,0,1,0,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        };

        [TableMatrix(DrawElementMethod = "DrawElement", RowHeight = 30)]
        public YuoGrid[,] Map;

#if UNITY_EDITOR

        private static YuoGrid DrawElement(Rect rect, YuoGrid value)
        {
            if (!value.CanMove)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 0, 0));
            }
            if (value.IsMoved)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 1, 0));
            }
            if (value.Tag)
            {
                UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 1, 1));
            }
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
        public int num = 1;

        private void Start()
        {
            Init();
            for (int i = 0; i < num; i++)
            {
                Search(5, 1, MapSizeX - 1, MapSizeY - 1);
            }
            for (int i = 0; i < OverBranch.Path.Count; i++)
            {
                OverBranch.Path[i].Tag = true;
            }
        }

        private void Search(int StartX, int StartY, int TargetX, int TargetY)
        {
            foreach (var item in Map)
            {
                item.IsMoved = false;
            }
            BranchList.Clear();
            OverBranch = null;
            SearchEnd = false;
            int MaxSearchNum = MapSizeX * MapSizeY;
            Branch branch = branchPools.GetItem(null);
            branch.Endx = TargetX;
            branch.Endy = TargetY;
            branch.Add(Map[StartX, StartY]);
            BranchList.Add(branch);
            int Count = 0;
            for (int i = 0; i < MaxSearchNum; i++)
            {
                if (SearchEnd)
                {

                    return;
                }
                //步进
                Count = BranchList.Count;
                for (int j = 0; j < Count; j++)
                {
                    GoNext(BranchList[j]);
                }
            }
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (SearchEnd) return;
                int Count = BranchList.Count;
                for (int j = 0; j < Count; j++)
                {
                    GoNext(BranchList[j]);
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (SearchEnd) return;
                int Count = BranchList.Count;
                for (int j = 0; j < Count; j++)
                {
                    GoNext(BranchList[j]);
                }
            }
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

        [HideInInspector]
        public Branch OverBranch;
        public int MapSizeX;
        public int MapSizeY;

        [HideInInspector]
        /// <summary>
        /// 所有的分支
        /// </summary>
        public List<Branch> BranchList = new List<Branch>();
        void MoveTo(Branch branch, int x, int y)
        {
            branch.Add(Map[x, y]);
            if (isEnd(x, y, branch.Endx, branch.Endy))
            {
                SearchEnd = true;
                OverBranch = branch;
                return;
            }
            if (!BranchList.Contains(branch))
            {
                BranchList.Add(branch);
            }
        }

        public void GoNext(Branch branch)
        {
            var now = branch.Now();
            int addx = branch.Endx <= now.x ? -1 : 1;
            int addy = branch.Endy <= now.y ? -1 : 1;
            bool MainBranch = false;
            if (CanMove(now.x + addx, now.y))
            {

                if (!CanMove(now.x + addx, now.y + addy))
                {
                    //右下不能移动 右下分两支
                    Move(branch, now.x, now.y + addy); //下
                    Move(branch, now.x + addx, now.y); //右
                }
                if (!CanMove(now.x + addx, now.y - addx))
                {
                    //右上不能移动 右上分两支
                    Move(branch, now.x + addx, now.y); //右
                    Move(branch, now.x, now.y - addy); //上
                }
                if (!CanMove(now.x - addx, now.y + addy))
                {
                    //左下不能移动 左下分两支
                    Move(branch, now.x, now.y + addy); //下
                    Move(branch, now.x - addx, now.y); //左
                }
                if (!CanMove(now.x - addx, now.y - addy))
                {
                    //左上不能移动 左上分两支
                    Move(branch, now.x, now.y - addy); //上
                    Move(branch, now.x - addx, now.y); //左
                }
                if (!CanMove(now.x, now.y + addy) && !CanMove(now.x, now.y + addy))
                {
                    //上下不能移动 左右分两支
                    Move(branch, now.x + addx, now.y); //右
                    Move(branch, now.x - addx, now.y); //左
                }
                if (MainBranch)
                {
                    //想要不太贴墙就把这个return去掉
                    return;
                }
                //没有任何拐角,继续走
                if (((now.x - branch.Endx).RAbs() > (now.y - branch.Endy).RAbs()))
                    Move(branch, now.x, now.y + addy); //下
                else
                    Move(branch, now.x + addx, now.y); //右
            }
            else
            {
                if (!CanMove(now.x, now.y + addy) && !CanMove(now.x, now.y + addy))
                {
                    //上下不能移动 左右分两支
                    Move(branch, now.x + addx, now.y); //右
                    Move(branch, now.x - addx, now.y); //左
                }
                else
                {
                    //能移动就上下分两支
                    Move(branch, now.x, now.y + addy); //下
                    Move(branch, now.x, now.y - addy); //上
                }
            }

            void Move(Branch branch, int x, int y)
            {
                //结束了
                if (SearchEnd) return;
                //超出范围的
                if (!(x).InRange(0, MapSizeX - 1) || !(y).InRange(0, MapSizeY - 1)) return;
                //已经走过的路径
                if (HasMoved(x, y)) return;
                //墙体,不能移动的
                if (!CanMove(x, y)) return;
                //如果主分支没有被使用
                if (!MainBranch)
                {
                    MainBranch = true;
                    MoveTo(branch, x, y);
                }
                //使用了就创建一个新的分支
                else
                {
                    var bt = branchPools.GetItem(branch);
                    MoveTo(bt, x, y);
                    //MoveTest(new Branch(branch) { StartPos = new Vector2Int(x, y), index = BranchList.Count }, x, y);
                }
            }
        }
        BranchPools branchPools = new BranchPools();
        private bool CanMove(int x, int y)
        {
            if (!(x).InRange(0, MapSizeX - 1) || !(y).InRange(0, MapSizeY - 1))
                return false;
            return Map[x, y].CanMove;
        }
        #region 类型
        public class BranchPools
        {
            private List<Branch> Actives = new List<Branch>();
            private List<Branch> Pools = new List<Branch>();

            public void Remove(Branch item)
            {
                if (Actives.Contains(item))
                {
                    Actives.Remove(item);
                    Pools.Add(item);
                }
                else
                {
                    $"要移除的物体 [{item}] 不属于该对象池".Log("#ff0000");
                }
            }

            /// <summary>
            /// 临时变量
            /// </summary>
            private Branch ItemTemp;

            /// <summary>
            /// 获取一个item
            /// </summary>
            /// <returns></returns>
            public Branch GetItem(Branch parent)
            {
                if (Pools.Count > 0)
                {
                    ItemTemp = Pools[0];
                    Pools.Remove(ItemTemp);
                }
                else
                {
                    ItemTemp = CreatItem();
                }
                Actives.Add(ItemTemp);
                ItemTemp.Init(parent);
                return ItemTemp;
            }

            /// <summary>
            /// 创建新的item
            /// </summary>
            /// <returns></returns>
            public Branch CreatItem()
            {
                return new Branch();
            }
        }
        public class Branch
        {
            /// <summary>
            /// 这条分支的路径
            /// </summary>
            public List<YuoGrid> Path;
            public int Endx;
            public int Endy;
            public Branch()
            {
                Path = new List<YuoGrid>();
            }
            public YuoGrid Now()
            {
                return Path[Path.Count - 1];
            }
            public void Add(YuoGrid grid)
            {
                Path.Add(grid);
                grid.IsMoved = true;
            }
            public void Init(Branch parent)
            {
                if (parent != null)
                {
                    Path = new List<YuoGrid>(parent.Path);
                    Path.RemoveAt(Path.Count - 1);
                    Endx = parent.Endx;
                    Endy = parent.Endy;
                }
                else
                {
                    Path = new List<YuoGrid>();
                }
            }
        }

        public class YuoGrid
        {
            public int x;
            public int y;
            /// <summary>
            /// 显示的时候标记用的,可以删
            /// </summary>
            public bool Tag;
            public bool CanMove;
            public bool IsMoved;
        }

        #endregion

    }
}