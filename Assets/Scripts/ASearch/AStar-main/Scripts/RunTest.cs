using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuoTools;
using YuoTools.Extend.MathFunction;

namespace AStar_Yuri_bk0717
{
    /// <summary>
    /// 此脚本，主要用于整个算法实例展示的，逻辑层
    /// </summary>
    public class RunTest : MonoBehaviour
    {
        //获取地图创建脚本
        public MapMeshCreate mapMeshCreate;

        //网格生成障碍的频率
        [Header("障碍生成频率：")] [Range(0, 1)] public float probability;

        bool isStartFind = false; //是否开始寻路

        //记录鼠标点击次数
        public int clickNum = 0;

        //寻路起始点，终点的Point对象，好传入寻路算法
        Point start;
        Point end;

        /// <summary>
        /// 按下空格则生成（重置）地图，按下S键则开始寻路算法
        /// </summary>
        void Update()
        {
            //按下空格
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Run();
                //isCreateMap = true;
                //按下空格，则清空计数
                clickNum = 0;
            }

            //按下S键 -- 开始寻路算法，说明此时
            if (isStartFind == true && Input.GetKeyDown(KeyCode.S))
            {
                //按下，开始寻路，则把clickNum也清0，用处不大，仅作完善
                clickNum = 0;
                //Debug.Log("here45-run");

                //1.实例封装算法类 -- AStarWrapper
                int[,] map = new int[mapMeshCreate.meshRange.horizontal, mapMeshCreate.meshRange.vertical];
                for (var x = 0; x < mapMeshCreate.m_Popo.GetLength(0); x++)
                {
                    for (var y = 0; y < mapMeshCreate.m_Popo.GetLength(1); y++)
                    {
                        map[x, y] = mapMeshCreate.m_Popo[x, y].IsWall ? 1 : 0;
                        map[x, y].Log();
                    }
                }

                // YuoAStarSearch yuoAStarSearch = new YuoAStarSearch(map);
                // var result =
                //     yuoAStarSearch.Search(new YuoVector2Int(start.X, start.Z), new YuoVector2Int(end.X, end.Z));
                //
                // foreach (var yuoVector2Int in result)
                // {
                //     var point = mapMeshCreate.m_Popo[yuoVector2Int.x, yuoVector2Int.y];
                //     point.ChangeColor(Color.green);
                // }

                AStarWrapper aStarWrapper = new AStarWrapper();
                //2. 调用算法初始化
                aStarWrapper.Init(mapMeshCreate, start, end);
                //3. 开启线程
                StartCoroutine(aStarWrapper.OnStart());

                isStartFind = false;
            }
        }

        public YuoInt2 MapSize = new(10, 10);

        /// <summary>
        /// 运行，调用mapMeshCreate的生成地图
        /// </summary>
        private void Run()
        {
            //建立委托事件绑定
            mapMeshCreate.PointEvent = PointEvent;
            mapMeshCreate.CreateMap();
        }

        private int[,] map;

        /// <summary>
        /// 重载mapMeshCreate的pointEvent执行方法，通过委托传入
        /// </summary>
        /// <param name="go"></param>
        /// <param name="row"></param>
        /// <param name="row"></param>
        private void PointEvent(GameObject go, int row, int column)
        {
            //1.下面要随机生成障碍了，先取到方块的point组件
            Point point = go.GetComponent<Point>();
            int x = point.X;
            int y = point.Z;
            int r = map[x, y];
            //2. 根据障碍生成频率，来确定当前这个物体是否改颜色
            // float f = Random.Range(0, 1.0f);
            Color color = r == 1 ? Color.red : Color.white;
            //3. 初始化对象
            point.ChangeColor(color);
            // point.IsWall = f <= probability;
            point.IsWall = r == 1;
            point.X = row;
            point.Z = column;

            //接下来检测点击事件，模板点击事件
            point.OnClick = () =>
            {
                if (point.IsWall)
                {
                    //如果方块是障碍，直接返回不做处理
                    return;
                }

                //点击次数加1；
                clickNum++;

                Debug.Log(point.X);
                Debug.Log("是否为障碍:" + point.IsWall.ToString());
                //点第一次说明是起点，第二次说明是终点。
                switch (clickNum)
                {
                    case 1:
                        start = point;
                        point.ChangeColor(Color.yellow);
                        break;
                    case 2:
                        end = point;
                        point.ChangeColor(Color.yellow);
                        isStartFind = true; //按下两个键就可以开始寻路
                        break;
                    default:
                        isStartFind = true; //按下两个键就可以开始寻路
                        break;
                }
            };
        }
    }
}