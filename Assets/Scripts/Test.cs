using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEditor.Localization;
using UnityEditor.Scripting.Python;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YuoTools;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    public int Num = 100000;

    private Stopwatch sp = new();

    public StringTableCollection TableCollection;

    private void Start()
    {
        // IpcHelper.Listen();
        // IpcHelper.MessageReceived += message => message.Log();
    }

    private void OnDestroy()
    {
        IpcHelper.Destroy();
    }

    public Vector3 t1;
    public Transform tran1;
    public Transform tran2;
    public Transform tran3;
    public Transform tran4;
    public Transform tran5;

    [Button]
    public void Test1()
    {
        tran1.position = t1;
        tran2.position = Vector3.ProjectOnPlane(t1, Vector3.up);
        print(Vector3.Angle(tran1.position, tran2.position));
        var qua1 = Quaternion.LookRotation(tran1.position);

        var qua2 = Quaternion.LookRotation(tran2.position);
        var f2 = Quaternion.Inverse(qua1) * qua2;
        tran3.rotation = (qua2 * f2);
        tran4.rotation = qua1;
        tran5.rotation = qua2;
        print((qua1.eulerAngles, qua2.eulerAngles, f2.eulerAngles, (qua1 * f2).eulerAngles));
    }

    public Transform target;

    void Update()
    {
        // 计算从当前方向到目标方向的旋转
        Quaternion rotation = Quaternion.FromToRotation(transform.forward, target.position - transform.position);

        // 将变换的方向设置为目标方向
        transform.rotation *= rotation;
    }

    //性能测试
    [Button]
    public void TestTime()
    {
        // Debug.ClearDeveloperConsole();
        // ListAddTest().Log();
        // NativeListAddTest().Log();
        // ListFindTest().Log();
        // NativeListFindTest().Log();
        // ListRemoveTest().Log();
        // NativeListRemoveTest().Log();
        // TableCollection.GetTable("Test_1").Log().LocaleIdentifier.Log().Code.Log();
        // foreach (var tableCollectionStringTable in TableCollection.StringTables)
        // {
        //     foreach (var stringTableEntry in tableCollectionStringTable.Values)
        //     {
        //         $"{stringTableEntry.Key}_{stringTableEntry.Value}".Log();
        //     }
        // }
    }

    [Button]
    public void RunPy(TextAsset text)
    {
        PythonRunner.RunString(text.text);
    }

    string ListAddTest()
    {
        sp = new();
        List<int> list = new();
        //增
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        sp.Stop();
        return $"List 增：{sp.ElapsedMilliseconds}ms\n";
    }

    string NativeListAddTest()
    {
        sp = new();
        NativeList<int> list = new(Num, Allocator.Temp);
        //增
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        sp.Stop();

        list.Dispose();
        return $"NativeList 增：{sp.ElapsedMilliseconds}ms\n";
    }

    string ListFindTest()
    {
        sp = new();
        List<int> list = new();
        //增
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        //查
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            int index = list.IndexOf(i);
        }

        sp.Stop();

        return $"List 查：{sp.ElapsedMilliseconds}ms\n";
    }

    string NativeListFindTest()
    {
        sp = new();
        NativeList<int> list = new(Num, Allocator.Temp);
        //增
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        //查
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            int index = list.IndexOf(i);
        }

        sp.Stop();

        list.Dispose();
        return $"NativeList 查：{sp.ElapsedMilliseconds}ms\n";
    }

    string ListRemoveTest()
    {
        sp = new();
        List<int> list = new();
        //增
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        //删
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            list.RemoveAt(0);
        }

        sp.Stop();

        return $"List 删：{sp.ElapsedMilliseconds}ms\n";
    }

    string NativeListRemoveTest()
    {
        sp = new();
        NativeList<int> list = new(Num, Allocator.Temp);
        //增
        for (int i = 0; i < Num; i++)
        {
            list.Add(i);
        }

        //删
        sp.Start();
        for (int i = 0; i < Num; i++)
        {
            list.RemoveAt(0);
        }

        sp.Stop();

        list.Dispose();
        return $"NativeList 删：{sp.ElapsedMilliseconds}ms\n";
    }
}