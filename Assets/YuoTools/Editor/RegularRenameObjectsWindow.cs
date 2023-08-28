namespace YuoTools.Editor
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class RegularRenameObjectsWindow : EditorWindow
    {
        Object[] objects;

        [MenuItem("Tools/正则批量修改")]
        public static void ShowWindow()
        {
            var window = GetWindow<RegularRenameObjectsWindow>("Rename Objects");
            window.minSize = new Vector2(900, 600);
        }

        private string regexPattern = "";
        private string regexReplace = "";


        private string suffix = "";
        private string suffixStartNum = "";
        private int suffixOption;

        private string prefix = "";
        private string prefixStartNum = "";
        private int prefixOption;

        private void OnGUI()
        {
            objects = Selection.objects;
            GUILayout.BeginHorizontal();
            GUILayout.Label("原名字", GUILayout.Width(400));
            GUILayout.Label("新名字", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            for (int index = 0; index < objects.Length; index++)
            {
                if (index > 10)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("……", GUILayout.Width(400));
                    GUILayout.Label("……", GUILayout.Width(400));
                    GUILayout.EndHorizontal();
                    break;
                }

                var obj = objects[index];
                GUILayout.BeginHorizontal();
                GUILayout.Label(obj.name, GUILayout.Width(400));
                GUILayout.Label(GetTargetName(obj.name, index), GUILayout.Width(400));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("正则表达式", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("查找", GUILayout.Width(400));
            GUILayout.Label("替换", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("/", GUILayout.Width(20));
            regexPattern = GUILayout.TextField(regexPattern, GUILayout.Width(200));
            GUILayout.Label("/", GUILayout.Width(20));
            regexReplace = GUILayout.TextField(regexReplace, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("前缀", GUILayout.Width(100));

            prefixOption = GUILayout.Toolbar(prefixOption, new string[] { "自定义文本", "序号" }, GUILayout.Width(200));

            if (prefixOption == 0)
            {
                prefix = GUILayout.TextField(prefix, GUILayout.Width(200));
            }

            else if (prefixOption == 1)
            {
                GUILayout.Label("起始序号", GUILayout.Width(100));
                prefixStartNum = GUILayout.TextField(prefixStartNum, GUILayout.Width(100));

                if (!int.TryParse(prefixStartNum, out int num) && prefixStartNum != "")
                {
                    prefixStartNum = "0";
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("后缀", GUILayout.Width(100));

            suffixOption = GUILayout.Toolbar(suffixOption, new string[] { "自定义文本", "序号" }, GUILayout.Width(200));

            if (suffixOption == 0)
            {
                suffix = GUILayout.TextField(suffix, GUILayout.Width(200));
            }

            else if (suffixOption == 1)
            {
                GUILayout.Label("起始序号", GUILayout.Width(100));
                suffixStartNum = GUILayout.TextField(suffixStartNum, GUILayout.Width(100));

                if (!int.TryParse(suffixStartNum, out int num) && suffixStartNum != "")
                {
                    suffixStartNum = "0";
                }
            }

            GUILayout.EndHorizontal();


            GUILayout.Space(20);


            if (GUILayout.Button("应用正则表达式"))
            {
                Undo.SetCurrentGroupName($"应用正则表达式 [数量:{objects.Length}]");
                Undo.RecordObjects(objects, "应用正则表达式");
                for (int index = 0; index < objects.Length; index++)
                {
                    Object obj = objects[index];
                    obj.name = GetTargetName(obj.name, index);
                }

                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }

            string GetTargetName(string str, int index = 0)
            {
                string result = Regular(str, regexPattern, regexReplace);

                if (prefixOption == 0)
                {
                    result = prefix + result;
                }
                else if (prefixOption == 1)
                {
                    if (!int.TryParse(prefixStartNum, out int num))
                    {
                        num = 0;
                    }

                    result = $"{index + num}_" + result;
                }

                if (suffixOption == 0)
                {
                    result += suffix;
                }
                else if (suffixOption == 1)
                {
                    if (!int.TryParse(suffixStartNum, out int num))
                    {
                        num = 0;
                    }

                    result += $"_{index + num}";
                }


                return result;
            }
        }

        string Regular(string str, string regex, string replace)
        {
            try
            {
                if (string.IsNullOrEmpty(regex))
                {
                    return str;
                }

                return Regex.Replace(str, regex, replace);
            }
            catch (Exception)
            {
                //Debug.LogError(e);
                return str;
            }
        }
    }
}
