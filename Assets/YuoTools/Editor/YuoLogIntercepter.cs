using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using YuoTools.Extend.Helper;

namespace YuoTools.Editor
{
    internal sealed class YuoLogIntercepter
    {
        [OnOpenAsset(-1)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            try
            {
                var stackTrack = GetStackTrace();
                if (stackTrack == null) return false;
                var logLines = stackTrack.Split('\n');
                if (logLines == null) return false;
                int defIndex = 0;
                for (var index = 0; index < logLines.Length; index++)
                {
                    var log = logLines[index];

                    if (log.Contains("(at"))
                    {
                        var logLine = log.GetLimitStr(":", ")").ToInt();
                        if (logLine == line)
                        {
                            defIndex = index;
                            break;
                        }
                    }
                }

                for (var index = 0; index < logLines.Length; index++)
                {
                    var log = logLines[index];
                    if (!log.Contains("UnityEngine.Debug") &&
                        !log.Contains("YuoLog"))
                    {
                        if (CheckIn()) return true;
                        if (CheckAt()) return true;
                    }

                    bool CheckAt()
                    {
                        string tag = "(at";
                        if (log.Contains(tag))
                        {
                            //从两个字符串中间取得路径 并去除空格 再去除多余Assets
                            var classPath = log.GetLimitStr(tag, ":").Replace(" ", "").Replace("Assets", "");
                            var logLine = log.GetLimitStr(":", ")").ToInt();

                            if (index < defIndex)
                            {
                                return false;
                            }

                            InternalEditorUtility.OpenFileAtLineExternal(Application.dataPath + classPath, logLine);
                            return true;
                        }

                        return false;
                    }

                    bool CheckIn()
                    {
                        string tagIn = " in ";
                        if (log.Contains(tagIn) && log.Contains(".cs:") && log.Contains(" at "))
                        {
                            var classPath = log.GetLimitStr(tagIn, ":").Replace(" ", "");
                            //将\替换为/
                            //将//替换为/
                            classPath = classPath.Replace("\\", "/");
                            classPath = classPath.Replace("//", "/");

                            var logLine = log.Split(".cs:")[1].ToInt();
                            InternalEditorUtility.OpenFileAtLineExternal(classPath, logLine);
                            return true;
                        }

                        return false;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        static string GetStackTrace()
        {
            //UnityEditor.ConsoleWindow
            var consoleWndType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            //找到成员
            var fieldInfo = consoleWndType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var consoleWnd = fieldInfo.GetValue(null);
                if (consoleWnd == null)
                    return "";
                // 如果console窗口时焦点窗口的话，获取stacktrace
                if ((EditorWindow)consoleWnd == EditorWindow.focusedWindow)
                {
                    fieldInfo = consoleWndType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    return fieldInfo.GetValue(consoleWnd).ToString();
                }
            }

            return "";
        }
    }
}