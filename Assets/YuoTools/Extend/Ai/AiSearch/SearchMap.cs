using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YuoTools
{
    public class SearchMap
    {
        private MapPoint[][] Map;

        public SearchMap(Vector2Int MapSize)
        {
            //初始化地图
            Map = new MapPoint[MapSize.x][];
            for (int x = 0; x < MapSize.x; x++)
            {
                Map[x] = new MapPoint[MapSize.y];
            }
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    Map[x][y].Pos = new Vector2Int(x, y);
                }
            }
        }

        public MapPoint GetPoint(int x, int y)
        {
            return Map[x][y];
        }

        public bool CanMove(int x, int y)
        {
            return !Map[x][y].obstruction;
        }

        public struct MapPoint
        {
            public bool obstruction;
            public int Cost;
            public Vector2Int Pos;
        }
    }
}