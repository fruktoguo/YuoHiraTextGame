using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AStar_Yuri_bk0717
{
    /// <summary>
    /// AStar中 地图上各 “点” 的属性，网格地图的网格
    /// 挂载到方块Prefab上
    /// </summary> 
    public class Point : MonoBehaviour
    {
        public Point Parent { get; set; }   //父亲节点，用于后面倒推形成“路径”

        /// <summary>
        /// G ： 起点到随意一个指定格子的移动耗费
        /// H ： 指定格子，到终点的预计耗费
        /// F = G + H; 总耗费
        /// </summary>
        public float F { get; set; }
        public float G { get; set; }
        public float H { get; set; }

        /// <summary>
        /// 点在相对于地图上的坐标值，我们以X , Z坐标为二维图
        /// 是局部坐标值
        /// </summary>
        public int X { get; set; }
        public int Z { get; set; }

        //这个点，是不是障碍物 -- 属性 
        public bool IsWall { get; set; }

        //事件：是否被点击
        public Action OnClick;  

        //该点，上的游戏物体
        //public GameObject pointgameObject;
        //该 点，在三维空间上的位置
        public Vector3 p_position;

        private void Start()
        {
            p_position = this.gameObject.transform.position;
        }
        /*/// <summary>
        /// 构造函数
        /// 需传入参数：parent、X、Z、gameObject、position
        /// parent、gameObject、position可有默认值
        /// FGH通过计算得到，不需要构造获得
        /// iswall,先默认为false
        /// </summary>
        public Point(int x,int z,GameObject go,Point parent = null)
        {
            this.X = x;
            this.Z = z;
            //this.gameObject = go;
            this.Parent = parent;
            this.p_position = go.transform.position;
            IsWall = false;            
        }*/

        /// <summary>
        /// 更新父节点、更新G 、 F
        /// </summary>
        /// <param name="parent">要变成父节点的点</param>
        /// <param name="g">该新父节点的g</param>
        public void UpdateParent(Point parent,float g)
        {
            this.Parent = parent;
            this.G = g;
            //计算更新F
            F = G + H;
        }

        /// <summary>
        /// 方便对Points更改颜色
        /// </summary>
        /// <param name="color">要更改的颜色</param>
        public void ChangeColor(Color color)
        {
            this.gameObject.GetComponent<MeshRenderer>().material.color = color;
        }

        //委托绑定模板，监听绑定事件，具体实现在RunTest
        public void OnMouseDown()
        {
            OnClick?.Invoke();
        }
    }
}
