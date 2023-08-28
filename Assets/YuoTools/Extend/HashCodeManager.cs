using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;

[AutoAddToMain]
public class HashCodeManager : YuoComponentInstance<HashCodeManager>
{
    [ShowInInspector] private Dictionary<int, string> data = new Dictionary<int, string>();

    public void Add(int key, string value)
    {
        data.Add(key, value);
    }

    public bool TryGetValue(int key, out string value)
    {
        return data.TryGetValue(key, out value);
    }

    public string GetValue(int key)
    {
        return data[key];
    }

    public bool ContainsKey(int key)
    {
        return data.ContainsKey(key);
    }

    public void Remove(int key)
    {
        data.Remove(key);
    }

    public void Clear()
    {
        data.Clear();
    }
}

public class HashCodeManagerAwakeSystem : YuoSystem<HashCodeManager>, IAwake
{
    public override string Group => SystemGroupConst.Main;
    protected override void Run(HashCodeManager component)
    {
        var data = Resources.Load<HashCodeData>("HashCodeData");
        if (data != null)
        {
            foreach (var item in data.Data)
            {
                component.Add(item.Key, item.Value);
            }
        }
    }
}

public static class HashCodeHelper
{
    public static HashCodeData Data = null;

    public static void Add(string str, int code)
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            //编辑器下才会保存,保存到Resource中
            if (Data == null)
            {
                Data = Resources.Load<HashCodeData>("HashCodeData");
                if (Data == null)
                {
                    Data = ScriptableObject.CreateInstance<HashCodeData>();
                    var path = "Assets/Resources";
                    //如果没有文件夹就创建
                    if (!UnityEditor.AssetDatabase.IsValidFolder(path))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    UnityEditor.AssetDatabase.CreateAsset(Data, path + "/HashCodeData.asset");
                }
            }

            UnityEditor.EditorUtility.SetDirty(Data);
#endif
        }

        Data ??= ScriptableObject.CreateInstance<HashCodeData>();
        Data.Data.TryAdd(code, str);
    }
}

public static class HashCodeEx
{
    /// <summary>
    ///  获取HashCode,并且保存到HashCodeManager中
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int GetHashCodeSave(this string obj)
    {
        if (obj == null)
        {
            return 0;
        }

        var hashCode = obj.GetHashCode();
        if (HashCodeManager.Get.ContainsKey(hashCode))
        {
            return hashCode;
        }

        HashCodeManager.Get.Add(hashCode, obj);
        return hashCode;
    }
}