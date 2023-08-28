using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ET;
using Newtonsoft.Json;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Main.Ecs
{
    [Serializable]
    public partial class SaveManagerComponent : YuoComponentInstance<SaveManagerComponent>
    {
        public List<string> saveTypeKeys = new();
        public List<Type> SaveTypes = new();
        public List<YuoSaveAttribute> SaveTypeInfo = new();
        public string savePath = "";

        //--------逻辑--------

        [SerializeField] Dictionary<string, GameData> _allData = new();

        public GameData data = new();

        public Type GetType(string key)
        {
            if (saveTypeKeys.Contains(key))
            {
                return SaveTypes[saveTypeKeys.IndexOf(key)];
            }

            return null;
        }

        public string GetKey(Type type)
        {
            if (SaveTypes.Contains(type))
            {
                return saveTypeKeys[SaveTypes.IndexOf(type)];
            }

            return type.FullName;
        }

        void Add(string key, Type type)
        {
            if (!saveTypeKeys.Contains(key))
            {
                saveTypeKeys.Add(key);
                SaveTypes.Add(type);
            }
        }

        void Remove(string key)
        {
            if (saveTypeKeys.Contains(key))
            {
                SaveTypes.RemoveAt(saveTypeKeys.IndexOf(key));
                saveTypeKeys.Remove(key);
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

        public void SaveSceneDataOfIndex(int id)
        {
            if (!_allData.ContainsKey(Ecs.SaveGroup.Save)) _allData.Add(Ecs.SaveGroup.Save, new GameData());
            data = _allData[$"Save/{Ecs.SaveGroup.Save}_{id}"];
            DicList<long, YuoComponent> entities = new();
            foreach (var entity in YuoWorld.Scene.Children)
            {
                foreach (var component in entity.Components)
                {
                    if (SaveTypes.Contains(component.Key))
                    {
                        entities.AddItem(component.Value.Entity.EntityData.Id, component.Value);
                    }
                }
            }

            SerializeEntities(entities);
            UpdateParent(data);
            SaveFile();
        }

        public int SaveCount
        {
            get
            {
                int count = 0;
                foreach (var dir in FileHelper.GetChildDirectory(savePath))
                {
                    if (dir.StartsWith(savePath + "\\Save_"))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public async ETTask<FileInfo> GetSaveInfo(int id)
        {
            var path = $"{savePath}/Save_{id}/Save.json";
            await FileHelper.CheckOrCreateFilePathAsync(path);
            return await FileHelper.GetFileInfo(path);
        }

        public async ETTask<Texture2D> GetSaveImage(int id)
        {
            var path = $"{savePath}/Save_{id}/preview.png";
            if (FileHelper.CheckFilePath(path))
            {
                return await TextureHelper.LoadTexture(path);
            }

            return null;
        }

        public void SaveGroup(string groupName, string path = "/GameSave")
        {
            if (!_allData.ContainsKey(groupName)) _allData.Add(groupName, new GameData());
            data = _allData[groupName];
            //先把需要保存的实体存起来
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

            //序列化实体
            SerializeEntities(entities);
            //保存父子物体关系
            UpdateParent(data);
            //保存文件
            SaveFile(groupName, path);
        }

        /// <summary>
        /// 将实体序列化
        /// </summary>
        /// <param name="entities"></param>
        void SerializeEntities(DicList<long, YuoComponent> entities)
        {
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
        }

        /// <summary>
        /// 保存一个实体和所有子物体的所有组件,不需要挂载yuoSave特性
        /// </summary>
        public void SaveEntity(YuoEntity saveEntity,
            SerializeType serializeType = SerializeType.NewtonsoftJson)
        {
            GameDataForType dataForType = new();
            dataForType.Entities = new();
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
            SaveText(Serialize(dataForType, serializeType), saveEntity.EntityName);

            void FindAll(YuoEntity temp)
            {
                entities.AddRange(temp.Children);
                foreach (var item in temp.Children)
                {
                    FindAll(item);
                }
            }
        }

        public void LoadEntity(YuoEntity saveEntity,
            SerializeType serializeType = SerializeType.NewtonsoftJson)
        {
            var gameData = LoadFileOfType(saveEntity.EntityName, serializeType);
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

        void SaveFile(string groupName = Ecs.SaveGroup.Save, string path = "/GameSave")
        {
            //保存文本文件到路径
            if (_allData.TryGetValue(groupName, out GameData gamData))
            {
                string text = JsonConvert.SerializeObject(gamData);
                SaveText(text, groupName, path);
            }
        }

        void SaveText(string text, string fileName, string path = "/GameSave")
        {
            //判断文件夹是否存在，不存在则创建
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            //保存文件
            File.WriteAllText(savePath + $"{path}/{fileName}.json", text);
            $"{fileName}--保存成功--{text}".Log();
        }

        async Task SaveFile(GameData gamData, int id)
        {
            //保存文本文件到路径
            string text = JsonConvert.SerializeObject(gamData);
            string path = savePath + $"/Save_{id}/Save.json";
            await FileHelper.CheckOrCreateFilePathAsync(path);
            //保存文件
            await File.WriteAllTextAsync(path, text);
        }

        public void LoadScene(string groupName = Ecs.SaveGroup.Save)
        {
            LoadFile(groupName);
            if (_allData.ContainsKey(groupName))
            {
                data = _allData[groupName];
                LoadOfParent(YuoWorld.Scene);
            }
        }

        public GameData LoadSceneData(int id)
        {
            string group = $"{Ecs.SaveGroup.Save}_{id}/Save";
            LoadFile(group);
            if (_allData.ContainsKey(group))
            {
                return _allData[group];
            }

            return null;
        }

        public void LoadFile(string groupName = Ecs.SaveGroup.Save, string path = "/GameSave")
        {
            var file = savePath + $"{path}/{groupName}.json";

            FileHelper.CheckOrCreateFile(file);

            var text = File.ReadAllText(file);

            if (!_allData.ContainsKey(groupName))
                _allData.Add(groupName, null);
            _allData[groupName] = JsonConvert.DeserializeObject<GameData>(text) ?? new GameData();
        }

        GameDataForType LoadFileOfType(string filename, SerializeType serializeType = SerializeType.NewtonsoftJson,
            string path = "/GameSave")
        {
            var file = savePath + $"/GameSave/{filename}.json";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
            }

            var text = File.ReadAllText(file);
            return Deserialize<GameDataForType>(text, serializeType) ?? new GameDataForType();
        }

        GameData P_LoadFile(string path)
        {
            var file = path;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(file, "");
            }

            var text = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<GameData>(text) ?? new GameData();
        }

        string SerializeComponent(YuoComponent component, Type type)
        {
            return Serialize(component, SaveTypeInfo[SaveTypes.IndexOf(type)].serializeType);
        }

        private string Serialize<T>(T obj, SerializeType type)
        {
            return type switch
            {
                SerializeType.JsonUtility => JsonUtility.ToJson(obj),
                SerializeType.NewtonsoftJson => JsonConvert.SerializeObject(obj),
                _ => JsonUtility.ToJson(obj)
            };
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

        public void LoadOfParent(YuoEntity parent)
        {
            if (data == null) return;
            List<YuoEntity> newEntities = new();
            foreach (var entityData in data.Entities)
            {
                var entity = YuoWorld.Instance.GetEntity(entityData.Key);
                if (entity == null)
                {
                    entity = parent.AddChild(entityData.Key);
                    List<YuoComponent> components = new();
                    foreach (var componentData in entityData.Value)
                    {
                        var type = GetType(componentData.Key);
                        YuoComponent component = DeserializeComponent(componentData.Value, type);
                        // var component = JsonUtility.FromJson(componentData.Value, type) as YuoComponent;
                        entity.SetComponent(component);
                        components.Add(component);
                    }

                    newEntities.Add(entity);
                }
                else
                {
                    foreach (var componentData in entityData.Value)
                    {
                        try
                        {
                            var type = GetType(componentData.Key);
                            YuoComponent c = DeserializeComponent(componentData.Value, type);
                            entity.SetComponent(c);
                            // World.RunSystem<IOnLoad>(c);
                        }
                        catch (Exception e)
                        {
                            $"序列化错误 --- {e}".Log();
                        }
                    }

                    YuoWorld.RunSystem<IOnLoad>(entity);
                }
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

        public async Task CreateData(int id)
        {
            GameData newData = new();
            newData.OtherData.Add(OtherDataKey.Days, "1");
            newData.OtherData.Add(OtherDataKey.SaveTime, "1");
            await SaveFile(newData, id);
        }

        public string GetFilePath(string path) => $"{savePath}/{path}";

        public class OtherDataKey
        {
            public const string Days = "天数";
            public const string SaveTime = "保存时间";
        }

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
    }

    public class SaveManagerSystem : YuoSystem<SaveManagerComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Save;
        protected override void Run(SaveManagerComponent component)
        {
            component.Init();
            component.savePath = $"{Application.persistentDataPath}".Log();

            // foreach (var dir in await FileHelper.GetAllDirectory(Application.persistentDataPath))
            // {
            //     Debug.Log(dir);
            // }


            YuoWorld.Main.GetComponent<YuoInputComponent>().Add(new("Save")
            {
                key = KeyCode.Alpha1, OnDown = () =>
                {
                    component.SaveGroup(SaveGroup.Config);
                    "保存完成".Log();
                }
            });

            YuoWorld.Main.GetComponent<YuoInputComponent>().Add(new("Load")
            {
                key = KeyCode.Alpha2, OnDown = () =>
                {
                    component.LoadScene(SaveGroup.Config);
                    "读档完成".Log();
                }
            });
            component.LoadScene(SaveGroup.Config);
        }
    }

    public class SaveManagerExitGameSystem : YuoSystem<SaveManagerComponent>, IExitGame
    {
        public override string Group => SystemGroupConst.Save;
        protected override void Run(SaveManagerComponent component)
        {
            component.SaveGroup(SaveGroup.Config);
        }
    }

    public static partial class SaveGroup
    {
        /// <summary>
        /// 游戏存档
        /// </summary>
        public const string Save = "Save";

        /// <summary>
        /// 游戏设置
        /// </summary>
        public const string Setting = "Setting";

        /// <summary>
        /// 配置文件
        /// </summary>
        public const string Config = "Config";

        /// <summary>
        /// 语言文件
        /// </summary>
        public const string Language = "Language";
    }

    /// <summary>
    /// 加载后调用
    /// </summary>
    public interface IOnLoad : ISystemTag
    {
    }

    /// <summary>
    /// 加载后添加实体时调用
    /// </summary>
    public interface IOnLoadCreate : ISystemTag
    {
    }

    /// <summary>
    /// 保存之前调用
    /// </summary>
    public interface IOnSave : ISystemTag
    {
    }
}