using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar_Yuri_bk0717
{
    /// <summary>
    /// 类作用： 封装 A星算法单例
    /// 实现原理
    /// 1、首先有一张一定宽高的地图 （定义好 Point 点的地图，其中 Point 中有 IsWall 属性）
    /// 2、设定开始点，和目标点
    /// 3、传入 FindPath 开始寻找较短路径，找到返回true，否则 false
    /// 4、为 true 就可以通过 目标点的父亲点的父亲点的父亲点，直到父亲点为开始点，
    ///    这些点集合即是路径
    /// 5、FindPath 寻找原理
    /// 1）开列表，关列表初始化
    /// 2）添加开始点到开列表，然后获得周围点集合，接着又把开始点从开列表中移除，
    ///    并添加到关列表
    /// 3）判断这些周围点集合是否已经在开列表中，不在则更新这些点的F 和 父亲点，
    ///    并添加到开列表；再则重新计算G值，G较小则更新GF 和父亲点
    /// 4）从周围点集合中找到 F 最小的点，然后获得周围点集合，接着又把找到 F 最小的点
    ///    从开列表中移除，并添加到关列表
    /// 5）接着执行第 3） 步骤
    /// 6）直到目标点被添加到开列表中，则路径找到
    /// 7）否则，直到开列表中没有了数据，则说明没有合适路径
    /// </summary>
    class AStarWrapper
    {
        /* 所以此算法 需要如下几个方法： 
         *（总）FindPath：取得较短路径；
         * 总方法涉及的函数：
         * PointFilter：把关闭列表点，从周围集合过滤
           GetSurrondPoint：获取当前最小F周围点
           FindMinOfPoint：开列表所有F，找出最小的F点
           CalG、CalF：计算G、F；H比较简单，不封装。*/

        public MapMeshCreate mapMeshCreate;
        public Point Start;
        public Point End;
        public Point[,] map;
        public int mapWidth;
        public int mapHeight;
        //用一个栈来存放路径点，到时候直接弹出栈同时染色；
        public Stack<Point> resultPoints;
        //开列表：需要考虑的点；关列表：不需考虑的点集合；
        public List<Point> openList = new List<Point>();
        public List<Point> closedList = new List<Point>();

        //若找到了路径，此时从openlist往回把方块染成绿的

        /// <summary>
        /// 仅作算法方法FindPath所需参数，与外界脚本相连的初始化
        /// </summary>
        public void Init(MapMeshCreate mapMesh,Point sta,Point end)
        {
            this.mapMeshCreate = mapMesh;
            map = mapMeshCreate.m_Popo;
            this.Start = sta;
            this.End = end;
            resultPoints = new Stack<Point>();
            openList = new List<Point>();
            closedList = new List<Point>();

            mapWidth = mapMesh.meshRange.horizontal;
            mapHeight = mapMesh.meshRange.vertical;
        }

        /// <summary>
        /// 为了在本案例中为了使得整个寻路过程的步骤可视化，
        /// 使用一个协程来完成寻路过程的方法调用，这样，在每一次完成一格的寻路后，
        /// 可以通过协程来延时执行下一次循环
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnStart()
        {
            bool isHaveResult = FindPath(Start, End, map, mapWidth, mapHeight);
            if(isHaveResult == true)
            {
                yield return new WaitForSeconds(1);
                Traverse();
            }
            else
            {
                yield return new WaitForSeconds(1);
                Debug.Log("找不到最短路径，sorry~");
            }
        }

        /// <summary>
        /// 找到了路径后，根据“就可以通过 目标点的父亲点的父亲点的父亲点，
        ///                 直到父亲点为开始点，这些点集合即是路径”
        /// 来把这些点换颜色
        /// </summary>
        public void Traverse()
        {            
            if(openList.Count == 0)
            {
                //说明没有路径
                return;
            }

            //因为此函数的前置条件是，FindPath返回true，放到启动函数管
            //1.把目标点赋给po
            Point po = openList[openList.IndexOf(End)];
            while(po.Parent != Start)
            {
                //2.po = po的父节点
                po = po.Parent;
                //3.把po的颜色改了
                po.ChangeColor(Color.green);                
                //4.循环结束，直到路径开始的开始点，开始点的父节点为null；
            }
        }

        /// <summary>
        /// 函数作用：取得起点到终点的较短路径--此算法最终目标
        /// </summary>
        /// <param name="start">Point起点</param>
        /// <param name="end">Point终点</param>
        /// <param name="map">Point数组地图</param>
        /// <param name="mapWidth">地图长</param>
        /// <param name="mapHeight">地图宽</param>
        /// <returns>true：可以找到路径；
        /// false：找不到路径</returns>
        public bool FindPath(Point start,Point end, Point[,] map, int mapWidth,int mapHeight)
        {
            //算法开始：1.把开始点添加进开列表
            openList.Add(start);
            //2. 对开列表的点，只要开列表还有点，算法不停，
            //直到没有点，则返回false；说明找不到路径
            while(openList.Count > 0)
            {
                //3.寻找周围点 F最小的点，调用函数--FindMinOfPoint
                Point point = FindMinOfPoint(openList);

                //4.找到最小F的点后，这个点可能是路径的点，把它移除开列表，放进关列表
                openList.Remove(point);
                closedList.Add(point);
                //resultPoints.Push(point);

                //5.最小F 转换成当前点，接下来，找这个点的周围点集合
                List<Point> surrendedList = GetSurrondPoint(point, map, mapWidth, mapHeight);

                //6.对周围点集合，还得做一次过滤，因为会包含关列表的点；
                PointFilter(surrendedList, closedList);

                //7. 接下来，遍历处理好的周围点集合，
                foreach(Point item in surrendedList)
                {
                    //8.1： 若点存在开列表中，说明此点也在之前的周围点中出现过
                    //则计算G值，给一个机会，万一更小的呢？比较后要更新的就更新
                    if(openList.IndexOf(item) > -1)
                    {
                        //9.1：  计算G值 因为在开列表，则父节点为最小F
                        float nowG = CalG(item, point);
                        //9.2: G值更小的话，
                        if(nowG < item.G)
                        {
                            //把最小F做，当前点的父节点，且更新当前点的G
                            item.UpdateParent(point, nowG);                            
                        }
                    }
                    //8.2： 若点不存在开列表中，说明第一次遇见该店，把最小F做父节点吧
                    else
                    {
                        //9.3： 最小F做当前点的父节点
                        item.Parent = point;
                        //9.4： 计算更新后的FGH
                        CalF(item, end);
                        //9.5： 添加到开列表
                        openList.Add(item);                        
                    }
                }

                //10.循环结束后，周围点遍历完毕，也选出了当前点的后路，此时
                //我们来完善一些外循环的结束条件：
                //当end 出现在openList中，说明我们找到了一条路径！返回true                               
                if(openList.IndexOf(end) > -1)
                {
                    //此时，end点已经存在在openlist里了
                    return true;
                }
            }

            //外循环结束，openList里都不曾出现过end终点，说明找不到路径啊，返回false
            return false;
        }

        /// <summary>
        /// 开列表所有点，找出最小的F的点
        /// </summary>
        /// <param name="openList">开列表</param>
        /// <returns>返回这个最小F的点</returns>
        private Point FindMinOfPoint(List<Point> openList)
        {
            //遍历开列表，找到其中最小的f
            float minVal = float.MaxValue;  //最大的数，用来比较
            Point temp = null; //临时变量 

            foreach(Point p in openList)
            {
                //每次把较小的值赋给minVal，用来比较。
                if(p.F < minVal)
                {
                    temp = p;
                    minVal = p.F;
                }
            }

            //循环结束后，则临时点，就是最小的点
            return temp;
        }

        /// <summary>
        /// 获取当前最小F点的，周围点集合
        /// </summary>
        /// <param name="point">当前最小F点</param>
        /// <param name="map">地图</param>
        /// <param name="mapWidth">地图宽</param>
        /// <param name="mapHeight">地图高</param>
        /// <returns></returns>
        private List<Point> GetSurrondPoint(Point point,Point[,] map,int mapWidth, int mapHeight)
        {
            //1.一般一个点周围有：上下左右，左上左下右上右下 8个点，先定义出来
            Point up = null, down = null, left = null, right = null,
                lu = null, ld = null, ru = null, rd = null;

            //2.接下来挨个取点咯（如果有的话）

            //判定有无上点 ： 当它Z值，小于地图-1.说明不是最顶端，有上点
            //上点判断,取上点（如果有的话）
            if (point.Z < mapHeight - 1)
            {
                up = map[point.X, point.Z + 1];
            }
            //下点判断,取下点（如果有的话）
            if (point.Z > 0)
            {
                down = map[point.X, point.Z - 1];
            }
            //左点判断,取左点（如果有的话）
            if(point.X > 0)
            {
                left = map[point.X - 1, point.Z];
            }
            //左点判断,取右点（如果有的话）
            if (point.X < mapWidth - 1)
            {
                right = map[point.X + 1, point.Z];
            }

            //左上左下右上右下 的 判断：
            //有上点 和 左点，则说明有左上
            if(up != null && left != null)
            {
                lu = map[point.X - 1, point.Z + 1];
            }
            //有下点 和 左点，则说明有左下
            if (down != null && left != null)
            {
                ld = map[point.X - 1, point.Z - 1];
            }
            //有上点 和 右点，则说明有右上
            if (up != null && right != null)
            {
                ru = map[point.X + 1, point.Z + 1];
            }
            //有下点 和 右点，则说明有右下
            if (down != null && right != null)
            {
                rd = map[point.X + 1, point.Z - 1];
            }

            //3.建一个列表，把合格的表放进周围点列表中
            //条件： 点不为空，且不为障碍物，则可放进列表中
            List<Point> List = new List<Point>();

            if(up != null && up.IsWall == false)
            {
                List.Add(up);   //上
            }
            if (down != null && down.IsWall == false)
            {
                List.Add(down); //下
            }
            if (left != null && left.IsWall == false)
            {
                List.Add(left);   //左
            }
            if (right != null && right.IsWall == false)
            {
                List.Add(right);   //右
            }

            //左上左下右上右下 有特殊：如：左上点不为空，且左上 ，左，上都不为障碍才行
            if(lu != null && lu.IsWall == false && left.IsWall == false && up.IsWall == false)
            {
                List.Add(lu);   //左上
            }
            if (ld != null && ld.IsWall == false && left.IsWall == false && down.IsWall == false)
            {
                List.Add(ld);   //左下
            }
            if (ru != null && ru.IsWall == false && right.IsWall == false && up.IsWall == false)
            {
                List.Add(ru);   //右上
            }
            if (rd != null && rd.IsWall == false && right.IsWall == false && down.IsWall == false)
            {
                List.Add(rd);   //右下
            }

            //4.点添加完了，直接返回列表
            return List;
        }
    
        /// <summary>
        /// 把周围点集合做一个过滤，去掉关列表的点
        /// </summary>
        /// <param name="surrendList">周围点集合</param>
        /// <param name="closedList">关列表</param>
        private void PointFilter(List<Point> surrendList , List<Point> closedList)
        {
            //遍历关列表，发现在周围点的移除周围点列表
            foreach(Point item in closedList)
            {
                if(surrendList.IndexOf(item) > -1)
                {
                    //存在，则移除
                    surrendList.Remove(item);
                }
                //其他不做处理，继续循环
            }
        }
    
        /// <summary>
        /// 计算当前点的G值
        /// </summary>
        /// <param name="now">当前点</param>
        /// <param name="parent">父节点</param>
        /// <returns></returns>
        private float CalG(Point now,Point parent) 
        {
            //（now，parent点之间的距离，+ 父节点的G ）= 当前点G值
            return (Vector2.Distance(new Vector2(now.X, now.Z)
                , new Vector2(parent.X, parent.Z)) + parent.G);
        }

        /// <summary>
        /// 计算当前点的 F 值
        /// </summary>
        /// <param name="now">当前点</param>
        /// <param name="end">终点</param>        
        private void CalF(Point now, Point end)
        {
            //1.先计算 当前点的 H 值，简单算法：终点的X、Z分别与当前点的X、Z做减法，
            //取各自减法的绝对值，相加就是H
            float h = Mathf.Abs(end.X - now.X) + Mathf.Abs(end.Z - now.Z);
            //2.G值先默认为0
            float g = 0;
            
            //3.因为当前点，不一定有父节点，所以做判断，
            if(now.Parent == null)
            {
                //没有父节点，则G就是0；
                g = 0;
            }
            else
            {
                //有父节点，求G，跟上面一样
                g = (Vector2.Distance(new Vector2(now.X, now.Z)
                , new Vector2(now.Parent.X, now.Parent.Z)) + now.Parent.G);
            }

            //G 、H 都拿到了，更新当前点的 g f h 值
            float f = g + h;

            now.F = f;
            now.G = g;
            now.H = h;
        }
    }
}
