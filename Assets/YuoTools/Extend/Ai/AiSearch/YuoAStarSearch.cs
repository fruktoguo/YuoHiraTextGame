using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuoTools;
using YuoTools.Extend.Helper;
using YuoTools.Extend.YuoMathf;

public class YuoAStarSearch
{
    public int MapSizeX;
    public int MapSizeY;

    public YuoGrid[,] Map;

    public YuoAStarSearch(int[,] map)
    {
        SetMap(map);
    }

    void SetMap(int[,] map)
    {
        MapSizeX = map.GetLength(0);
        MapSizeY = map.GetLength(1);
        Map = new YuoGrid[MapSizeX, MapSizeY];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                var grid = new YuoGrid
                {
                    X = x,
                    Y = y,
                    CanMove = map[x, y] == 0
                };
                Map[x, y] = grid;
            }
        }
    }

    void ResetMap()
    {
        foreach (var item in Map)
        {
            item.Close = false;
            item.Open = false;
            item.F = 0;
            item.H = 0;
            item.Parent = null;
        }

        openQueue.Clear();
    }

    private bool Search(int startX, int startY, int targetX, int targetY)
    {
        ResetMap();
        YuoGrid startYuoGrid = Map[startX, startY];
        YuoGrid endYuoGrid = Map[targetX, targetY];
        Min = startYuoGrid;
        Open(startYuoGrid, null, endYuoGrid);
        StopwatchHelper.Start();
        int maxSearchNum = MapSizeX * MapSizeY * 100;

        while (openQueue.Count > 0)
        {
            maxSearchNum--;
            if (maxSearchNum < 0)
            {
                Debug.LogError("运行超时");
                return false;
            }

            FindNeighbors(Min, endYuoGrid);
            Close(Min);
            if (endYuoGrid.Open)
            {
                var ms = StopwatchHelper.Stop();
                Debug.Log($"运行结束,总共计算{MapSizeX * MapSizeY * 100 - maxSearchNum}次,耗时{ms}毫秒");
                return true;
            }
        }

        StopwatchHelper.Stop();
        return false;
    }

    public List<YuoVector2Int> Search(YuoVector2Int start, YuoVector2Int end)
    {
        if (start.x < 0 || start.x >= MapSizeX || start.y < 0 || start.y >= MapSizeY)
        {
            Debug.LogError("请输入正确的起止点");
        }

        if (!Search(start.x, start.y, end.x, end.y))
        {
            Debug.LogError("无法找到路径");
            return new();
        }

        var endNode = Map[end.x, end.y];
        List<YuoVector2Int> path = new();
        while (endNode.Parent != null)
        {
            endNode = endNode.Parent;
            path.Add(new(endNode.X, endNode.Y));
        }

        return path;
    }


    private void FindNeighbors(YuoGrid grid, YuoGrid end)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (!(grid.X + x).InRange(0, MapSizeX - 1) || !(grid.Y + y).InRange(0, MapSizeY - 1))
                    continue;
                var gridTemp = Map[grid.X + x, grid.Y + y];
                if (!gridTemp.CanMove)
                    continue;
                if (gridTemp.Open || gridTemp.Close)
                    continue;
                if (x != y && x != -y)
                {
                    Open(gridTemp, grid, end);
                }
                else if (true)
                {
                    //Open(gridTemp);
                }
            }
        }
    }

    public YuoGrid Min;

    public void Open(YuoGrid grid, YuoGrid parent, YuoGrid end)
    {
        if (grid.Open == false)
        {
            openQueue.Enqueue(grid);
        }

        grid.OpenGrid(parent, end);
        grid.Open = true;
        grid.Close = false;
        if (grid.F < Min.F)
        {
            Min = grid;
        }
    }

    private Queue<YuoGrid> openQueue = new Queue<YuoGrid>();

    void Close(YuoGrid grid)
    {
        if (grid.Open == true)
        {
            openQueue.Dequeue();
            if (openQueue.Count > 0)
            {
                Min = openQueue.Peek();
            }

            grid.Close = true;
        }
    }

    [System.Serializable]
    public class YuoGrid
    {
        public int X;
        public int Y;

        public bool CanMove;

        public bool Open;

        public bool Close;

        public int F = 0;
        public int G = 0;
        public int H = 0;

        public YuoGrid Parent;

        public void OpenGrid(YuoGrid parent, YuoGrid target)
        {
            if (parent == null)
                return;
            Parent = parent;
            int wi = Mathf.Abs(X - parent.X);
            int he = Mathf.Abs(Y - parent.Y);
            if (wi == 1 && he == 1)
            {
                G = parent.G + 14;
            }
            else
                G = parent.G + 10;

            H = (Mathf.Abs(X - target.X) + Mathf.Abs(Y - target.Y)) * 10;
            F = G + H;
        }
    }
}