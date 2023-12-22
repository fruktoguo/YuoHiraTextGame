using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.CSV;
using UnityEditor.Scripting.Python;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using YuoTools;
using YuoTools.Extend.Helper;
using static Unity.Collections.BurstCompatibleAttribute.BurstCompatibleCompileTarget;
using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    public int Num = 100000;
    public int Num2 = 100000;

    private Stopwatch sp = new();

    public StringTableCollection TableCollection;

    public void Test1()
    {
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
    public async void TestTime()
    {
        Debug.ClearDeveloperConsole();
        double time = 0;
        StopwatchHelper.Start();
        for (int i = 0; i < Num; i++)
        {
            Add1(999, 1);
        }

        time = StopwatchHelper.Stop();

        Debug.Log($"传统方法耗时{time}ms");

        StopwatchHelper.Start();
        for (int i = 0; i < Num; i++)
        {
            Add2(999, 1);
        }

        time = StopwatchHelper.Stop();

        Debug.Log($"Burst方法耗时{time}ms");

        StopwatchHelper.Start();
        for (int i = 0; i < Num; i++)
        {
            Add3(999, 1);
        }

        time = StopwatchHelper.Stop();

        Debug.Log($"Inline方法耗时{time}ms");

        StopwatchHelper.Start();
        Add4();
        time = StopwatchHelper.Stop();
        Debug.Log($"Job方法耗时{time}ms");

        // ListAddTest().Log();
        // NativeListAddTest().Log();
        // ListFindTest().Log();
        // NativeListFindTest().Log();
        // ListRemoveTest().Log();
        // NativeListRemoveTest().Log();
    }

    [Button]
    public void LocalizationTest()
    {
        StringTable table = LocalizationSettings.StringDatabase.GetTable("Main");
        table.GetEntry("Test_1").Value.Log();
    }

    [FilePath] public string TextPath;

    [Button]
    public void CsvTest()
    {
        var fs = new FileStream(TextPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Csv.ImportInto(new StreamReader(fs), TableCollection);
    }

    public string num1;
    public string num2;

    [Button]
    public void BigNumTest()
    {
        YuoBigNum b1 = num1;
        YuoBigNum b2 = num2;
        StopwatchHelper.Start();
        var result = b1 * b2;
        StopwatchHelper.Stop();
        $"计算完成，结果为：{result}，耗时：{StopwatchHelper.Stop()}ms".Log();
    }

    public void Add1(double a, double b)
    {
        double precision = 0.000001;
        while (Math.Abs(a - b * b) > precision)
        {
            b = (b + a / b) / 2;
        }
    }

    [BurstCompatible(CompileTarget = PlayerAndEditor)]
    public void Add2(double a, double b)
    {
        double precision = 0.000001;
        while (Math.Abs(a - b * b) > precision)
        {
            b = (b + a / b) / 2;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add3(double a, double b)
    {
        double precision = 0.000001;
        while (Math.Abs(a - b * b) > precision)
        {
            b = (b + a / b) / 2;
        }
    }

    public void Add4()
    {
        NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);
        for (int i = 0; i < Num2; i++)
        {
            handles.Add(new MyJob { num = Num }.Schedule());
        }

        JobHandle.CompleteAll(handles);
        handles.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct MyJob : IJob
    {
        public double num;

        public void Execute()
        {
            for (double i = 0; i < num; i++)
            {
                double precision = 0.000001;
                double a = 999;
                double b = 1;
                while (Math.Abs(a - b * b) > precision)
                {
                    b = (b + a / b) / 2;
                }
            }
        }
    }

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