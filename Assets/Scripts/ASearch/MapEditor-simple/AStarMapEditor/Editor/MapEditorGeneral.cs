using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MapEditorGeneral : EditorWindow
{
    //Ѱ·�㷨��Ҫ�����һЩ����
    private int mapX = 64;   //���ɵ�ͼ���x
    private int mapZ = 64;   //���ɵ�ͼ���Z
    private int blockSize = 8;  //���ɵ�ͼ��Ŀ�Ĵ�С
    int blockHeight = 8;        //���ɵ�ͼ��Ŀ�ĸ߶�
    private string blockPrefabPath = "Assets/AStarMapEditor/resource/Cube.prefab";    

    //��MenuItem���������������ĵ�ַ��
    [MenuItem("Tools/MapEditor-AStar")]
    static void Fun()
    {
        //��̬�ĺ���������һ������ -- �༭���Ĵ�ٴ���
        EditorWindow.GetWindow <MapEditorGeneral> ();
    }

    public void OnGUI()
    {       
        //�ڱ༭���ϣ���ʾһ������
        GUILayout.Label("��ͼX���������");
        //TextField �����û���������ĵط�����user����mapX��ת����int�����ڳ�Ա������
        this.mapX = Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("��ͼZ���������");
        this.mapZ = Convert.ToInt32(GUILayout.TextField(this.mapZ.ToString()));

        GUILayout.Label("��ͼ���С��");
        this.blockSize = Convert.ToInt32(GUILayout.TextField(this.blockSize.ToString()));

        GUILayout.Label("��ͼ��߶ȣ�");
        this.blockHeight = Convert.ToInt32(GUILayout.TextField(this.blockHeight.ToString()));

        GUILayout.Label("ѡ���ͼԭ�㣺");
        /*���ѡ���ˣ����ѡ�е�ԭ��������ʾ���������û��ѡ�У�����ʾδѡ��*/
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("��δѡ��UI�ڵ㣬�޷����ɵ�ͼ~");
        }
        /* �������ڣ��ܾò�������ʾѡ�е����֣�����������Ҫˢ�� */

        //����һ���ɵ���İ�ť���������ˣ�����в��� -- ���ɵ�ͼ��
        if (GUILayout.Button("��ԭ�������ɵ�ͼ��"))
        {
             if(Selection.activeGameObject != null)
            {
                Debug.Log("��ʼ����...");
                this.CreateBlockAt(Selection.activeGameObject);
                Debug.Log("���ɽ�����");
            }
        }

        //����һ���ɵ���İ�ť���������ˣ�����в��� -- ���õ�ͼ��
        if (GUILayout.Button("���õ�ͼ��"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ResetBlocks(Selection.activeGameObject);
            }
        }

        //����һ���ɵ���İ�ť���������ˣ�����в��� -- �����ͼ��
        if (GUILayout.Button("�����ͼ��"))
        {
            if(Selection.activeGameObject != null)
            {
                this.ClearBlockAt(Selection.activeGameObject);
            }
        }
    }

    /*���ã� ��ԭ�㣬���ɷ���
     ������ԭ������*/
    private void CreateBlockAt(GameObject org)
    {        
        //��������ɷ���ʱ�����ж���ͼԭ������û�й��� �ű���MapEditorMgr
        MapEditorMgr mgr = org.GetComponent<MapEditorMgr>();
        if (!mgr)
        {
            //���û�й��أ������һ��
            mgr = org.AddComponent<MapEditorMgr>();
        }
        //�ٰѹ��صĽű���������һЩ��ʼ��
        mgr.mapX = this.mapX;
        mgr.mapZ = this.mapZ;
        mgr.blockSize = this.blockSize;


        //�ȱ�ʾһ�����鿪ʼλ��
        Vector3 startPos = new Vector3(this.blockSize * 0.5f, 0, this.blockSize * 0.5f);
        //���ɷ��飬�Ȱ�ԭ���ϵķ���ɾ��
        this.ClearBlockAt(org);
        //���õ�������ԴĿ¼�µ�prefab����ֵ����Ϸ�������cubePrefab���������Ǻ���
        GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>
            (blockPrefabPath);
       
        //�����������A��Ѱ·�㷨���ȸ���X\Z��ֵ������һ��X*Z�ķ�����
        for(int i = 0; i < mapZ; i++)
        {
            Vector3 pos = startPos;
            for(int j = 0; j < mapX; j++)
            {                
                //ʵ����һ���������,��prefabʵ����һ������
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePrefab) as GameObject;
                //��ʼ���������
                //Debug.Log("it is 116");
                cube.transform.SetParent(org.transform, false); //���ø��ڵ㣬Ϊԭ��
                //Debug.Log("it is 118");
                cube.transform.localPosition = pos;
                cube.transform.localScale = new Vector3(this.blockSize, this.blockHeight, this.blockSize);

                //����һ����ͼ���Ҫ�����������ݣ�������ݽű�������ȥ
                BlockData blockData = cube.AddComponent<BlockData>();
                blockData.mapX = j;
                blockData.maxZ = i;
                blockData.isGo = 0;

                //��ǰ���ɷ���λ�õ�x���������ɷ���Ĵ�С;��������X���ϵķ���
                pos.x += this.blockSize;               
            }
            //������һ�У�ָ��X�к�zֵ�ƽ�
            startPos.z += this.blockSize;
        }
    }

    /*���ã� ��ԭ�㣬���÷���
     ������ԭ������*/
    private void ResetBlocks(GameObject org)
    {
        //���÷��飬���õ�ԭ���£��ӽڵ�������������ɵķ��������
        int count = org.transform.childCount;
        //�ٱ�������Щ���飬��ɾ����ֻ�ǰ�isGo�ĳɲ������ߡ�
        for (int i = 0; i < count; i++)
        {
            GameObject cube = org.transform.GetChild(i).gameObject;
            cube.GetComponent<BlockData>().isGo = 0;

            //������Щ���ʱ�򣬻�Ҫ����Щ���������ʾ����
            cube.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    /*���ã� ��ԭ�㣬������
     ������ԭ������*/
    private void ClearBlockAt(GameObject org)
    {
        //ɾ�����飬���õ�ԭ���£��ӽڵ�������������ɵķ��������
        int count = org.transform.childCount;
        //Debug.Log(count);
        //�ٱ�������Щ���飬����ɾ��
        for(int i = 0; i < count; i++)
        {
            GameObject cube = org.transform.GetChild(0).gameObject;
            GameObject.DestroyImmediate(cube);
        }
    }
    
    //���ˢ���������⣬�������������
    private void OnSelectionChange()
    {
        //selection�ı��Ժ�����ˢ��window�������´���OnGUI
        this.Repaint();
    }    
}
