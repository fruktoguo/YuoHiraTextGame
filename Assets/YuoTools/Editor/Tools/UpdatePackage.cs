using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using YuoTools.Extend.Helper;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public class UpdatePackageWindow : OdinEditorWindow
{
    [MenuItem("Tools/UpdatePackageTest")]
    public static void Test2()
    {
        //获取 AssetStoreAssetInspector 的 Instance
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.AssetStoreAssetInspector");
        var instanceProperty = inspectorType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        var instance = instanceProperty.GetValue(null);
        ReflexHelper.LogAll(instance);
    }
    [MenuItem("Tools/UpdatePackage")]
    public static async void UpdatePackage()
    {
        //检索PackageManager中的所有包
        ListRequest packages = UnityEditor.PackageManager.Client.List();
        //添加一个等待进度条
        EditorUtility.DisplayProgressBar("更新包", "正在检查更新", 0);
        while (!packages.IsCompleted)
        {
            await Task.Delay(100);
        }

        //移除进度条
        EditorUtility.ClearProgressBar();
        List<UpdateVersionData> updateList = new();
        List<PackageData> allPackage = new();
        //查看是否有更新
        foreach (PackageInfo packageInfo in packages.Result)
        {
            if (packageInfo.versions.compatible.Length > 1)
            {
                var targetVersion = packageInfo.versions.compatible[1];
                var nowVersion = packageInfo.version;
                if (targetVersion != nowVersion)
                {
                    var data = new UpdateVersionData(packageInfo.name, nowVersion, targetVersion);
                    if (targetVersion.Contains("pre")) data.isUpdate = false;
                    updateList.Add(data);
                }
            }

            allPackage.Add(new PackageData { name = packageInfo.name, version = packageInfo.version });
        }

        //检索我的所有资源
         
        var r = UnityEditor.PackageManager.Client.SearchAll();
        

        EditorUtility.DisplayProgressBar("更新包", "正在检查更新", 0);
        while (!r.IsCompleted)
        {
            await Task.Delay(100);
        }
        
        EditorUtility.ClearProgressBar();
        var window = GetWindow<UpdatePackageWindow>();
        window.Show();
        window.UpdateList = updateList;
        window.AllAssets = new List<string>(r.Result.Select(x => x.name));
        window.allPackage = allPackage;
    }

    [EnumToggleButtons] public PackageType packageType = PackageType.Update;

    [ShowIf("packageType", PackageType.Update)]
    [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, ShowItemCount = false, NumberOfItemsPerPage = 99)]
    public List<UpdateVersionData> UpdateList = new();

    public List<string> AllAssets = new();

    [Serializable]
    public class UpdateVersionData
    {
        [ReadOnly] [HideLabel] [HorizontalGroup(width: 300)]
        public string packageName;

        [ReadOnly] [HorizontalGroup(width: 200, LabelWidth = 100)]
        public string nowVersion;

        [HorizontalGroup(width: 200, LabelWidth = 100)]
        public string targetVersion;

        //是否需要更新
        [HorizontalGroup(width: 70, LabelWidth = 50)] [LabelText("是否更新")]
        public bool isUpdate = true;

        public UpdateVersionData(string packageName, string nowVersion, string targetVersion)
        {
            this.packageName = packageName;
            this.nowVersion = nowVersion;
            this.targetVersion = targetVersion;
        }
    }

    [Serializable]
    public class PackageData
    {
        [ReadOnly] [HideLabel] [HorizontalGroup(width: 300)]
        public string name;

        [ReadOnly] [HorizontalGroup(width: 200, LabelWidth = 100)]
        public string version;

        [HorizontalGroup(width: 200, LabelWidth = 100)] [LabelText("是否移除")]
        public bool isRemove;
    }

    [ShowIf("packageType", PackageType.Remove)]
    public List<PackageData> allPackage = new();

    [Button]
    public async void Execute()
    {
        foreach (var data in UpdateList)
        {
            if (!data.isUpdate) continue;
            var update = UnityEditor.PackageManager.Client.Add(data.packageName + "@" + data.targetVersion);
            //添加一个等待进度条
            EditorUtility.DisplayProgressBar("更新包",
                $"正在更新 {data.packageName} 从 {data.nowVersion} 到 {data.targetVersion}", 0);
            while (!update.IsCompleted)
            {
                await Task.Delay(100);
            }

            //移除进度条
            EditorUtility.ClearProgressBar();
        }

        foreach (var data in allPackage)
        {
            if (data.isRemove)
            {
                var update = UnityEditor.PackageManager.Client.Remove(data.name);
                //添加一个等待进度条
                EditorUtility.DisplayProgressBar("移除包",
                    $"正在移除 {data}", 0);
                while (!update.IsCompleted)
                {
                    await Task.Delay(100);
                }

                //移除进度条
                EditorUtility.ClearProgressBar();
            }
        }

        //关闭窗口
        Close();
        //刷新资源
        AssetDatabase.Refresh();
        UpdatePackage();
    }

    public enum PackageType
    {
        Update,
        Remove
    }
}