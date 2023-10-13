#if UNITY_EDITOR

using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace YuoTools.Main.Ecs
{
    public partial class YuoComponent
    {
        [Button]
        [HorizontalGroup("SelectField")]
        [ShowIf("_privateSelect")]
        void SelectField()
        {
            var result = AssetDatabase.FindAssets(Type.Name);
            if (result.Length > 0)
            {
                Selection.activeObject =
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0]));
            }
        }

        [Button]
        [HorizontalGroup("SelectField")]
        [ShowIf("_privateSelect")]
        void OpenField()
        {
            var result = AssetDatabase.FindAssets(Type.Name);
            if (result.Length > 0)
            {
                AssetDatabase.OpenAsset(
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0])));
            }

            //只是为了去掉警告
            _privateTips = _privateTips.ToString();
            _privateSelect = !_privateSelect;
            _privateSelect = false;
        }

        [Button]
        [HorizontalGroup("SelectField")]
        [ShowIf("_privateSelect")]
        void CopyComponentName()
        {
            //复制到剪切板
            GUIUtility.systemCopyBuffer = Type.Name;
        }

        private bool _isPlaying => Application.isPlaying;

        [ShowInInspector] [HorizontalGroup("SelectField", width: 50)] [HideLabel] [ShowIf("_isPlaying")]
        private bool _privateSelect;

        [ShowInInspector] [HorizontalGroup("SelectField")] [HideLabel] [HideIf("_privateSelect")] [ReadOnly]
        private string _privateTips = "选择或者打开组件对应的文件(如果文件名和组件不一致,请手动查找)";

        [ShowIf("BaseComponentType", null)]
        [ShowInInspector]
        [LabelText("父组件类型")]
        private string ShowBase => BaseComponentType?.Name;

        [HorizontalGroup("SelectField")]
        [Button(ButtonSizes.Medium)]
        [ShowIf("_privateSelect")]
        private void SavePrefab()
        {
            var path = EditorUtility.OpenFolderPanel("选择保存路径", Application.dataPath, "").Log();
            path = $"{path}/{Type.Name}_Prefab.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            if (string.IsNullOrEmpty(path)) return;
            var data = YuoComponentPrefabData.Create(this);
            AssetDatabase.CreateAsset(data, path.Log());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif