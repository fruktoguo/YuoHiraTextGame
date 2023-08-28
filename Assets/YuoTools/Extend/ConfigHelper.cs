using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    [AutoAddToMain()]
    public class ConfigHelper : YuoComponentInstance<ConfigHelper>
    {
        public Dictionary<string, object> data;
        public Dictionary<ConfigEnum, object> dataEnum = new();
        public bool isChange = false;
        public const string ConfigPath = "Config.json";

        public static T GetConfig<T>(string key, T def) where T : new()
        {
            if (Get.data.TryGetValue(key, out var value))
            {
                if (value is T t)
                {
                    return t;
                }
            }

            Get.AddConfig<T>(key, def);
            return def;
        }

        public static T GetConfig<T>(ConfigEnum key, T def) where T : new()
        {
            var list = Get.dataEnum;
            if (list.TryGetValue(key, out var value))
            {
                if (value is T t)
                {
                    return t;
                }
            }

            if (ValueConvert(value, out T to))
            {
                list[key] = to;
                Get.data[key.ToString()] = to;
                Get.isChange = true;
                return to;
            }

            Get.AddConfig(key, def);
            return def;
        }

        public static bool ValueConvert<T>(object from, out T to)
        {
            var toType = typeof(T);
            var fromType = from.GetType();
            if (toType == fromType)
            {
                to = (T)from;
                return true;
            }

            // $"{fromType.Name}无法转换到{toType.Name}".LogError();

            //Int
            if (toType == typeof(int))
            {
                to = (T)(object)Convert.ToInt32(from);
                return true;
            }

            //Float
            if (toType == typeof(float))
            {
                to = (T)(object)Convert.ToSingle(from);
                return true;
            }

            //string
            if (toType == typeof(string))
            {
                to = (T)(object)Convert.ToString(from);
                return true;
            }

            //bool
            if (toType == typeof(bool))
            {
                to = (T)(object)Convert.ToBoolean(from);
                return true;
            }

            //long
            if (toType == typeof(long))
            {
                to = (T)(object)Convert.ToInt64(from);
                return true;
            }

            //double
            if (toType == typeof(double))
            {
                to = (T)(object)Convert.ToDouble(from);
                return true;
            }

            //char
            if (toType == typeof(char))
            {
                to = (T)(object)Convert.ToChar(from);
                return true;
            }

            to = default;
            return false;
        }

        public static T GetConfig<T>(ConfigEnum key) where T : new()
        {
            var list = Get.dataEnum;
            if (list.TryGetValue(key, out var value))
            {
                if (value is T t)
                {
                    return t;
                }
            }

            var def = new T();
            Get.AddConfig(key, def);
            return def;
        }

        public static object GetConfig(ConfigEnum key)
        {
            var list = Get.dataEnum;
            return list.TryGetValue(key, out var value) ? value : key.ToString();
        }

        public static object GetConfig(string key)
        {
            var list = Get.data;
            return list.TryGetValue(key, out var value) ? value : key.LogError();
        }

        private void AddConfig<T>(string key, T def) where T : new()
        {
            if (data.ContainsKey(key))
            {
                return;
            }

            data.Add(key, def);

            isChange = true;
        }

        public static bool ContainsKey(ConfigEnum key)
        {
            return Get.dataEnum.ContainsKey(key);
        }

        public static bool ContainsKey(string key)
        {
            return Get.data.ContainsKey(key);
        }

        private void AddConfig<T>(ConfigEnum key, T def) where T : new()
        {
            if (dataEnum.ContainsKey(key))
            {
                return;
            }

            data.Add(key.ToString(), def);
            dataEnum.Add(key, def);
            isChange = true;
        }

        public ConfigHelper()
        {
            var text = FileHelper.ReadAllText((Application.dataPath + "/" + ConfigPath).Log());
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text) ?? new();
            dataEnum.Clear();
            int index = 0;
            foreach (var (key, value) in data)
            {
                dataEnum.Add((ConfigEnum)index, value);
                index++;
            }
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/生成配置类")]
        static void KeyToClass()
        {
            var data = new Dictionary<string, object>();
            string classPath = Application.dataPath + "/ConfigEnum.cs";
            var text = FileHelper.ReadAllText(Application.dataPath + "/" + ConfigPath);
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text) ?? new();

            string classText = "public enum ConfigEnum\n{\n";
            int index = 0;
            foreach (var (key, value) in data)
            {
                classText += $"\t{key} = {index++},\n";
            }

            classText += "}";
            FileHelper.WriteAllText(classPath, classText);

            Debug.Log("生成成功");

            UnityEditor.AssetDatabase.Refresh();

            KeyToClassLanguage("AddressableResources/language/language_zh-cn.json");
        }

        public static void KeyToClassLanguage(string path)
        {
            var data = new Dictionary<string, object>();
            string classPath = Application.dataPath + "/LanguageEnum.cs";
            var text = FileHelper.ReadAllText(Application.dataPath + "/" + path);
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text) ?? new();

            string classText = "public enum LanguageEnum\n{\n";
            int index = 0;
            foreach (var (key, value) in data)
            {
                classText += $"\t{key} = {index++},\n";
            }

            classText += "}";
            FileHelper.WriteAllText(classPath, classText);

            Debug.Log("生成成功");

            UnityEditor.AssetDatabase.Refresh();
        }

#endif
    }

    public class ConfigHelperDestroySystem : YuoSystem<ConfigHelper>, IExitGame
    {
        public override string Group => SystemGroupConst.Main;

        protected override void Run(ConfigHelper component)
        {
            if (component.isChange)
            {
                // 保存配置
                //格式化Json
                var json = JsonConvert.SerializeObject(component.data, Formatting.Indented);
                FileHelper.WriteAllText(Application.dataPath + "/" + ConfigHelper.ConfigPath, json);
            }
        }
    }

    public static class ConfigEx
    {
        public static T GetConfig<T>(this ConfigEnum config, T def) where T : new()
        {
            return ConfigHelper.GetConfig(config, def);
        }

        public static T GetConfig<T>(this ConfigEnum config) where T : new()
        {
            return ConfigHelper.GetConfig<T>(config);
        }

        public static object GetConfig(this ConfigEnum config)
        {
            return ConfigHelper.GetConfig(config);
        }
    }
}