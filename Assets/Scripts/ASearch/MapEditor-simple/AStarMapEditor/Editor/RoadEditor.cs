using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//带上一个装饰器，这个脚本挂载在脚本MapEditorMgr上装饰
[CustomEditor(typeof(MapEditorMgr))]
[CanEditMultipleObjects]
public class RoadEditor : Editor
{
    MapEditorMgr script;
    private bool placing = false;   //是不是编辑模式
    private bool enterPlacingBatMode = false;//是否进入连续铺路模式
    private string mapPath = "Assets/AStarMapEditor/resource/mapTexture.asset";

    //拓展键盘时，要把当前的view申请focus
    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }

    //调用一个接口，去监测场景下的GUI上的用户输入
    public void OnSceneGUI()
    {
        if(this.placing == false)
        {
            //如果不是编辑模式，则直接返回，不检测
            return;
        }
        

        //连续模式
        if (Event.current.type == EventType.KeyDown)
        {
            //有按键按下，则做如下判定
            if(Event.current.keyCode == KeyCode.Space)
            {
                //当进入事件，但不希望被其他打断时，用use
                Event.current.Use();
                Debug.Log("space");
                //如果用户按下了空格，说明它想进入连续模式
                //如果用户进入连续模式后，又按下空格，则退出
                this.enterPlacingBatMode = !this.enterPlacingBatMode;
            }
            else if(Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                Debug.Log("C");
                //如果按下了C，说明它进入单个模式
                this.enterPlacingBatMode = false;

                //单个模式下的射线追踪
                Ray worldRay1 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo1;
                if (!Physics.Raycast(worldRay1, out hitInfo1))
                {
                    //如果没有碰撞到，则直接返回
                    return;
                }
                if (hitInfo1.collider.gameObject.name != "Cube")
                {
                    //如果撞到的不是块！也返回
                    return;
                }

                this.changeMapValue(ref hitInfo1);
                return;
            }
        }

        //如果不是连续模式，则直接返回，不调用检测GUI了
        if (this.enterPlacingBatMode == false)
        {
            return;
        }

        //连续模式
        //从鼠标发射一条射线，看撞到了哪个方块
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if(!Physics.Raycast(worldRay,out hitInfo))
        {
            //如果没有碰撞到，则直接返回
            return;
        }
        if(hitInfo.collider.gameObject.name != "Cube")
        {
            //如果撞到的不是块！也返回
            return;
        }

        //把选中的块，它的值设置成true
        this.setMapValue(ref hitInfo,1);
    }

    /*
     单个模式下要设置，方块的值
    参数：射线碰撞的物体
     */
    public void changeMapValue(ref RaycastHit hitInfo)
    {
        BlockData data = hitInfo.collider.gameObject.GetComponent<BlockData>();
        //把选中的方块的isGo做一个取反的操作
        data.isGo = (data.isGo == 1) ? 0 : 1;
        if (data.isGo == 1)
        {
            //当可以行走时，说明我们要把红方块的网格隐藏了，好放贴图；
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    /*
     作用：设置地图的值，
    参数：射线碰撞的物体，和要设置的值
     */
    public void setMapValue(ref RaycastHit hitInfo,int value)
    {
        //先取到块的data
        BlockData data = hitInfo.collider.gameObject.GetComponent<BlockData>();
        if(data == null)
        {
            return;
        }
        data.isGo = value;
        if(data.isGo == 1)
        {
            //当可以行走时，说明我们要把红方块的网格隐藏了，好放贴图；
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

    }
    //重载一个GUI的方法，去做一个检查器图形用户界面
    public override void OnInspectorGUI()
    {
        //展示，检查器的默认变量
        DrawDefaultInspector(); //保留原有脚本的GUI
        GUILayout.Label("设置配置数据文件的生成路径");
        this.mapPath = GUILayout.TextField(mapPath);

        this.script = (MapEditorMgr)this.target;
        SceneView view = GetSceneView();

        if (!this.placing && GUILayout.Button("开始编辑", GUILayout.Height(40)))
        {
            //如果不是编辑模式，则把这个按钮显示出来            
            this.placing = true;
            //此时用户还未进入编辑模式，则把enterPlacingBatMode设为false
            this.enterPlacingBatMode = false;
            //让键盘关注到另一个编辑器窗口
            view.Focus();
        }

        //如果是按钮显示了，变成黄色
        GUI.backgroundColor = Color.yellow;

        if(this.placing && GUILayout.Button("结束编辑", GUILayout.Height(40)))
        {
            //如果已经是编辑模式，则显示结束编辑，并改变状态
            this.placing = false;
            this.enterPlacingBatMode = false;
            this.ExportMapBitMap();
        }

        //base.OnInspectorGUI();
    }

    //导出数据，导出bitmap
    private void ExportMapBitMap()
    {
        //创建一个2d地图纹理？
        Texture2D mapTex = new Texture2D(this.script.mapX, this.script.mapZ,
            TextureFormat.Alpha8, false);
        //纹理的数据
        byte[] rawData = mapTex.GetRawTextureData();
        //先把所有数据初始化成黑色
        for(int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0;
        }

        //遍历组件实例的孩子
        for(int i = 0; i < this.script.gameObject.transform.childCount; i++)
        {
            //获得每个孩子的BlockData数据
            BlockData blockData = this.script.gameObject.transform.GetChild(i)
                .GetComponent<BlockData>();
            rawData[i] = (byte)((blockData.isGo == 0) ? 0 : 255);
        }

        //改变后，把纹理加载到方块上 -- 纹理的原始数据
        mapTex.LoadRawTextureData(rawData);

        //加载后，把它保存到文件夹里去，先删后加
        AssetDatabase.DeleteAsset(this.mapPath);
        AssetDatabase.CreateAsset(mapTex, this.mapPath);

        //保存并刷新
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
