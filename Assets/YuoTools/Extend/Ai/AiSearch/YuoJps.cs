using System;
using System.Collections.Generic;
using UnityEngine;
using YuoTools;

public interface IGrid
{
    bool IsMovable(Vector2Int pos); // 检查目标点位是否可移动
    bool IsWall(Vector2Int pos); // 检查目标点是否是墙体或者是否不可移动
}

public interface IPriorityQueue<T>
{
    bool NotEmpty { get; }
    T Get();
    void Set(T obj, int priority);
    void Clear();
}


public static class JpsUtils
{
    public static readonly Vector2Int Left = Vector2Int.left;
    public static readonly Vector2Int Right = Vector2Int.right;
    public static readonly Vector2Int Up = Vector2Int.up;
    public static readonly Vector2Int Down = Vector2Int.down;
    public static readonly Vector2Int UpRight = Vector2Int.one;
    public static readonly Vector2Int UpLeft = new Vector2Int(-1, 1);
    public static readonly Vector2Int DownRight = new Vector2Int(1, -1);
    public static readonly Vector2Int DownLeft = new Vector2Int(-1, -1);
    public static Dictionary<Vector2Int, Vector2Int[]> VerticalDirLut;

    public static void Init()
    {
        VerticalDirLut = new Dictionary<Vector2Int, Vector2Int[]>();
        Vector2Int[] horizontalLines = new Vector2Int[] { Left, Right };
        Vector2Int[] verticalLines = new Vector2Int[] { Up, Down };
        VerticalDirLut.Add(Left, verticalLines);
        VerticalDirLut.Add(Right, verticalLines);
        VerticalDirLut.Add(Up, horizontalLines);
        VerticalDirLut.Add(Down, horizontalLines);
    }

    /// <summary> 判断当前方向是否为一个直线方向 </summary>
    public static bool IsLineDir(Vector2Int dir)
    {
        return dir.x * dir.y == 0;
    }

    public static int Manhattan(Vector2Int p1, Vector2Int p2)
    {
        /* 曼哈顿距离 */

        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    public static int Euler(Vector2Int p1, Vector2Int p2)
    {
        /* 欧拉距离 */

        int dx = p1.x - p2.x;
        int dy = p1.y - p2.y;
        return dx * dx + dy * dy;
    }
}

public class Jps
{
    public Dictionary<Vector2Int, JpsNode> Lut => lut;

    private Dictionary<Vector2Int, JpsNode> lut = new();
    public IPriorityQueue<JpsNode> nodes;
    private Vector2Int start;
    private Vector2Int end;
    private IGrid map;

    public Jps()
    {
        JpsUtils.Init();
        // nodes = new IPriorityQueue<JpsNode>();
    }

    public Vector2Int[] Find(IGrid iMap, Vector2Int startPos, Vector2Int endPos)
    {
        this.lut.Clear();
        this.nodes.Clear();
        this.map = iMap;
        this.start = startPos;
        this.end = endPos;

        this.lut.Add(startPos, new JpsNode(startPos, startPos, Array.Empty<Vector2Int>(), 0)); // 直接将起点加入到查找表

        // 起点是一个特殊的跳点，也是唯一一个全方向检测的跳点，其他跳点最多拥有三个方向
        Vector2Int[] dirs = new Vector2Int[]
        {
            JpsUtils.Up,
            JpsUtils.Down,
            JpsUtils.Left,
            JpsUtils.Right,
            JpsUtils.UpLeft,
            JpsUtils.UpRight,
            JpsUtils.DownLeft,
            JpsUtils.DownRight,
        };

        JpsNode jpsNode = new JpsNode(startPos, startPos, dirs, 0);
        nodes.Set(jpsNode, 0);

        int num = 0;
        while (nodes.NotEmpty)
        {
            JpsNode node = nodes.Get();
            if (node.Pos == end) return CompletePath();
            foreach (Vector2Int d in node.Dirs)
            {
                if (JpsUtils.IsLineDir(d))
                {
                    TestLine(node.Pos, d, node.Cost);
                }
                else
                {
                    TestDiagonal(node.Pos, node.Pos, d, node.Cost);
                }
                num++;
            }
        }

        num.Log();
        return null;
    }

    public Vector2Int[] CompletePath()
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Queue<JpsNode> openSet = new Queue<JpsNode>();
        openSet.Enqueue(lut[end]);
        while (openSet.Count > 0)
        {
            JpsNode node = openSet.Dequeue();
            closedSet.Add(node.Pos);
            foreach (Vector2Int pos in node.Parents)
            {
                if (closedSet.Contains(pos)) continue;
                cameFrom.Add(node.Pos, pos);
                if (pos == start) return _trace(cameFrom);
                openSet.Enqueue(lut[pos]);
            }
        }

        return null;
    }

