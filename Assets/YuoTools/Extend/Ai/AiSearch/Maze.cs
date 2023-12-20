using System.Collections.Generic;
using UnityEngine;
using YuoTools.Extend.MathFunction;

namespace YuoTools
{
    public class MazeGenerator
    {
        private static int row; //迷宫的行
        private static int col; //迷宫的列
        private static int totalPointsCount;
        private static int pointsCount;
        private static YuoInt2 startPoint; //迷宫的起点坐标
        private static YuoInt2 endPoint; //迷宫的终点坐标
        private static int[,] mapList; //迷宫的二维数组

        enum Direction
        {
            Up = 1,
            Right,
            Down,
            Left
        }

        //0代表未标记点
        //1代表墙
        //2代表已标记点
        public static int[,] GenerateMap(int width, int height, YuoInt2 startPointer, YuoInt2 endPointer)
        {
            var w = (width + 1) / 2;
            var h = (height + 1) / 2;
            totalPointsCount = w * h;
            row = w * 2 + 1;
            col = h * 2 + 1;
            mapList = new int[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        mapList[i, j] = 1;
                    }
                    else
                    {
                        mapList[i, j] = 0;
                    }
                }
            }

            startPoint = startPointer + 1;
            endPoint = endPointer - 1;
            pointsCount = 1;
            mapList[startPoint.x, startPoint.y] = 2;
            Dfs(startPoint);
            
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (mapList[i, j] == 1)
                    {
                        mapList[i, j] = 1;
                    }
                    else
                    {
                        mapList[i, j] = 0;
                    }
                }
            }
            mapList[startPoint.x, startPoint.y] = 0;
            mapList[endPoint.x, endPoint.y] = 0;
            
            return mapList;
        }

        private static void Dfs(YuoInt2 currentPoint)
        {
            if (pointsCount == totalPointsCount || currentPoint == endPoint)
            {
                return;
            }

            List<int> accessDir = new List<int>();
            while (true)
            {
                HasAccess(accessDir, currentPoint); //获得当前点可向哪几个方向扩展
                if (accessDir.Count > 0)
                {
                    int randomIndex = Random.Range(0, accessDir.Count); // 随机选择一个方向
                    YuoInt2 nextPoint = OpenUp(currentPoint, (Direction)accessDir[randomIndex]); //打通墙
                    pointsCount++;
                    accessDir.RemoveAt(randomIndex);
                    Dfs(nextPoint);
                }
                else
                {
                    break;
                }
            }
        }

        private static void HasAccess(List<int> dirList, YuoInt2 currentPoint)
        {
            dirList.Clear();
            for (int i = (int)Direction.Up; i <= (int)Direction.Left; i++)
            {
                YuoInt2 neighborPoint = GetNeighborPoint(currentPoint, (Direction)i);
                if (neighborPoint.x > 0 && neighborPoint.x < row)
                {
                    if (neighborPoint.y > 0 && neighborPoint.y < col)
                    {
                        if (mapList[neighborPoint.x, neighborPoint.y] == 0)
                        {
                            dirList.Add(i);
                        }
                    }
                }
            }
        }

        private static YuoInt2 GetNeighborPoint(YuoInt2 currentPoint, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new YuoInt2(currentPoint.x, currentPoint.y - 2);
                case Direction.Right:
                    return new YuoInt2(currentPoint.x + 2, currentPoint.y);
                case Direction.Down:
                    return new YuoInt2(currentPoint.x, currentPoint.y + 2);
                case Direction.Left:
                    return new YuoInt2(currentPoint.x - 2, currentPoint.y);
                default:
                    return new YuoInt2(0, 0);
            }
        }

        private static YuoInt2 OpenUp(YuoInt2 currentPoint, Direction direction)
        {
            YuoInt2 nextPoint = GetNeighborPoint(currentPoint, direction);
            mapList[nextPoint.x, nextPoint.y] = 2;
            switch (direction)
            {
                case Direction.Up:
                    mapList[currentPoint.x, currentPoint.y - 1] = 2;
                    break;
                case Direction.Right:
                    mapList[currentPoint.x + 1, currentPoint.y] = 2;
                    break;
                case Direction.Down:
                    mapList[currentPoint.x, currentPoint.y + 1] = 2;
                    break;
                case Direction.Left:
                    mapList[currentPoint.x - 1, currentPoint.y] = 2;
                    break;
                default:
                    break;
            }

            return nextPoint;
        }
    }
}