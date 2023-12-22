using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using YuoTools;

public class YuoSearchJobsHelper
{
    public static (AStarSearchJob job, NativeList<int2> result, NativeArray<int> resultIndex, IDisposable[] disposables)
        InitJob(int[,] map,
            Vector2Int[] startPos,
            Vector2Int[] endPos)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var nodeMap = new NativeArray<MapNode>(width * height, Allocator.TempJob);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodeMap[x * height + y] = new MapNode(x, y, map[x, y] != 1);
            }
        }

        NativeQueue<MapNode> openQueue = new NativeQueue<MapNode>(Allocator.TempJob);

        var result = new NativeList<int2>(Allocator.TempJob);
        var resultIndex = new NativeArray<int>(startPos.Length, Allocator.TempJob);

        var startPosArray = new NativeArray<int2>(startPos.Length, Allocator.TempJob);
        for (int i = 0; i < startPos.Length; i++)
        {
            startPosArray[i] = new int2(startPos[i].x, startPos[i].y);
        }

        var endPosArray = new NativeArray<int2>(endPos.Length, Allocator.TempJob);
        for (int i = 0; i < endPos.Length; i++)
        {
            endPosArray[i] = new int2(endPos[i].x, endPos[i].y);
        }

        var job = new AStarSearchJob()
        {
            StartPosArray = startPosArray,
            EndPosArray = endPosArray,
            Min = new MapNode(int.MaxValue, int.MaxValue, false),
            OpenQueue = openQueue,
            Result = result,
            ResultIndex = resultIndex,
            NodeMap = nodeMap,
            MapSizeX = map.GetLength(0),
            MapSizeY = map.GetLength(1),
            SearchIndex = 0,
        };
        return (job, result, resultIndex, new IDisposable[] { startPosArray, endPosArray, nodeMap, openQueue, result });
    }
}

public struct MapNode : IEquatable<MapNode>, IComparable<MapNode>
{
    public bool Equals(MapNode other)
    {
        return Pos.x == other.Pos.x && Pos.y == other.Pos.y;
    }

    public int CompareTo(MapNode other)
    {
        return F.CompareTo(other.F);
    }

    public bool CanMove;
    public bool Open;
    public bool Close;

    public int F;
    public int G;
    public int H;

    public int2 Parent;

    public int2 Pos;

    public MapNode(int x, int y, bool canMove)
    {
        Pos = new int2(x, y);
        CanMove = canMove;
        Open = false;
        Close = false;
        F = 0;
        G = 0;
        H = 0;
        Parent = new(int.MinValue, int.MinValue);
    }

    public void Reset()
    {
        Open = false;
        Close = false;
        F = 0;
        G = 0;
        H = 0;
        Parent = new(int.MinValue, int.MinValue);
    }

    public void OpenNode(MapNode parent, int2 target)
    {
        Parent = parent.Pos;
        int wi = Math.Abs(Pos.x - parent.Pos.x);
        int he = Math.Abs(Pos.y - parent.Pos.y);
        if (wi == 1 && he == 1)
        {
            G = parent.G + 14;
        }
        else
            G = parent.G + 10;

        H = (Math.Abs(Pos.x - target.x) + Math.Abs(Pos.y - target.y)) * 10;
        F = G + H;
    }
}

[BurstCompile]
public struct AStarSearchJob : IJob
{
    public NativeArray<MapNode> NodeMap;
    public int MapSizeX;
    public int MapSizeY;
    public NativeQueue<MapNode> OpenQueue;
    public NativeArray<int2> StartPosArray;
    public NativeArray<int2> EndPosArray;
    public MapNode Min;
    public NativeList<int2> Result;
    public NativeArray<int> ResultIndex;
    public int SearchIndex;
    int2 StartPos => StartPosArray[SearchIndex];
    int2 EndPos => EndPosArray[SearchIndex];

    public void Execute()
    {
        foreach (var pos in StartPosArray)
        {
            ExecuteIndex(SearchIndex++);
            ResetMap();
        }
    }

    void ExecuteIndex(int index)
    {
        SearchIndex = index;
        var startNode = GetNode(StartPos);
        var endNode = GetNode(EndPos);
        Min = startNode;
        Open(StartPos, default, false);

        int maxSearchNum = MapSizeX * MapSizeY * 100;

        while (OpenQueue.Count > 0)
        {
            maxSearchNum--;
            if (maxSearchNum < 0)
            {
                Debug.LogError("运行超时");
                return;
            }

            FindNeighbors(Min);
            Close(Min.Pos);
            if (GetNode(EndPos).Open)
            {
                // Debug.Log($"AStar运行成功,总共计算{MapSizeX * MapSizeY * 100 - maxSearchNum}次");
                SetResult();
                return;
            }
        }
    }

    void SetResult()
    {
        NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
        var pos = EndPos;
        while (!pos.Equals(StartPos))
        {
            path.Add(pos);
            pos = GetNode(pos).Parent;
        }

        Result.AddRange(path);
        ResultIndex[SearchIndex] = path.Length;
        path.Dispose();
    }

    void ResetMap()
    {
        foreach (var mapNode in NodeMap)
        {
            mapNode.Reset();
        }
    }

    MapNode GetNode(int2 pos) => NodeMap[pos.x * MapSizeY + pos.y];

    bool InRange(int2 pos) => pos.x.InRange(0, MapSizeX - 1) && pos.y.InRange(0, MapSizeY - 1);

    private void FindNeighbors(MapNode grid)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                var nextPos = grid.Pos + new int2(x, y);
                if (!InRange(nextPos))
                    continue;
                var gridTemp = GetNode(nextPos);
                if (!gridTemp.CanMove)
                    continue;
                if (gridTemp.Open || gridTemp.Close)
                    continue;
                if (x != y && x != -y)
                {
                    Open(gridTemp.Pos, grid);
                }
                else if (true)
                {
                    //Open(gridTemp);
                }
            }
        }
    }

    void Open(int2 pos, MapNode parent, bool hasParent = true)
    {
        var node = GetNode(pos);
        if (node.Open == false)
        {
            OpenQueue.Enqueue(node);
        }

        if (hasParent) node.OpenNode(parent, EndPos);
        node.Open = true;
        node.Close = false;
        if (node.F < Min.F)
        {
            Min = node;
        }

        SetMapNode(pos, node);
    }

    void SetMapNode(int2 pos, MapNode node)
    {
        NodeMap[pos.x * MapSizeY + pos.y] = node;
    }

    void Close(int2 pos)
    {
        var node = GetNode(pos);
        if (node.Open)
        {
            OpenQueue.Dequeue();
            if (OpenQueue.Count > 0)
            {
                Min = OpenQueue.Peek();
            }

            SetMapNode(pos, node);
        }
    }
}