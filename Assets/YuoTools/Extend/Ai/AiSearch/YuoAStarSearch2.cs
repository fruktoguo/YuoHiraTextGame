using System.Collections.Generic;
using UnityEngine;
using YuoTools.Extend.YuoMathf;

public class YuoAStarSearch2
{
    private int[,] map;
    private List<Node> openList;
    private List<Node> closeList;

    public YuoAStarSearch2(int[,] map)
    {
        this.map = map;
        openList = new List<Node>();
        closeList = new List<Node>();
    }

    public List<Node> FindPath(YuoVector2Int start, YuoVector2Int end, bool ignoreCorner = true)
    {
        var startNode = new Node(start);
        var endNode = new Node(end);
        openList.Add(startNode);
        int maxSearchNum = map.GetLength(0) * map.GetLength(1) * 100;

        while (openList.Count != 0)
        {
            maxSearchNum--;
            if (maxSearchNum < 0)
            {
                Debug.LogError("运行超时");
                return null;
            }

            var temp = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < temp.f || (openList[i].f == temp.f && openList[i].h < temp.h))
                    temp = openList[i];
            }

            openList.Remove(temp);
            closeList.Add(temp);
            var surroundPoints = SurroundPoints(temp, ignoreCorner);
            foreach (Node point in surroundPoints)
            {
                if (openList.Exists(x => x.pos == point.pos))
                {
                    FoundPoint(temp, point);
                }
                else
                {
                    NotFoundPoint(temp, endNode, point);
                }
            }

            if (openList.Exists(x => x.pos == endNode.pos))
            {
                return GeneratePath(startNode, endNode);
            }
        }

        return null;
    }

    public List<YuoVector2Int> GetAllPathPoints(YuoVector2Int start, YuoVector2Int end, bool ignoreCorner = true)
    {
        List<Node> path = FindPath(start, end, ignoreCorner);
        List<YuoVector2Int> pathPoints = new List<YuoVector2Int>();
        if (path != null)
        {
            foreach (Node node in path)
            {
                pathPoints.Add(node.pos);
            }
        }

        return pathPoints;
    }

    private List<Node> GeneratePath(Node start, Node end)
    {
        var temp = end;
        var path = new List<Node>();
        while (true)
        {
            path.Add(temp);
            if (temp.pos == start.pos)
            {
                break;
            }

            temp = temp.parent;
        }

        path.Reverse();
        return path;
    }

    private void FoundPoint(Node tempStart, Node point)
    {
        var g = CalcG(tempStart, point);
        if (g < point.g)
        {
            point.parent = tempStart;
            point.g = g;
            point.CalcF();
        }
    }

    private void NotFoundPoint(Node tempStart, Node end, Node point)
    {
        point.parent = tempStart;
        point.g = CalcG(tempStart, point);
        point.h = CalcH(end, point);
        point.CalcF();
        openList.Add(point);
    }

    private int CalcG(Node start, Node point)
    {
        var g = Vector2.Distance(start.pos, point.pos) == 1 ? 10 : 14;
        var parentG = point.parent != null ? point.parent.g : 0;
        return g + parentG;
    }

    private int CalcH(Node end, Node point)
    {
        var step = Mathf.Abs(point.pos.x - end.pos.x) + Mathf.Abs(point.pos.y - end.pos.y);
        return (int)step * 10;
    }

    private List<Node> SurroundPoints(Node point, bool ignoreCorner)
    {
        var surroundPoints = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var posX = point.pos.x + x;
                var posY = point.pos.y + y;
                if (CanReach(point.pos.x, point.pos.y, posX, posY, ignoreCorner))
                    surroundPoints.Add(new Node(new Vector2(posX, posY)));
            }
        }

        return surroundPoints;
    }

    private bool CanReach(int x, int y, int targetX, int targetY, bool ignoreCorner)
    {
        if (targetX < 0 || targetY < 0 || targetX >= map.GetLength(0) || targetY >= map.GetLength(1))
            return false;
        if (map[targetX, targetY] != 0)
            return false;
        if (Mathf.Abs(x - targetX) + Mathf.Abs(y - targetY) == 1)
            return true;
        if (map[x, targetY] != 0 && map[targetX, y] != 0)
            return ignoreCorner;
        else
            return true;
    }
}

public class Node
{
    public YuoVector2Int pos;
    public Node parent;
    public int f;
    public int g;
    public int h;

    public Node(YuoVector2Int pos)
    {
        this.pos = pos;
    }

    public void CalcF()
    {
        this.f = this.g + this.h;
    }
}