using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Threading.Tasks;
using OpenAi;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEngine.Serialization;
using YuoTools.Extend.Helper;

namespace AICommand
{
    public sealed class AICommandWindow : OdinEditorWindow
    {
        #region Temporary script file operations

        const string TempFilePath = "Assets/AICommandTemp.cs";

        bool TempFileExists => System.IO.File.Exists(TempFilePath);

        void CreateScriptAsset(string code)
        {
            // UnityEditor internal method: ProjectWindowUtil.CreateScriptAssetWithContent
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
            if (method != null) method.Invoke(null, new object[] { TempFilePath, code });
        }

        #endregion

        #region Script generator

        [EnumToggleButtons] public CommandType commandType = CommandType.EditorScript;

        public enum CommandType
        {
            EditorScript,
            CmdCommand,
        }

        static string EditorScriptPrompt(string input)
            => "Write a Unity Editor script.\n" +
               " - It provides its functionality as a menu item placed \"Edit\" > \"Do Task\".\n" +
               " - It doesn’t provide any editor window. It immediately does the task when the menu item is invoked.\n" +
               " - Don’t use GameObject.FindGameObjectsWithTag.\n" +
               " - There is no selected object. Find game objects manually.\n" +
               "The task is described as follows:\n" +
               "It is necessary to fully qualify the namespace.\n" +
               "你必须要添加完整的中文注释\n" +
               input;

        static string CmdPrompt(string input)
            => "Write a command that can be executed in the Windows command line.\n" +
               "The task is described as follows:\n" +
               "不需要任何的注释\n" +
               "这个命令必须要在Windows命令行中执行\n" +
               "必须保证不会严重威胁Windows安全\n" +
               input;

        [HorizontalGroup("button")]
        [Button("生成代码", ButtonHeight = 100)]
        async void CreateGenerator()
        {
            var message = commandType switch
            {
                CommandType.EditorScript => EditorScriptPrompt(prompt),
                CommandType.CmdCommand => CmdPrompt(prompt),
                _ => ""
            };

            result = await ChatGpt.SingleAskStream(message, s => result = s);
        }

        [HorizontalGroup("button")]
        [Button("运行代码", ButtonHeight = 100)]
        void RunGenerator()
        {
            if (result.IsNullOrWhitespace()) return;
            switch (commandType)
            {
                case CommandType.EditorScript:
                    CreateScriptAsset(result);
                    break;
                case CommandType.CmdCommand:
                    WindowsHelper.Command(result);
                    break;
            }
        }

        #endregion

        #region Editor GUI

        [HorizontalGroup()] [TextArea(5, 100)] [LabelWidth(100)]
        public string prompt = "输入Command";

        [HorizontalGroup()] [TextArea(5, 100)] public string result = "";


        [MenuItem("Tools/AI Command")]
        static void Init() => GetWindow<AICommandWindow>(true, "AI Command");

        #endregion

        #region Script lifecycle

        protected override void OnEnable()
            => AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

        protected override void OnDisable()
            => AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

        void OnAfterAssemblyReload()
        {
            if (!TempFileExists) return;
            EditorApplication.ExecuteMenuItem("Edit/Do Task");
            AssetDatabase.DeleteAsset(TempFilePath);
        }

        #endregion
    }
} // namespace AICommand