    private Vector2Int[] _trace(Dictionary<Vector2Int, Vector2Int> cameFrom)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        return path.ToArray();
    }

    private void AddPoint(Vector2Int parent, Vector2Int p, Vector2Int[] dirs, int cost)
    {
        /* 追加一个新的跳点 */

        if (lut.TryGetValue(p, out var value))
        {
            value.Parents.Add(parent);
        }
        else
        {
            JpsNode node = new JpsNode(parent, p, dirs, cost);
            lut.Add(p, node);
            nodes.Set(node, cost + JpsUtils.Euler(p, end));
        }
    }

    private void TestDiagonal(Vector2Int parent, Vector2Int p, Vector2Int d, int cost)
    {
        /* 斜向跳跃准备, 主要是检查是否可以跳跃以及当前是否应该搜索跳点 */
        /* 检查x分量和y分量上是否存在碰撞体, 如果都不存在则不寻找强制邻居 */

        // 计算障碍物1和障碍物2的位置
        Vector2Int b1 = new Vector2Int(p.x + d.x, p.y);
        Vector2Int b2 = new Vector2Int(p.x, p.y + d.y);
        if (map.IsMovable(b1))
        {
            if (map.IsMovable(b2))
            {
                /* 情况1，B1和B2均为空，可以移动且本次移动不需要检测斜向的跳点 */
                p += d;
                if (map.IsMovable(p))
                {
                    //新的位置不是障碍物
                    cost++;
                    if (p == end)
                    {
                        AddPoint(parent, p, null, cost);
                        return;
                    }

                    if (DiagonalExplore(p, d, cost))
                    {
                        AddPoint(parent, p, new[] { d }, cost);
                        return;
                    }

                    TestDiagonal(parent, p, d, cost); // 递归该函数
                }
            }
            else
            {
                // 情况3，b1可以移动，而b2不可移动
                p += d;
                if (map.IsMovable(p))
                {
                    cost++;
                    if (p == end)
                    {
                        AddPoint(parent, p, null, cost);
                        return;
                    }

                    List<Vector2Int> dirs = TestForceNeighborsInDiagonal(p, b2, d, Vector2Int.up);
                    if (DiagonalExplore(p, d, cost) || dirs.Count > 0)
                    {
                        dirs.Add(d);
                        AddPoint(parent, p, dirs.ToArray(), cost);
                        return;
                    }

                    TestDiagonal(parent, p, d, cost);
                }
            }
        }
        else
        {
            if (map.IsMovable(b2))
            {
                // 情况4，b2可以移动，而b1不可移动

                p += d;
                if (map.IsMovable(p))
                {
                    cost++;
                    if (p == end)
                    {
                        AddPoint(parent, p, null, cost);
                        return;
                    }

                    List<Vector2Int> dirs = TestForceNeighborsInDiagonal(p, b1, d, Vector2Int.right);
                    if (DiagonalExplore(p, d, cost) || dirs.Count > 0)
                    {
                        dirs.Add(d);
                        AddPoint(parent, p, dirs.ToArray(), cost);
                        return;
                    }

                    TestDiagonal(parent, p, d, cost);
                }
            }
            else
            {
                // 情况2，两者均不可移动，什么都不做
                // code..
            }
        }
    }

    private List<Vector2Int> TestForceNeighborsInDiagonal(Vector2Int x, Vector2Int b, Vector2Int d, Vector2Int mask)
    {
        /* 检查给定地目标点和方向是否存在强制邻居, 该函数只适用于斜向搜索
        只要检测到一边就可以退出函数了，因为只可能存在一边 
        @X: 移动到的点X，
        @B：X点侧边的障碍物
        @D: X - parent 
        @mask: 方向遮罩 */

        List<Vector2Int> directions = new List<Vector2Int>();
        b += d * mask;
        if (map.IsMovable(b))
        {
            directions.Add(b - x);
        }

        return directions;
    }

    private bool DiagonalExplore(Vector2Int p, Vector2Int d, int cost)
    {
        /* 朝着角点的分量方向进行探索 */
        bool _1 = TestLine(p, new Vector2Int(d.x, 0), cost);
        bool _2 = TestLine(p, new Vector2Int(0, d.y), cost);
        return _1 || _2;
    }

    private bool TestLine(Vector2Int parent, Vector2Int d, int cost)
    {
        /* 从当前点p开始沿着直线方向d进行跳跃, 如果遇到了跳点, 则返回真值, 否则返回假值 
        该函数认为节点parent已经被访问过了 */

        Vector2Int p = parent + d;
        while (map.IsMovable(p))
        {
            if (p == end)
            {
                /* 找到终点时将终点加入openSet */
                AddPoint(parent, p, Array.Empty<Vector2Int>(), 0);
                return true;
            }

            cost++;
            List<Vector2Int> directions = TestForceNeighborsInLine(p, d);
            if (directions.Count > 0)
            {
                directions.Add(d);
                AddPoint(parent, p, directions.ToArray(), cost);
                return true;
            }

            p += d;
        }

        return false;
    }

    private List<Vector2Int> TestForceNeighborsInLine(Vector2Int p, Vector2Int d)
    {
        /* 检查给定的目标点和方向是否存在强制邻居, 该函数只适用于横纵搜索 
        @p: 点X
        @d: 方向PX，P为X的父节点*/

        List<Vector2Int> directions = new List<Vector2Int>();
        foreach (Vector2Int vd in JpsUtils.VerticalDirLut[d])
        {
            Vector2Int blockPt = vd + p;
            if (map.IsWall(blockPt) && map.IsMovable(blockPt + d)) directions.Add(vd + d);
        }

        return directions;
    }
}

public class JpsNode
{
    public Vector2Int Pos;
    public List<Vector2Int> Dirs;
    public int Cost;
    public List<Vector2Int> Parents;

    public JpsNode(Vector2Int pos, Vector2Int parent, Vector2Int[] dirs, int cost)
    {
        Pos = pos;
        Dirs = new List<Vector2Int>(dirs);
        Cost = cost;
        Parents = new List<Vector2Int> { parent };
    }
}