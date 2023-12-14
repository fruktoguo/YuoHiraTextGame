using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AStar_Yuri_bk0717
{
    /// <summary>
    /// 挂载到脚本上，根据地图属性，生成地图；显示地图属性
    /// </summary>
    public class MapMeshCreate : MonoBehaviour
    {
        /// <summary>
        /// 地图范围，长宽的属性
        /// </summary>
        [Serializable]
        public class MeshRange
        {
            public int horizontal;
            public int vertical;
        }

        /// <summary>
        /// 一些显示在脚本上，需要用户初始化的UI
        /// </summary>
        [Header("地图范围，长宽：")]
        public MeshRange meshRange;
        [Header("地图起始点：")]
        public Transform startPoint;
        private Vector3 startPos;
        [Header("创建地图网格的，父节点--原点：")]
        public Transform parentTrans;
        [Header("地图格子的预制体：")]
        public GameObject p_Prefab;
        [Header("地图模板大小：")]
        public Vector2 scale;

        private void Start()
        {
            startPos = startPoint.position;
        }
        

        //格子数组
        private GameObject[,] m_Points;
        //取到数组的方法；
        public GameObject[,] points
        {
            get
            {
                return m_Points;
            }
        }
        //取到生成预制体方块上挂载的脚本Point的数组
        public Point[,] m_Popo;
        public Point[,] popo
        {
            get
            {
                return m_Popo;
            }
        }

        //注册模板事件 -- 监听方块的事件
        public Action<GameObject, int, int> PointEvent;

        /// <summary>
        /// 基于挂载物体的初始化数据，创建的地图
        /// </summary>
        public void CreateMap()
        {
            //1.如果地图范围数据有一个没有，则创建不了地图，直接返回
            if(meshRange.horizontal == 0 || meshRange.vertical == 0)
            {
                return;
            }
            //2.清除原来的地图。
            ClearMap();           

            //3.初始化格子数组 -- 根据地图属性
            m_Points = new GameObject[meshRange.horizontal, meshRange.vertical];
            m_Popo = new Point[meshRange.horizontal, meshRange.vertical];
            //4.开始生成地图
            for(int i = 0; i < meshRange.horizontal; i++)
            {
                for(int j = 0; j < meshRange.vertical; j++)
                {
                    //创建一个点，把创建的对象返回下，为了生成后面的Point
                    CreatePoint(i, j);                    
                }
            }
        }
        /// <summary>
        /// 重载函数 基于传入的宽高来生成地图
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">地图高</param>
        public void CreateMap(int width,int height)
        {
            //1.做判断
            if(width == 0 || height == 0)
            {
                return;
            }

            ClearMap();
            m_Points = new GameObject[width, height];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    CreatePoint(i, j);
                }
            }

        }

        /// <summary>
        /// 删除地图，清空缓存
        /// </summary>
        public void ClearMap()
        {
            if (m_Points == null || m_Points.Length == 0)
            {
                return; //说明没有地图了
            }
            //清空地图
            foreach(GameObject go in m_Points)
            {
                if(go != null)
                {
                    Destroy(go);
                }
            }
            //清空缓存 -- 数组
            Array.Clear(m_Points, 0, m_Points.Length);
            //Array.Clear(m_Popo, 0, m_Popo.Length);
        }

        /// <summary>
        /// 根据点，在网格上的X,Z坐标
        /// </summary>
        /// <param name="row">X轴坐标 -- 行</param>
        /// <param name="column">Z轴坐标 -- 列</param>
        public void CreatePoint(int row,int column)
        {
            //1.先实例化一个物体出来，根据确定的预制体，并把它挂载到原点下
            GameObject go = GameObject.Instantiate(p_Prefab, parentTrans);
            
            //2.初始化它的坐标，计算它的全局坐标
            //2.1 起始点 + 模板 * x轴坐标（局部坐标）
            float posx = startPos.x + scale.x * row;
            float posz = startPos.z + scale.y * column;
            go.transform.position = new Vector3(posx, startPos.y, posz);

            //3.把方块，放进点数组中，
            m_Points[row, column] = go;
            //3.1取到方块上挂载的脚本Point,把脚本的X,Z改一下
            go.gameObject.GetComponent<Point>().X = row;
            go.gameObject.GetComponent<Point>().Z = column;
            //3.4 把方块上挂载的脚本Point，放进数组
            m_Popo[row, column] = go.gameObject.GetComponent<Point>();
            
            //4. 建立方块上的事件
            PointEvent?.Invoke(go, row, column);                      
        }
    }
}