using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend

{
    [AutoAddToMain]
    public class TranslateManagerComponent : YuoComponentGet<TranslateManagerComponent>
    {
        public Dictionary<string, string> KeyPath = new();
        public Dictionary<LanguageEnum, string> DataOfEnum = new();

        private const string LanguageConfigImage = "language_config_image";
        private const string LanguageConfigSound = "language_config_sound";
        [FilePath] [ShowInInspector] private string _configPath = "";

        public void LoadBasePathFile()
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(LanguageConfigImage);

                handle.WaitForCompletion();

                image = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到图像配置文件");
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(LanguageConfigSound);

                handle.WaitForCompletion();

                image = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到音频配置文件");
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>($"language_{Language}");

                handle.WaitForCompletion();

                text = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到对应的文本配置文件");
            }

            text ??= new Dictionary<string, string>();
            image ??= new Dictionary<string, string>();
            sound ??= new Dictionary<string, string>();

            "本地化组件加载完成".Log();

            foreach (var key in text.Keys)
            {
                if (key.Contains(" "))
                {
                    Debug.LogWarning($"文本配置文件中的 -{key}- 存在空格");
                }
            }

            foreach (var key in sound.Keys)
            {
                if (key.Contains(" "))
                {
                    Debug.LogWarning($"音频配置文件中的 -{key}- 存在空格");
                }
            }

            foreach (var key in image.Keys)
            {
                if (key.Contains(" "))
                {
                    Debug.LogWarning($"图像配置文件中的 -{key}- 存在空格");
                }
            }
        }

        public void Save()
        {
            if (!StringExtension.IsNullOrSpace(_configPath))
            {
                FileHelper.WriteAllText(_configPath, JsonConvert.SerializeObject(image));
            }
        }

        public string Language = "zh-cn";

        public Dictionary<string, string> text = new Dictionary<string, string>();
        public Dictionary<string, string> image = new Dictionary<string, string>();
        public Dictionary<string, string> sound = new Dictionary<string, string>();

        public string LoadText(string id)
        {
            if (text.TryGetValue(id, out string t))
            {
                return t;
            }

            $"没有找到对应的文本--->{id}".LogError();
            return id;
        }

        public Sprite LoadSprite(string id)
        {
            if (image.TryGetValue(id, out string path))
            {
                path = $"Textures/{Language}/{path}/{id}";
                return Resources.Load<Sprite>(path);
            }

            return null;
        }

        public string LoadText(LanguageEnum languageEnum)
        {
            if (DataOfEnum.TryGetValue(languageEnum, out string id))
            {
                return LoadText(id);
            }

            return languageEnum.ToString();
        }
    }

    //awake
    public class TranslateManagerAwakeSystem : YuoSystem<TranslateManagerComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Translate;
        protected override void Run(TranslateManagerComponent component)
        {
            component.LoadBasePathFile();
            component.DataOfEnum.Clear();
            foreach (var languageEnum in EnumHelper.GetValues<LanguageEnum>())
            {
                component.DataOfEnum.Add(languageEnum, languageEnum.ToString());
            }
        }
    }

    public static class YuoLanguageEx
    {
        public static string Language(this string key)
        {
            return TranslateManagerComponent.Get.LoadText(key);
        }

        public static string Language(this LanguageEnum key)
        {
            return TranslateManagerComponent.Get.LoadText(key);
        }

        public static string Language(this LanguageEnum key, params object[] args)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), args);
        }

        public static string Language(this LanguageEnum key, object arg0)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2, object arg3)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2, arg3);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2, object arg3,
            object arg4)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2, arg3, arg4);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2, object arg3,
            object arg4, object arg5)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2, object arg3,
            object arg4, object arg5, object arg6)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string Language(this LanguageEnum key, object arg0, object arg1, object arg2, object arg3,
            object arg4, object arg5, object arg6, object arg7)
        {
            return string.Format(TranslateManagerComponent.Get.LoadText(key), arg0, arg1, arg2, arg3, arg4, arg5, arg6,
                arg7);
        }
    }

    //destroy
    public class TranslateManagerDestroySystem : YuoSystem<TranslateManagerComponent>, IExitGame
    {
        public override string Group => SystemGroupConst.Translate;
        protected override void Run(TranslateManagerComponent component)
        {
            component.Save();
        }
    }
}