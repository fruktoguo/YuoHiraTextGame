using Sirenix.OdinInspector;
using Sirenix.Utilities;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using YuoTools;

public class YuoAStarSearch : SerializedMonoBehaviour
{
    //public Vector2Int MapSize = Vector2Int.one * 10;
    public int MapSizeX;

    public int MapSizeY;

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
        if (value.Close)
        {
            UnityEditor.EditorGUI.DrawRect(rect, new Color(1, 0, 0));
            UnityEditor.EditorGUI.TextArea(rect.SetHeight(20), (value.f).ToString());
        }
        return value;
    }

#endif

    private void Init()
    {
        //_Map = new int[50, 50];
        MapSizeX = _Map.GetLength(0);
        MapSizeY = _Map.GetLength(1);
        // for (int x = 0; x < MapSizeX; x++)
        // {
        //     for (int y = 0; y < MapSizeY; y++)
        //     {
        //         //if (x < MapSizeX - 2 && y == MapSizeY / 2)
        //         if (x > 1 && y == MapSizeY / 2)
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

    public List<YuoGrid> Search(Vector2Int startPos, Vector2Int endPos)
    {
        Search(startPos.x, startPos.y, endPos.x, endPos.y);
        YuoGrid End = Map[MapSizeX - 1, MapSizeY - 1];
        if (End.Parent == null)
        {
            return null;
        }
        List<YuoGrid> path = new List<YuoGrid>();
        while (End.Parent != null)
        {
            End = End.Parent;
            path.Add(End);
        }
        return path;
    }

    private void Search(int StartX, int StartY, int TargetX, int TargetY)
    {
        foreach (var item in Map)
        {
            item.Close = false;
            item.Open = false;
            item.f = 0;
            item.h = 0;
            item.Parent = null;
        }
        openQueue.Clear();
        YuoGrid startYuoGrid = Map[StartX, StartY];
        YuoGrid endYuoGrid = Map[TargetX, TargetY];
        Min = startYuoGrid;
        Open(startYuoGrid, null);
        //????·????,???????
        int MaxSearchNum = 100000;
        while (openQueue.Count > 0)
        {
            MaxSearchNum--;
            if (MaxSearchNum < 0)
            {
                Debug.LogError("?????");
                return;
            }
            FindNeighbors(Min);
            Close(Min);
            if (endYuoGrid.Open)
            {
                return;
            }
        }
    }

    /// <summary>
    /// ?????Χ?????Openlist
    /// </summary>
    /// <param name="grid"></param>
    private void FindNeighbors(YuoGrid grid)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                //????????
                if (x == 0 && y == 0)
                    continue;
                //??????????
                if (!(grid.x + x).InRange(0, MapSizeX - 1) || !(grid.y + y).InRange(0, MapSizeY - 1))
                    continue;
                var gridTemp = Map[grid.x + x, grid.y + y];
                //????????
                if (!gridTemp.CanMove)
                    continue;
                if (gridTemp.Open || gridTemp.Close)
                    continue;
                if (x != y && x != -y)
                {
                    Open(gridTemp, grid);
                }
                else if (true)
                {
                    //Open(gridTemp);
                }
            }
        }
    }

    public YuoGrid Min;

    public void Open(YuoGrid grid, YuoGrid parent)
    {
        if (grid.Open == false)
        {
            openQueue.Enqueue(grid);
        }
        grid.OpenGrid(parent);
        grid.Open = true;
        grid.Close = false;
        if (grid.f < Min.f)
        {
            Min = grid;
        }
    }

    private Queue<YuoGrid> openQueue = new Queue<YuoGrid>();

    public void Close(YuoGrid grid)
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

    public int num;

    private void Start()
    {
        Init();
        for (int i = 0; i < num; i++)
        {
            Search(5, 1, MapSizeX - 1, MapSizeY - 1);
        }
        YuoGrid End = Map[MapSizeX - 1, MapSizeY - 1];
        List<YuoGrid> path = new List<YuoGrid>();
        while (End.Parent != null)
        {
            End = End.Parent;
            path.Add(End);
        }
        if (End.Open)
        {
        }
    }

    [System.Serializable]
    public class YuoGrid
    {
        public int x;
        public int y;

        public bool CanMove;

        // ?·???
        public bool Open;

        public bool Close;

        public int f = 0;
        public int g = 0;
        public int h = 0;

        public YuoGrid Parent;

        public void OpenGrid(YuoGrid parent)
        {
            if (parent == null)
                return;
            Parent = parent;
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
}