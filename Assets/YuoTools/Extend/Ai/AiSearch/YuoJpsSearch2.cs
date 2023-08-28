using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace YuoTools
{
    public class PriorityQueue : IPriorityQueue<JpsNode>
    {
        private readonly SortedDictionary<int, Queue<JpsNode>> _queues = new SortedDictionary<int, Queue<JpsNode>>();

        public bool NotEmpty => _queues.Count > 0;

        public JpsNode Get()
        {
            if (_queues.Count == 0)
            {
                ("队列为空").LogError();
            }

            var queue = _queues.First().Value;
            var item = queue.Dequeue();
            if (queue.Count == 0)
            {
                _queues.Remove(_queues.First().Key);
            }

            return item;
        }

        public void Set(JpsNode obj, int priority)
        {
            if (!_queues.TryGetValue(priority, out var queue))
            {
                queue = new Queue<JpsNode>();
                _queues.Add(priority, queue);
            }

            queue.Enqueue(obj);
        }

        public void Clear()
        {
            _queues.Clear();
        }
    }

    public class YuoJpsSearch2 : MonoBehaviour
    {
        public int MapSizeX;
        public int MapSizeY;

        public Vector2Int StartPos;

        public GameObject tempGo;

        public TestMap Map = new TestMap();

        private void Awake()
        {
            Map.Init();
            MapSizeX = Map.Size.x;
            MapSizeY = Map.Size.y;
            for (int x = 0; x < MapSizeX; x++)
            {
                for (int y = 0; y < MapSizeY; y++)
                {
                    var go = Instantiate(tempGo, transform);
                    go.transform.localPosition = new Vector3(x, y);
                    var sr = go.GetComponent<SpriteRenderer>();
                    sr.color = !Map.IsWall(new Vector2Int(x, y)) ? Color.red : Color.black;
                    Map.Grids[y, x].go = go;
                }
            }

            Jps jps = new Jps();
            jps.nodes = new PriorityQueue();
            var paths = jps.Find(Map, StartPos, new Vector2Int(10, 10));
            if (paths == null) return;
            // 将路径颜色改为绿色
            foreach (var path in paths)
            {
                Map.Grids[path.x, path.y].go.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public class TestMap : IGrid
    {
        private int[,] _map = new int[,]
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

        public class Grid
        {
            public Vector2Int Pos;
            public bool IsMovable;
            public bool IsWall;
            public GameObject go;
        }

        public Vector2Int Size = new Vector2Int(17, 27);
        public Grid[,] Grids = new Grid[17, 27];

        public void Init()
        {
            Size = new Vector2Int(_map.GetLength(1), _map.GetLength(0));
            Grids = new Grid[Size.y, Size.x];
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Grids[y, x] = new Grid
                    {
                        Pos = new Vector2Int(x, y),
                        IsMovable = false,
                        IsWall = _map[x, y] == 1,
                    };
                }
            }
        }

        public bool IsMovable(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size.x || pos.y < 0 || pos.y >= Size.y)
            {
                return false;
            }

            return Grids[pos.x, pos.y].IsMovable;
        }

        public bool IsWall(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Size.x || pos.y < 0 || pos.y >= Size.y)
            {
                return false;
            }

            return Grids[pos.x, pos.y].IsWall;
        }
    }
}