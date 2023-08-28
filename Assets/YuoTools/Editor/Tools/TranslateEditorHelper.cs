using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using YuoTools.Extend.Helper;

namespace YuoTools.Editor.Tools
{
    public class TranslateEditorHelper : OdinEditorWindow
    {
        [MenuItem("Tools/TranslateEditorHelper")]
        private static void OpenWindow()
        {
            var window = GetWindow<TranslateEditorHelper>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 1000);
            window.Show();
        }

        [Button("生成")]
        [HorizontalGroup("1", width: 50)]
        public void Init()
        {
            //获取场景中所有的物体
            var allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allTranslate.Clear();
            //判断物体上是否挂载了TranslateComponent组件
            foreach (var obj in allGameObjects)
            {
                if (obj.TryGetComponent<TranslateComponent>(out var translate))
                {
                    allTranslate.Add(translate.gameObject, translate.ID);
                }
            }

            Serialize();
        }

        [Button("序列化")]
        [HorizontalGroup("1", width: 50)]
        public void Serialize()
        {
            text = new Dictionary<string, string>();
            image = new Dictionary<string, string>();
            sound = new Dictionary<string, string>();
            foreach (var tran in allTranslate)
            {
                var com = tran.Key.GetComponent<TranslateComponent>();
                if (com == null) continue;
                switch (com.TranslateType)
                {
                    case TranslateType.None:
                        break;
                    case TranslateType.Text or TranslateType.TextMeshProUGUI:
                        text.Add(com.ID, "");
                        break;
                    case TranslateType.Image:
                        image.Add(com.ID, "");
                        break;
                    case TranslateType.Sound:
                        break;
                    case TranslateType.Video:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            copyPanel = "";
            copyPanel += ConvertJsonString(JsonConvert.SerializeObject(image));
            copyPanel += ConvertJsonString(JsonConvert.SerializeObject(text));
        }

        [HorizontalGroup("dic")] public Dictionary<string, string> text = new();
        [HorizontalGroup("dic")] public Dictionary<string, string> image = new();
        [HorizontalGroup("dic")] public Dictionary<string, string> sound = new();

        [Button("加载")]
        [HorizontalGroup("1", width: 50)]
        public void LoadConfig()
        {
            switch (fileType)
            {
                case TranslateFileType.Text:
                    configText = FileHelper.ReadAllText(ConfigPath);
                    break;
                case TranslateFileType.Image:
                    configImage = FileHelper.ReadAllText(ConfigPath);
                    break;
                case TranslateFileType.Sound:
                    configSound = FileHelper.ReadAllText(ConfigPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Button("保存")]
        [HorizontalGroup("1", width: 50)]
        public void SaveConfig()
        {
            configText = ConvertJsonString(configText);
            configImage = ConvertJsonString(configImage);
            configSound = ConvertJsonString(configSound);
            switch (fileType)
            {
                case TranslateFileType.Text:
                    FileHelper.WriteAllText(ConfigPath, configText);
                    break;
                case TranslateFileType.Image:
                    FileHelper.WriteAllText(ConfigPath, configImage);
                    break;
                case TranslateFileType.Sound:
                    FileHelper.WriteAllText(ConfigPath, configSound);
                    break;
            }
        }

        string ConfigPath => fileType switch
        {
            TranslateFileType.Text => $"Assets/AddressableResources/language/language_{Language}.json",
            TranslateFileType.Image => "Assets/AddressableResources/language/language_config_image.json",
            TranslateFileType.Sound => "Assets/AddressableResources/language/language_config_sound.json",
            _ => ""
        };

        [Button]
        [HorizontalGroup("1", width: 100)]
        public void CreateFolder()
        {
            LoadConfig();
            var config = fileType switch
            {
                TranslateFileType.Text => configText,
                TranslateFileType.Image => configImage,
                TranslateFileType.Sound => configSound,
                _ => ""
            };
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
            if (fileType != TranslateFileType.Text)
            {
                string type = fileType switch
                {
                    TranslateFileType.Image => "Textures",
                    TranslateFileType.Sound => "Sounds",
                    _ => ""
                };
                List<string> paths = new List<string>();
                foreach (var item in data.Values)
                {
                    //add no repeat
                    if (!paths.Contains(item))
                    {
                        paths.Add(item);
                    }
                }

                CreateFolder(paths, Language, type);
                CreateFolder(paths, "en", type);
            }
        }

        void CreateFolder(List<string> oldPaths, string lan, string type = "Textures")
        {
            //copy list
            var newPaths = new List<string>(oldPaths);
            for (var index = 0; index < newPaths.Count; index++)
            {
                newPaths[index] = $"Assets/Resources/{type}/{lan}/{newPaths[index]}";
                if (!newPaths[index].EndsWith("/"))
                {
                    newPaths[index] += "/";
                }

                Debug.Log(newPaths[index]);
            }

            foreach (var path in newPaths)
            {
                FileHelper.CreateDirectoryPath(path);
            }
        }

        [HorizontalGroup("Top")] public string Language = "zh-cn";

        [EnumToggleButtons] [HorizontalGroup("Top")]
        public TranslateFileType fileType = TranslateFileType.Text;

        public enum TranslateFileType
        {
            Text,
            Image,
            Sound,
        }

        /// <summary>
        ///   格式化json字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }

            return str;
        }

        [HorizontalGroup("1")]
        public Dictionary<GameObject, string> allTranslate = new Dictionary<GameObject, string>();

        [HorizontalGroup()] [MultiLineProperty(Lines = 999)] [LabelWidth(60)]
        public string copyPanel = "";

        // [HorizontalGroup()] [MultiLineProperty(Lines = 999)] [LabelWidth(60)]
        // public string config = "";
        [HorizontalGroup()]
        [MultiLineProperty(Lines = 999)]
        [LabelWidth(60)]
        [ShowIf("fileType", TranslateFileType.Text)]
        public string configText = "";

        [HorizontalGroup()]
        [MultiLineProperty(Lines = 999)]
        [LabelWidth(60)]
        [ShowIf("fileType", TranslateFileType.Image)]
        public string configImage = "";

        [HorizontalGroup()]
        [MultiLineProperty(Lines = 999)]
        [LabelWidth(60)]
        [ShowIf("fileType", TranslateFileType.Sound)]
        public string configSound = "";
    }
}