using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Main.Ecs
{
    public class YuoSaveComponent : YuoComponentInstance<YuoSaveComponent>
    {
        #region SaveInfo

        [HorizontalGroup()] public readonly List<string> SaveTypeKeys = new();
        [HorizontalGroup()] public readonly List<Type> SaveTypes = new();
        [HorizontalGroup()] public readonly List<YuoSaveAttribute> SaveTypeInfo = new();

        public Type GetType(string key)
        {
            if (SaveTypeKeys.Contains(key))
            {
                return SaveTypes[SaveTypeKeys.IndexOf(key)];
            }

            return null;
        }

        public string GetKey(Type type)
        {
            if (SaveTypes.Contains(type))
            {
                return SaveTypeKeys[SaveTypes.IndexOf(type)];
            }

            return type.FullName;
        }

        void Add(string key, Type type)
        {
            if (!SaveTypeKeys.Contains(key))
            {
                SaveTypeKeys.Add(key);
                SaveTypes.Add(type);
            }
        }

        void Remove(string key)
        {
            if (SaveTypeKeys.Contains(key))
            {
                SaveTypes.RemoveAt(SaveTypeKeys.IndexOf(key));
                SaveTypeKeys.Remove(key);
            }
        }

        public void Init()
        {
            "开始初始化保存组件".Log();

            foreach (var type in YuoWorld.Instance.GetAllComponents().Keys)
            {
                //注册组件
                var save = type.GetAttribute<YuoSaveAttribute>();
                if (save != null)
                {
                    if (save.haveName)
                    {
                        Add(save.SaveName, type);
                    }
                    else
                    {
                        Add(type.FullName, type);
                    }

                    SaveTypeInfo.Add(save);
                }
            }
        }

        #endregion

        #region 文件操作

        public string savePath = "";

        void SaveText(string text, string fileName, string path = "/GameSave")
        {
            FileHelper.WriteAllText(savePath + $"{path}/{fileName}.json", text);
            $"{fileName}--保存成功--{text}".Log();
        }

        string LoadText(string fileName, string path = "/GameSave")
        {
            //判断文件夹是否存在，不存在则创建
            FileHelper.CreateDirectoryPath(savePath + path);
            var text = FileHelper.ReadAllText(savePath + $"{path}/{fileName}.json");
            $"{fileName}--读取成功--{text}".Log();
            return text;
        }

        #endregion

        #region 序列化和反序列化

        private string Serialize<T>(T obj, SerializeType type)
        {
            return type switch
            {
                SerializeType.JsonUtility => JsonUtility.ToJson(obj),
                SerializeType.NewtonsoftJson => JsonConvert.SerializeObject(obj),
                _ => JsonUtility.ToJson(obj)
            };
        }

        string SerializeComponent(YuoComponent component, Type type)
        {
            return Serialize(component, SaveTypeInfo[SaveTypes.IndexOf(type)].serializeType);
        }

        private T Deserialize<T>(string text,
            SerializeType serializeType = SerializeType.NewtonsoftJson, Type type = null)
            where T : class
        {
            return serializeType switch
            {
                SerializeType.JsonUtility => JsonUtility.FromJson(text, type ?? typeof(T)) as T,
                SerializeType.NewtonsoftJson => JsonConvert.DeserializeObject(text, type ?? typeof(T)) as T,
                _ => JsonUtility.FromJson(text, type) as T
            };
        }

        YuoComponent DeserializeComponent(string componentData, Type type)
        {
            return Deserialize<YuoComponent>(componentData, SaveTypeInfo[SaveTypes.IndexOf(type)].serializeType, type);
        }

        /// <summary>
        /// 将实体序列化
        /// </summary>
        /// <param name="entities"></param>
        GameData SerializeEntities(DicList<long, YuoComponent> entities)
        {
            GameData data = new();
            foreach (var entity in entities)
            {
                var id = entity.Key;
                Dictionary<string, string> components;
                if (!data.Entities.ContainsKey(entity.Key))
                {
                    components = new Dictionary<string, string>();
                    data.Entities.Add(id, components);
                }
                else
                {
                    components = data.Entities[id];
                }

                //保存前调用
                YuoWorld.RunSystem<IOnSave>(YuoWorld.Instance.GetEntity(id));

                foreach (YuoComponent component in entity.Value)
                {
                    string typeName = GetKey(component.Type);

                    if (!components.ContainsKey(typeName)) components.Add(typeName, null);

                    components[typeName] = SerializeComponent(component, component.Type);
                }
            }

            return data;
        }

        DicList<long, YuoComponent> GetComponentsOfGroup(string groupName)
        {
            DicList<long, YuoComponent> entities = new();
            foreach (var components in YuoWorld.Instance.GetAllComponents())
            {
                if (SaveTypes.Contains(components.Key) &&
                    SaveTypeInfo[SaveTypes.IndexOf(components.Key)].GroupName == groupName)
                {
                    // $"保存类型 {components.Key.Name}".Log();
                    foreach (var component in components.Value)
                    {
                        entities.AddItem(component.Entity.EntityData.Id, component);
                    }
                }
            }

            return entities;
        }

        DicList<long, YuoComponent> GetComponentsOfType(SaveType saveType)
        {
            DicList<long, YuoComponent> entities = new();
            foreach (var components in YuoWorld.Instance.GetAllComponents())
            {
                if (SaveTypes.Contains(components.Key) &&
                    SaveTypeInfo[SaveTypes.IndexOf(components.Key)].saveType == saveType)
                {
                    $"保存类型 {components.Key.Name}".Log();
                    foreach (var component in components.Value)
                    {
                        entities.AddItem(component.Entity.EntityData.Id, component);
                    }
                }
            }

            return entities;
        }

        public void SaveDataOfType(SaveType saveType, string fileName = "")
        {
            if (savePath.IsNullOrSpace())
            {
                "请先设置保存路径".LogError();
                return;
            }

            var entities = GetComponentsOfType(saveType);
            var data = SerializeEntities(entities);
            string path = savePath;
            switch (saveType)
            {
                case SaveType.Config:
                    path = savePath + (fileName.IsNullOrSpace()
                        ? $"/{SaveType.Config}.json"
                        : $"/{SaveType.Config}/{fileName}.json");
                    break;
                case SaveType.SettingInfo:
                    path = savePath + (fileName.IsNullOrSpace()
                        ? $"/{SaveType.SettingInfo}.json"
                        : $"/{SaveType.SettingInfo}/{fileName}.json");
                    break;
                case SaveType.SaveData:
                    break;
            }

            FileHelper.WriteAllText(path, JsonConvert.SerializeObject(data));
        }

        public void LoadDataOfType(SaveType saveType, string fileName = "")
        {
            if (savePath.IsNullOrSpace())
            {
                "请先设置保存路径".LogError();
                return;
            }

            string path = savePath;
            switch (saveType)
            {
                case SaveType.Config:
                    path = savePath + (fileName.IsNullOrSpace()
                        ? $"/{SaveType.Config}.json"
                        : $"/{SaveType.Config}/{fileName}.json");
                    break;
                case SaveType.SettingInfo:
                    path = savePath + (fileName.IsNullOrSpace()
                        ? $"/{SaveType.SettingInfo}.json"
                        : $"/{SaveType.SettingInfo}/{fileName}.json");
                    break;
                case SaveType.SaveData:
                    break;
            }

            string text = FileHelper.ReadAllText(path);

            if (text.IsNullOrSpace()) return;

            GameData data = Deserialize<GameData>(text);
            List<YuoEntity> newEntities = new();
            foreach (var entityData in data.Entities)
            {
                var entity = YuoWorld.Instance.GetEntity(entityData.Key);
                if (entity == null)
                {
                    entity = new YuoEntity(entityData.Key);
                    newEntities.Add(entity);
                }

                foreach (var componentData in entityData.Value)
                {
                    try
                    {
                        var type = GetType(componentData.Key);
                        YuoComponent c = DeserializeComponent(componentData.Value, type);
                        entity.SetComponent(c);
                    }
                    catch (Exception e)
                    {
                        $"序列化错误 --- {e}".Log();
                    }
                }

                YuoWorld.RunSystem<IOnLoad>(entity);
            }

            foreach (var entity in newEntities)
            {
                if (data.ParentDic.ContainsKey(entity.ID))
                {
                    YuoWorld.Instance.SetParent(entity, YuoWorld.Instance.GetEntity(data.ParentDic[entity.ID]));
                }
            }

            foreach (var entity in newEntities)
            {
                //加载后调用
                YuoWorld.RunSystem<IOnLoadCreate>(entity);
            }
        }

        #endregion


        #region 对非特性实体进行手动保存加载

        public void SaveEntity(YuoEntity saveEntity, string path = "",
            SerializeType serializeType = SerializeType.NewtonsoftJson)
        {
            GameDataForType dataForType = new();
            List<YuoEntity> entities = new();
            FindAll(saveEntity);
            foreach (var entity in entities)
            {
                var id = entity.ID;
                Dictionary<string, string> components = new();
                dataForType.Entities.Add(id, components);
                //保存前调用
                YuoWorld.RunSystem<IOnSave>(YuoWorld.Instance.GetEntity(id));

                foreach (YuoComponent component in entity.Components.Values)
                {
                    string typeName = component.Name;

                    if (!components.ContainsKey(typeName)) components.Add(typeName, null);
                    components[typeName] = Serialize(component, serializeType);
                    if (!dataForType.TypeDic.ContainsKey(typeName)) dataForType.TypeDic.Add(typeName, component.Type);
                }
            }

            UpdateParentOfType(dataForType);

            //保存文件
            SaveText(Serialize(dataForType, serializeType), path + saveEntity.EntityName);

            //获取实体和所有子实体的组件
            void FindAll(YuoEntity temp)
            {
                entities.AddRange(temp.Children);
                foreach (var item in temp.Children)
                {
                    FindAll(item);
                }
            }
        }

        public void LoadEntity(YuoEntity saveEntity, string path = "",
            SerializeType serializeType = SerializeType.NewtonsoftJson)
        {
            var gameData = LoadFileOfType(saveEntity.EntityName, serializeType, path + saveEntity.EntityName);
            List<YuoEntity> newEntities = new();
            foreach (var entityData in gameData.Entities)
            {
                var entity = YuoWorld.Instance.GetEntity(entityData.Key);
                if (entity != null)
                {
                    newEntities.Add(entity);
                }
            }

            foreach (var entity in newEntities)
            {
                if (gameData.ParentDic.ContainsKey(entity.ID))
                {
                    YuoWorld.Instance.SetParent(entity, YuoWorld.Instance.GetEntity(gameData.ParentDic[entity.ID]));
                }
            }

            foreach (var entityData in gameData.Entities)
            {
                var entity = YuoWorld.Instance.GetEntity(entityData.Key);
                if (entity == null)
                {
                    entity = saveEntity.AddChild(entityData.Key);
                    foreach (var componentData in entityData.Value)
                        try
                        {
                            {
                                var type = gameData.TypeDic[componentData.Key];

                                YuoComponent component =
                                    Deserialize<YuoComponent>(componentData.Value, serializeType, type);
                                entity.SetComponent(component);
                            }
                        }
                        catch (Exception e)
                        {
                            $"序列化错误 --- {e}".Log();
                        }

                    YuoWorld.RunSystem<IOnLoadCreate>(entity);
                }
                else
                {
                    foreach (var componentData in entityData.Value)
                    {
                        try
                        {
                            var type = gameData.TypeDic[componentData.Key];

                            YuoComponent component =
                                Deserialize<YuoComponent>(componentData.Value, serializeType, type);
                            entity.SetComponent(component, gameData.TypeDic[componentData.Key]);
                        }
                        catch (Exception e)
                        {
                            $"序列化错误 --- {e}".Log();
                        }
                    }
                }
            }

            foreach (var entity in newEntities)
            {
                //加载后调用
                YuoWorld.RunSystem<IOnLoad>(entity);
            }
        }

        #endregion

        GameDataForType LoadFileOfType(string filename, SerializeType serializeType = SerializeType.NewtonsoftJson,
            string path = "/GameSave")
        {
            var file = savePath + $"{path}/{filename}.json";
            var text = FileHelper.ReadAllText(file);

            return Deserialize<GameDataForType>(text, serializeType) ?? new GameDataForType();
        }

        void UpdateParent(GameData data)
        {
            data.ParentDic.Clear();
            foreach (var entity in data.Entities)
            {
                var parent = YuoWorld.Instance.GetEntity(entity.Key).Parent;
                if (parent != null) data.ParentDic.Add(entity.Key, parent.ID);
            }
        }

        void UpdateParentOfType(GameDataForType data)
        {
            data.ParentDic.Clear();
            foreach (var entity in data.Entities)
            {
                var parent = YuoWorld.Instance.GetEntity(entity.Key).Parent;
                if (parent != null) data.ParentDic.Add(entity.Key, parent.ID);
            }
        }


        #region 数据类

        [Serializable]
        public class GameData
        {
            public Dictionary<long, Dictionary<string, string>> Entities = new();
            public Dictionary<long, long> ParentDic = new();
            public Dictionary<string, string> OtherData = new();
        }

        [Serializable]
        public class GameDataForType
        {
            public Dictionary<long, Dictionary<string, string>> Entities = new();
            public Dictionary<long, long> ParentDic = new();
            public Dictionary<string, Type> TypeDic = new();
        }

        #endregion
    }

    public class YuoSaveLoadSystem : YuoSystem<YuoSaveComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Save;
        protected override void Run(YuoSaveComponent component)
        {
            component.Init();
            component.savePath = $"{Application.persistentDataPath}".Log();

            component.LoadDataOfType(SaveType.Config);
            component.LoadDataOfType(SaveType.SettingInfo);
        }
    }

    public class YuoSaveSaveSystem : YuoSystem<YuoSaveComponent>, IExitGame
    {
        public override string Group => SystemGroupConst.Save;
        protected override void Run(YuoSaveComponent component)
        {
            component.SaveDataOfType(SaveType.Config);
            component.SaveDataOfType(SaveType.SettingInfo);
        }
    }

    /// <summary>
    /// 自动保存组件信息,需要继承YuoComponent才会被调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YuoSaveAttribute : Attribute
    {
        public readonly SaveType saveType = SaveType.SaveData;
        public readonly SerializeType serializeType = SerializeType.JsonUtility;
        public readonly string SaveName;
        public readonly bool haveName;
        public readonly string GroupName = SaveGroup.Save;

        public YuoSaveAttribute(SaveType saveType = SaveType.SaveData,
            SerializeType serializeType = SerializeType.JsonUtility,
            string saveName = null, string groupName = SaveGroup.Save)
        {
            this.saveType = saveType;
            this.serializeType = serializeType;
            this.SaveName = saveName;
            this.GroupName = groupName;
            haveName = !string.IsNullOrEmpty(saveName);
        }
    }

    public enum SaveType
    {
        Config = 0,
        SettingInfo = 1,
        SaveData = 2
    }

    public enum SerializeType
    {
        JsonUtility = 0,
        NewtonsoftJson = 1,
        Protobuf = 2,
        JsonNode = 3,
        SampleJson = 4
    }
}