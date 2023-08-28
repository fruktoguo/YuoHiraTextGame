using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace YuoTools
{
    public class YuoJpsSearch : MonoBehaviour
    {
        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="now"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dir ComputeDir(int nowX, int nowY, int endX, int endY)
        {
            int x = endX - nowX;
            int y = endY - nowY;
            if (x.RAbs() >= y.RAbs())
            {
                if (x >= 0)
                {
                    return Dir.Right;
                }
                else
                {
                    return Dir.Left;
                }
            }
            else
            {
                if (y >= 0)
                {
                    return Dir.Up;
                }
                else
                {
                    return Dir.Down;
                }
            }
        }

        private int[,] _Map = new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        public YuoGrid[,] Map;
        public int MapSizeX;
        public int MapSizeY;

        public Vector2Int StartPos;

        public GameObject tempGo;

        private void Init()
        {
            MapSizeX = _Map.GetLength(0);
            MapSizeY = _Map.GetLength(1);
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
                    var go = Instantiate(tempGo, transform);
                    go.transform.localPosition = new Vector3(x, y);
                    var sr = go.GetComponent<SpriteRenderer>();
                    grid.sr = sr;
                    sr.color = !grid.CanMove ? Color.red : Color.black;
                    go.AddComponent<GridTest>().grid = grid;
                }
            }

            JumpPoints = new List<YuoGrid>();
        }

        private void Start()
        {
            Init();
            foreach (var item in Map)
            {
                ComputeJump(item);
            }

            //YuoGrid last = null;
            //foreach (var item in JumpPoints)
            //{
            //    if (last != null)
            //        last = item;
            //}
            Connect();
            FindJump(StartPos);
        }

        private void FindJump(Vector2Int start)
        {
            GetGrid(start).sr.color = Color.white;
        }

        private void ComputeJump(YuoGrid grid)
        {
            //只有墙体附近可以跳跃
            if (grid.CanMove) return;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    //去除掉自己
                    if (x == 0 && y == 0)
                        continue;
                    //斜方向
                    if (x != 0 && y != 0)
                    {
                        //如果斜方向的两个相邻直线方向可以通过
                        if (CanMove(grid.x + x, grid.y + y) && CanMove(grid.x + x, grid.y) &&
                            CanMove(grid.x, grid.y + y))
                        {
                            SetJump(Map[grid.x + x, grid.y + y], x, y);
                        }
                    }
                }
            }
        }

        private void SetJump(YuoGrid grid, int x, int y)
        {
            grid.CanJump = true;
            if (!JumpPoints.Contains(grid))
            {
                //print("添加了一个跳点");
                JumpPoints.Add(grid);
                grid.sr.color = Color.green;
            }

            grid.jump.AddDir(x == -1 ? Dir.Right : Dir.Left);
            grid.jump.AddDir(y == -1 ? Dir.Up : Dir.Down);
            foreach (var item in grid.jump.Dirs)
            {
                if (item == Vector2Int.up)
                    grid.sr.transform.GetChild(2).gameObject.SetActive(true);
                if (item == Vector2Int.down)
                    grid.sr.transform.GetChild(3).gameObject.SetActive(true);
                if (item == Vector2Int.left)
                    grid.sr.transform.GetChild(0).gameObject.SetActive(true);
                if (item == Vector2Int.right)
                    grid.sr.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public class GridTest : MonoBehaviour
        {
            public YuoGrid grid;
        }

        public List<YuoGrid> JumpPoints;
        public Vector2Int[] Dirs = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        public async void Connect()
        {
            foreach (var item in JumpPoints)
            {
                //foreach (var dir in item.jump.Dirs)
                foreach (var dir in Dirs)
                {
                    var Dir = dir;
                    Dir *= -1;
                    //上下方向的就往左右找,左右方向的就往上下找
                    Vector2Int nextDir1;
                    Vector2Int nextDir2;
                    if (Dir.x == 0)
                    {
                        nextDir1 = Vector2Int.left;
                        nextDir2 = Vector2Int.right;
                    }
                    else
                    {
                        nextDir1 = Vector2Int.up;
                        nextDir2 = Vector2Int.down;
                    }

                    List<Branch> branches = new List<Branch>();
                    Branch MainBranch = new Branch() { dir = Dir, pos = new Vector2Int(item.x, item.y) };
                    bool over = false;
                    while (!over)
                    {
                        YuoGrid grid = null;
                        if (!MainBranch.Stop)
                        {
                            grid = GetGrid(MainBranch.Next());
                            if (grid == null)
                            {
                                MainBranch.Stop = true;
                            }
                        }

                        if (!MainBranch.Stop)
                        {
                            if (!grid.CanMove)
                            {
                                MainBranch.Stop = true;
                            }
                            else
                            {
                                branches.Add(new Branch() { dir = nextDir1, pos = MainBranch.pos });
                                branches.Add(new Branch() { dir = nextDir2, pos = MainBranch.pos });
                            }

                            if (grid.CanJump)
                            {
                                await YuoWait.WaitTimeAsync(0.1f);
                                DebugConnect(item, grid);
                                over = true;
                            }
                        }
                        else
                        {
                            bool JumpOut = true;
                            foreach (var b in branches)
                            {
                                if (!b.Stop)
                                {
                                    JumpOut = false;
                                }
                            }

                            if (JumpOut)
                            {
                                break;
                            }
                        }

                        foreach (var b in branches)
                        {
                            if (b.Stop) continue;
                            var _grid = GetGrid(b.Next());
                            if (_grid == null)
                            {
                                b.Stop = true;
                                continue;
                            }

                            if (!_grid.CanMove)
                            {
                                b.Stop = true;
                            }

                            if (_grid.CanJump)
                            {
                                await YuoWait.WaitTimeAsync(0.1f);
                                DebugConnect(item, _grid);
                                over = true;
                            }
                        }
                    }
                }
            }
        }

        private class Branch
        {
            public Vector2Int pos;
            public Vector2Int dir;
            public bool Stop;

            public Vector2Int Next()
            {
                pos += dir;
                return pos;
            }
        }

        private void DebugConnect(YuoGrid start, YuoGrid end)
        {
            start.sr.color = Color.yellow;
            var offset = Random.Range(0, 0f);
            Debug.DrawLine(start.sr.transform.position + Vector3.one * offset,
                end.sr.transform.position + Vector3.one * offset, Color.white, 9999);
        }

        private static Vector2Int GetDir(Dir dir)
        {
            switch (dir)
            {
                case Dir.Up:
                    return Vector2Int.up;

                case Dir.Down:
                    return Vector2Int.down;

                case Dir.Left:
                    return Vector2Int.left;

                case Dir.Right:
                    return Vector2Int.right;

                default:
                    return Vector2Int.zero;
            }
        }

        public YuoGrid GetGrid(int x, int y)
        {
            if (x < 0 || x >= MapSizeX || y < 0 || y >= MapSizeY)
                return null;
            return Map[x, y];
        }

        public YuoGrid GetGrid(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= MapSizeX || pos.y < 0 || pos.y >= MapSizeY)
                return null;
            return Map[pos.x, pos.y];
        }

        private bool CanMove(int x, int y)
        {
            if (!(x).InRange(0, MapSizeX - 1) || !(y).InRange(0, MapSizeY - 1))
                return false;
            return Map[x, y].CanMove;
        }

        #region 类型

        public enum Dir
        {
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
        }

        [System.Serializable]
        public class JumpPoint
        {
            public void AddDir(Dir dir)
            {
                if (!ContainsDir(dir))
                {
                    Dirs.Add(GetDir(dir));
                    dirs += dir.GetHashCode();
                }
            }

            public bool ContainsDir(Dir dir)
            {
                return (dirs & dir.GetHashCode()) != 0;
            }

            public List<YuoGrid> Connects = new List<YuoGrid>();
            public List<Vector2Int> Dirs = new List<Vector2Int>();
            private int dirs = 0;
        }

        [System.Serializable]
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
            internal bool CanJump;
            public JumpPoint jump = new JumpPoint();

            [HideInInspector] public SpriteRenderer sr;
        }

        #endregion 类型
    }
}