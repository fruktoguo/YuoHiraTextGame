using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//����һ��װ����������ű������ڽű�MapEditorMgr��װ��
[CustomEditor(typeof(MapEditorMgr))]
[CanEditMultipleObjects]
public class RoadEditor : Editor
{
    MapEditorMgr script;
    private bool placing = false;   //�ǲ��Ǳ༭ģʽ
    private bool enterPlacingBatMode = false;//�Ƿ����������·ģʽ
    private string mapPath = "Assets/AStarMapEditor/resource/mapTexture.asset";

    //��չ����ʱ��Ҫ�ѵ�ǰ��view����focus
    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }

    //����һ���ӿڣ�ȥ��ⳡ���µ�GUI�ϵ��û�����
    public void OnSceneGUI()
    {
        if(this.placing == false)
        {
            //������Ǳ༭ģʽ����ֱ�ӷ��أ������
            return;
        }
        

        //����ģʽ
        if (Event.current.type == EventType.KeyDown)
        {
            //�а������£����������ж�
            if(Event.current.keyCode == KeyCode.Space)
            {
                //�������¼�������ϣ�����������ʱ����use
                Event.current.Use();
                Debug.Log("space");
                //����û������˿ո�˵�������������ģʽ
                //����û���������ģʽ���ְ��¿ո����˳�
                this.enterPlacingBatMode = !this.enterPlacingBatMode;
            }
            else if(Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                Debug.Log("C");
                //���������C��˵�������뵥��ģʽ
                this.enterPlacingBatMode = false;

                //����ģʽ�µ�����׷��
                Ray worldRay1 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo1;
                if (!Physics.Raycast(worldRay1, out hitInfo1))
                {
                    //���û����ײ������ֱ�ӷ���
                    return;
                }
                if (hitInfo1.collider.gameObject.name != "Cube")
                {
                    //���ײ���Ĳ��ǿ飡Ҳ����
                    return;
                }

                this.changeMapValue(ref hitInfo1);
                return;
            }
        }

        //�����������ģʽ����ֱ�ӷ��أ������ü��GUI��
        if (this.enterPlacingBatMode == false)
        {
            return;
        }

        //����ģʽ
        //����귢��һ�����ߣ���ײ�����ĸ�����
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if(!Physics.Raycast(worldRay,out hitInfo))
        {
            //���û����ײ������ֱ�ӷ���
            return;
        }
        if(hitInfo.collider.gameObject.name != "Cube")
        {
            //���ײ���Ĳ��ǿ飡Ҳ����
            return;
        }

        //��ѡ�еĿ飬����ֵ���ó�true
        this.setMapValue(ref hitInfo,1);
    }

    /*
     ����ģʽ��Ҫ���ã������ֵ
    ������������ײ������
     */
    public void changeMapValue(ref RaycastHit hitInfo)
    {
        BlockData data = hitInfo.collider.gameObject.GetComponent<BlockData>();
        //��ѡ�еķ����isGo��һ��ȡ���Ĳ���
        data.isGo = (data.isGo == 1) ? 0 : 1;
        if (data.isGo == 1)
        {
            //����������ʱ��˵������Ҫ�Ѻ췽������������ˣ��÷���ͼ��
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    /*
     ���ã����õ�ͼ��ֵ��
    ������������ײ�����壬��Ҫ���õ�ֵ
     */
    public void setMapValue(ref RaycastHit hitInfo,int value)
    {
        //��ȡ�����data
        BlockData data = hitInfo.collider.gameObject.GetComponent<BlockData>();
        if(data == null)
        {
            return;
        }
        data.isGo = value;
        if(data.isGo == 1)
        {
            //����������ʱ��˵������Ҫ�Ѻ췽������������ˣ��÷���ͼ��
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

    }
    //����һ��GUI�ķ�����ȥ��һ�������ͼ���û�����
    public override void OnInspectorGUI()
    {
        //չʾ���������Ĭ�ϱ���
        DrawDefaultInspector(); //����ԭ�нű���GUI
        GUILayout.Label("�������������ļ�������·��");
        this.mapPath = GUILayout.TextField(mapPath);

        this.script = (MapEditorMgr)this.target;
        SceneView view = GetSceneView();

        if (!this.placing && GUILayout.Button("��ʼ�༭", GUILayout.Height(40)))
        {
            //������Ǳ༭ģʽ����������ť��ʾ����            
            this.placing = true;
            //��ʱ�û���δ����༭ģʽ�����enterPlacingBatMode��Ϊfalse
            this.enterPlacingBatMode = false;
            //�ü��̹�ע����һ���༭������
            view.Focus();
        }

        //����ǰ�ť��ʾ�ˣ���ɻ�ɫ
        GUI.backgroundColor = Color.yellow;

        if(this.placing && GUILayout.Button("�����༭", GUILayout.Height(40)))
        {
            //����Ѿ��Ǳ༭ģʽ������ʾ�����༭�����ı�״̬
            this.placing = false;
            this.enterPlacingBatMode = false;
            this.ExportMapBitMap();
        }

        //base.OnInspectorGUI();
    }

    //�������ݣ�����bitmap
    private void ExportMapBitMap()
    {
        //����һ��2d��ͼ����
        Texture2D mapTex = new Texture2D(this.script.mapX, this.script.mapZ,
            TextureFormat.Alpha8, false);
        //���������
        byte[] rawData = mapTex.GetRawTextureData();
        //�Ȱ��������ݳ�ʼ���ɺ�ɫ
        for(int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0;
        }

        //�������ʵ���ĺ���
        for(int i = 0; i < this.script.gameObject.transform.childCount; i++)
        {
            //���ÿ�����ӵ�BlockData����
            BlockData blockData = this.script.gameObject.transform.GetChild(i)
                .GetComponent<BlockData>();
            rawData[i] = (byte)((blockData.isGo == 0) ? 0 : 255);
        }

        //�ı�󣬰�������ص������� -- �����ԭʼ����
        mapTex.LoadRawTextureData(rawData);

        //���غ󣬰������浽�ļ�����ȥ����ɾ���
        AssetDatabase.DeleteAsset(this.mapPath);
        AssetDatabase.CreateAsset(mapTex, this.mapPath);

        //���沢ˢ��
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
