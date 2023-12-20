using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using YuoTools.Extend.Helper;
using YuoTools.Extend.MathFunction;

namespace YuoTools
{
    public class YuoBStarSearch
    {
        public class YuoGrid
        {
            public YuoInt2 Pos;
            public bool CanMove;
            public bool IsMoved;
            public YuoGrid Parent;
            public int PathIndex = 0;

            public void Reset()
            {
                Parent = null;
                IsMoved = false;
                PathIndex = 0;
            }

            public void MoveTo(YuoGrid grid)
            {
                grid.Parent = this;
                grid.IsMoved = true;
                grid.PathIndex = PathIndex + 1;
            }
        }

        private YuoGrid[,] map;


        public YuoBStarSearch(int[,] data)
        {
            var mapData = data;
            MapSizeX = mapData.GetLength(0);
            MapSizeY = mapData.GetLength(1);
            map = new YuoGrid[MapSizeX, MapSizeY];
            for (int x = 0; x < mapData.GetLength(0); x++)
            {
                for (int y = 0; y < mapData.GetLength(1); y++)
                {
                    var grid = new YuoGrid
                    {
                        Pos = new YuoInt2(x, y),
                        CanMove = mapData[x, y] == 0
                    };
                    map[x, y] = grid;
                }
            }
        }

        private int maxSearchNum = 1000000;

        public bool Search(int startX, int startY, int targetX, int targetY)
        {
            foreach (var item in map)
            {
                item.Reset();
            }

            maxSearchNum = MapSizeX * MapSizeY * 100;
            searchEnd = false;
            startPoint = new(startX, startY);
            endPoint = new(targetX, targetY);
            StopwatchHelper.Start();
            Move();
            var ms = StopwatchHelper.Stop();
            Debug.Log($"BStar运行{(searchEnd ? "成功" : "失败")},总共计算{MapSizeX * MapSizeY * 100 - maxSearchNum}次,耗时{ms}毫秒");
            return searchEnd;
        }

        public List<YuoInt2> Search(YuoInt2 start, YuoInt2 end)
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

            startPoint = start;
            endPoint = end;
            var endNode = map[end.x, end.y];
            var startNode = map[start.x, start.y];
            List<YuoInt2> path = new();
            while (endNode.Parent != null && endNode != startNode)
            {
                endNode = endNode.Parent;
                path.Add(endNode.Pos);
            }

            return path;
        }

        private bool searchEnd = false;

        private bool HasMoved(int x, int y)
        {
            return map[x, y].IsMoved;
        }

        private bool IsEnd(YuoInt2 point)
        {
            if (endPoint.x == point.x && endPoint.y == point.y) return true;
            return false;
        }

        private YuoInt2 endPoint;
        private YuoInt2 startPoint;

        private void Move()
        {
            Queue<YuoInt2> queue = new Queue<YuoInt2>();
            queue.Enqueue(startPoint);
            map[startPoint.x, startPoint.y].IsMoved = true;
            while (queue.Count > 0)
            {
                var currentPoint = queue.Dequeue();
                YuoGrid current = map[currentPoint.x, currentPoint.y];
                if (IsEnd(current.Pos))
                {
                    searchEnd = true;
                    return;
                }

                GoNext(startPoint, queue);
            }
        }

        void CheckMoveTo(YuoInt2 now, YuoInt2 next, Queue<YuoInt2> queue)
        {
            //不能走的地方
            if (!CanMove(next.x, next.y)) return;
            if (maxSearchNum <= 0) return;
            map[now.x, now.y].MoveTo(map[next.x, next.y]);
            queue.Enqueue(new(next.x, next.y));
        }


        public int MapSizeX;
        public int MapSizeY;

        bool toRight, toLeft, toUp, toDown;

        private void GoNext(YuoInt2 now, Queue<YuoInt2> queue)
        {
            //运行了一次
            maxSearchNum--;
            //校准方向
            int addX = endPoint.x <= now.x ? -1 : 1;
            int addY = endPoint.y <= now.y ? -1 : 1;
            toRight = toLeft = toUp = toDown = false;

            if (CanMove(now.x + addX, now.y))
            {
                //默认为往右走,实际会根据目标点和当前位置校准
                //判断哪个方向近
                if ((endPoint.y - now.y).RAbs() > (endPoint.x - now.x).RAbs())
                    toDown = true;
                else
                    toRight = true;

                if (!CanMove(now.x, now.y + addY))
                {
                    //下方有障碍,左右分两支
                    toRight = true;
                    toLeft = true;
                }

                if (!CanMove(now.x + addX, now.y + addY))
                {
                    //右下方有障碍
                    //右下分两支
                    toRight = true;
                    toDown = true;
                }

                if (!CanMove(now.x - addX, now.y + addY))
                {
                    //左下方有障碍
                    //左下分两支
                    toLeft = true;
                    toDown = true;
                }

                if (!CanMove(now.x + addX, now.y - addY))
                {
                    //右上方有障碍
                    //右上分两支
                    toRight = true;
                    toUp = true;
                }

                if (!CanMove(now.x - addX, now.y - addY))
                {
                    //左上方有障碍
                    //左上分两支
                    toLeft = true;
                    toUp = true;
                }
            }
            else
            {
                //右方有障碍,上下左分三支
                toLeft = true;
                toUp = true;
                toDown = true;
            }

            if (toRight) CheckMoveTo(now, now + YuoInt2.Right, queue);
            if (toLeft) CheckMoveTo(now, now + YuoInt2.Left, queue);
            if (toUp) CheckMoveTo(now, now + YuoInt2.Up, queue);
            if (toDown) CheckMoveTo(now, now + YuoInt2.Down, queue);
        }

        private bool CanMove(int x, int y)
        {
            if (!x.InRange(0, MapSizeX - 1) || !y.InRange(0, MapSizeY - 1))
                return false;
            return map[x, y].CanMove && !map[x, y].IsMoved;
        }
    }
}