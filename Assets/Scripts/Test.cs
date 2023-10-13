using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEditor.Localization;
using UnityEngine;
using YuoTools;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    public int Num = 100000;

    private Stopwatch sp = new();

    public StringTableCollection TableCollection;
    //性能测试
    [Button]
    public void TestTime()
    {
        Debug.ClearDeveloperConsole();
        // ListAddTest().Log();
        // NativeListAddTest().Log();
        // ListFindTest().Log();
        // NativeListFindTest().Log();
        // ListRemoveTest().Log();
        // NativeListRemoveTest().Log();
        // TableCollection.GetTable("Test_1").LocaleIdentifier.Code.Log();
        // TableCollection.SharedData.GetEntry("Test_1").Metadata.Log();
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