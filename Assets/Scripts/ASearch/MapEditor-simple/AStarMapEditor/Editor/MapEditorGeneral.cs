using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MapEditorGeneral : EditorWindow
{
    //寻路算法需要输入的一些数据
    private int mapX = 64;   //生成地图块的x
    private int mapZ = 64;   //生成地图块的Z
    private int blockSize = 8;  //生成地图块的块的大小
    int blockHeight = 8;        //生成地图块的块的高度
    private string blockPrefabPath = "Assets/AStarMapEditor/resource/Cube.prefab";    

    //用MenuItem，参数是任务栏的地址，
    [MenuItem("Tools/MapEditor-AStar")]
    static void Fun()
    {
        //静态的函数，创建一个窗口 -- 编辑器的大纲窗口
        EditorWindow.GetWindow <MapEditorGeneral> ();
    }

    public void OnGUI()
    {       
        //在编辑器上，显示一下内容
        GUILayout.Label("地图X方向块数：");
        //TextField 即是用户可以输入的地方，让user输入mapX，转换成int，存在成员变量中
        this.mapX = Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("地图Z方向块数：");
        this.mapZ = Convert.ToInt32(GUILayout.TextField(this.mapZ.ToString()));

        GUILayout.Label("地图块大小：");
        this.blockSize = Convert.ToInt32(GUILayout.TextField(this.blockSize.ToString()));

        GUILayout.Label("地图块高度：");
        this.blockHeight = Convert.ToInt32(GUILayout.TextField(this.blockHeight.ToString()));

        GUILayout.Label("选择地图原点：");
        /*如果选中了，则把选中的原点名字显示出来，如果没有选中，就提示未选中*/
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("您未选中UI节点，无法生成地图~");
        }
        /* 问题在于，很久才重新显示选中的名字，所以我们需要刷新 */

        //生成一个可点击的按钮，如果点击了，则进行操作 -- 生成地图块
        if (GUILayout.Button("在原点下生成地图块"))
        {
             if(Selection.activeGameObject != null)
            {
                Debug.Log("开始生成...");
                this.CreateBlockAt(Selection.activeGameObject);
                Debug.Log("生成结束！");
            }
        }

        //生成一个可点击的按钮，如果点击了，则进行操作 -- 重置地图块
        if (GUILayout.Button("重置地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ResetBlocks(Selection.activeGameObject);
            }
        }

        //生成一个可点击的按钮，如果点击了，则进行操作 -- 清理地图块
        if (GUILayout.Button("清理地图块"))
        {
            if(Selection.activeGameObject != null)
            {
                this.ClearBlockAt(Selection.activeGameObject);
            }
        }
    }

    /*作用： 在原点，生成方块
     参数：原点物体*/
    private void CreateBlockAt(GameObject org)
    {        
        //当点击生成方块时，先判定地图原点上有没有挂载 脚本：MapEditorMgr
        MapEditorMgr mgr = org.GetComponent<MapEditorMgr>();
        if (!mgr)
        {
            //如果没有挂载，则挂载一下
            mgr = org.AddComponent<MapEditorMgr>();
        }
        //再把挂载的脚本的属性做一些初始化
        mgr.mapX = this.mapX;
        mgr.mapZ = this.mapZ;
        mgr.blockSize = this.blockSize;


        //先表示一个方块开始位置
        Vector3 startPos = new Vector3(this.blockSize * 0.5f, 0, this.blockSize * 0.5f);
        //生成方块，先把原点上的方块删除
        this.ClearBlockAt(org);
        //再拿到我们资源目录下的prefab，赋值给游戏物体对象：cubePrefab，后面我们好用
        GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>
            (blockPrefabPath);
       
        //接下来，结合A星寻路算法，先根据X\Z的值，创建一个X*Z的方块区
        for(int i = 0; i < mapZ; i++)
        {
            Vector3 pos = startPos;
            for(int j = 0; j < mapX; j++)
            {                
                //实例化一个对象出来,用prefab实例化一个对象
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePrefab) as GameObject;
                //初始化这个方块
                //Debug.Log("it is 116");
                cube.transform.SetParent(org.transform, false); //设置父节点，为原点
                //Debug.Log("it is 118");
                cube.transform.localPosition = pos;
                cube.transform.localScale = new Vector3(this.blockSize, this.blockHeight, this.blockSize);

                //生成一个地图块后，要保存它的数据，则把数据脚本挂载上去
                BlockData blockData = cube.AddComponent<BlockData>();
                blockData.mapX = j;
                blockData.maxZ = i;
                blockData.isGo = 0;

                //当前生成方块位置的x，加上生成方块的大小;依次生成X轴上的方块
                pos.x += this.blockSize;               
            }
            //走完这一行，指，X行后，z值推进
            startPos.z += this.blockSize;
        }
    }

    /*作用： 在原点，重置方块
     参数：原点物体*/
    private void ResetBlocks(GameObject org)
    {
        //重置方块，先拿到原点下，子节点的数量，即生成的方块的数量
        int count = org.transform.childCount;
        //再遍历，这些方块，不删除，只是把isGo改成不可行走。
        for (int i = 0; i < count; i++)
        {
            GameObject cube = org.transform.GetChild(i).gameObject;
            cube.GetComponent<BlockData>().isGo = 0;

            //重置这些块的时候，还要把这些块的网格显示出来
            cube.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    /*作用： 在原点，清理方块
     参数：原点物体*/
    private void ClearBlockAt(GameObject org)
    {
        //删除方块，先拿到原点下，子节点的数量，即生成的方块的数量
        int count = org.transform.childCount;
        //Debug.Log(count);
        //再遍历，这些方块，依次删除
        for(int i = 0; i < count; i++)
        {
            GameObject cube = org.transform.GetChild(0).gameObject;
            GameObject.DestroyImmediate(cube);
        }
    }
    
    //解决刷新慢的问题，调用这个函数，
    private void OnSelectionChange()
    {
        //selection改变以后，重新刷新window，即重新触发OnGUI
        this.Repaint();
    }    
}
