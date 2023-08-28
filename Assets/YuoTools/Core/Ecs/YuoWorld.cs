using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuoTools.Main.Ecs
{
    public class YuoWorld
    {
        public static YuoWorld Instance { get; private set; }

        public static YuoEntity Main { get; private set; }

        #region Systems
        
        //system不需要indexof等耗时操作,所以直接用list

        private List<SystemBase> allSystem;

        private MultiMap<Type, SystemBase> systemsOfComponent;

        private MultiMap<Type, SystemBase> systemsOfTag;

        private MultiMap<Type, SystemBaseBase> systemsOfBaseComponent;

        #endregion

        public Dictionary<long, YuoEntity> Entities { get; private set; }
        
        //component需要频繁的indexof等耗时操作,且不需要多个相同的component和排序,所以用hashset

        private MultiHashSetMap<Type, YuoComponent> components;

        public static YuoEntity Scene => Instance.AllScenes[0];
        
        //scene entity 数量很少,所以无所谓

        public readonly List<YuoEntity> AllScenes = new();
        
        //不需要排序,不会有重复的
        private readonly List<YuoEntity> entityTrash = new();

        private readonly List<YuoComponent> componentsTrash = new();
        
        //不需要排序,不会有重复的
        private MultiHashSetMap<Type, Type> linkComponent = new();

        //不需要排序,不会有重复的,且频繁Indexof,所以用hashset
        private HashSet<Type> singleComponents = new();

        [Sirenix.OdinInspector.ShowInInspector]
        private Dictionary<Type, Type> baseComponents = new();

        private bool _worldIsDestroy;

        #region 基础生命周期

        public void OnInit()
        {
            allSystem = new();
            Entities = new();
            components = new();
            systemsOfTag = new();
            systemsOfComponent = new();
            systemsOfBaseComponent = new();

            Instance = this;

            //系统初始化必须放在所有初始化之前
            Initialization();

            //基本核心组件挂载的实体
            Main = new YuoEntity(0);

            Main.EntityName = "核心组件";

            //添加一个场景实体
            var scene = new YuoEntity(IDGenerate.GetID(IDGenerate.IDType.Scene, 0));

            scene.EntityName = "默认场景";
            AllScenes.Add(scene);

            scene.AddComponent<SceneComponent>();
        }

        public void OnDestroy()
        {
            _worldIsDestroy = true;
            //退出前会执行ExitGame系统
            RunSystemForAllEntity<IExitGame>();

            //转换成List，防止在Dispose中修改Entities产生的错误
            foreach (var entity in Entities.Values.ToList())
            {
                entity?.Dispose();
            }

            //清理一下所有集合
            Entities.Clear();
            components.Clear();
            systemsOfTag.Clear();
            systemsOfComponent.Clear();
            systemsOfBaseComponent.Clear();
            allSystem.Clear();

            //清除所有Scene
            AllScenes.Clear();
            Instance = null;
            //手动GC
            GC.Collect();
            "Game Exit".Log();
        }

        public void Update()
        {
            ClearTrash();
            if (startSystems.Count > 0)
            {
                List<YuoComponent> startTemps = new List<YuoComponent>();
                startTemps.AddRange(startSystems);
                foreach (var component in startTemps)
                {
                    //添加链接组件
                    if (linkComponent.TryGetValue(component.Type, out var linkTypes))
                    {
                        foreach (var linkType in linkTypes)
                        {
                            component.Entity.AddComponent(linkType);
                        }
                    }
                }

                var startType = typeof(IStart);
                foreach (var component in startTemps)
                {
                    try
                    {
                        RunSystem(startType, component);
                    }
                    catch (Exception e)
                    {
                        e.LogError();
                    }
                }
            }

            ClearTrash();
            startSystems.Clear();

            RunSystemForAllEntity(SystemType.Update);
        }

        public void FixedUpdate()
        {
            ClearTrash();
            RunSystemForAllEntity(SystemType.FixedUpdate);
            ClearTrash();
        }

        public void LateUpdate()
        {
            ClearTrash();
            RunSystemForAllEntity(SystemType.LateUpdate);
            ClearTrash();
        }

        #endregion

        void CoverSystem()
        {
            //先查找所有需要覆盖的system类型
            var coverSystems = new List<Type>();
            foreach (var system in allSystem)
            {
                var coverSystem = system.GetType().GetCustomAttribute<CoverSystemAttribute>();
                if (coverSystem != null)
                {
                    coverSystems.Add(coverSystem.CoverType);
                }
            }

            //再在所有system中查找需要覆盖的system
            //先创建一个临时system集合,用于存放需要覆盖的system
            var tempSystem = new List<SystemBase>();
            foreach (var system in allSystem)
            {
                if (coverSystems.Contains(system.Type))
                    tempSystem.Add(system);
            }

            //再把临时system集合中的system移除
            foreach (var system in tempSystem)
            {
                RemoveSystem(system);
            }

            systemDic.Clear();
            foreach (var systemBase in allSystem)
            {
                systemDic.Add(systemBase.Type, systemBase);
            }
        }

        private Dictionary<Type, SystemBase> systemDic = new Dictionary<Type, SystemBase>();

        void RemoveSystem(SystemBase system)
        {
            allSystem.Remove(system);
            systemsOfTag.Remove(system.GetType());
            systemsOfComponent.Remove(system.GetType());
        }

        void ClearTrash()
        {
            if (componentsTrash.Count == 0 && entityTrash.Count == 0) return;

            foreach (var yuoComponent in componentsTrash)
            {
                yuoComponent.Dispose();
            }

            foreach (var entity in entityTrash)
            {
                entity.Dispose();
            }

            entityTrash.Clear();
        }

        #region AddAndRemove

        public static void DestroyEntity(YuoEntity entity)
        {
            Instance.entityTrash.Add(entity);
        }

        public static void DestroyComponent(YuoComponent component)
        {
            Instance.componentsTrash.Add(component);
        }

        public static void DestroyEntityForce(YuoEntity entity)
        {
            entity?.Dispose();
        }

        public static void DestroyComponentForce(YuoComponent component)
        {
            component?.Dispose();
        }

        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterEntity(YuoEntity entity)
        {
            if (!Entities.ContainsKey(entity.EntityData.Id))
            {
                Entities.Add(entity.EntityData.Id, entity);
            }
            else
                $"实体ID重复，请检查：{entity.EntityData.Id}".LogError();
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity"></param>
        public void UnRegisterEntity(YuoEntity entity)
        {
            if (Entities.ContainsKey(entity.EntityData.Id))
            {
                Entities.Remove(entity.EntityData.Id);
            }
        }

        public void RemoveComponent(YuoEntity entity, YuoComponent component)
        {
            components[component.Type].Remove(component);
            //World销毁时,不调用Destroy,只调用ExitGame
            if (!_worldIsDestroy)
            {
                foreach (var system in systemsOfComponent[component.Type])
                {
                    if (system is IDestroy)
                    {
                        system.RunType = SystemType.Destroy;
                        system.m_Run(entity);
                    }

                    system.RemoveComponent(entity);
                }
            }

            component.Dispose();
        }

        /// <summary>
        /// 将一个组件手动添加到实体上,这里一般是在实体创建时自动调用
        /// </summary>
        public void AddComponent(YuoEntity entity, YuoComponent component)
        {
            AddComponent(entity, component, component.Type);
        }

        /// <summary>
        /// 将一个组件手动添加到实体上,Type可指定组件类型
        /// </summary>
        public void AddComponent(YuoEntity entity, YuoComponent component, Type componentType)
        {
            if (!components.ContainsKey(componentType))
            {
                components.Add(componentType, new());
            }

            if (!components[componentType].Contains(component))
            {
                if (baseComponents.TryGetValue(componentType, out var baseType))
                {
                    component.BaseComponentType = baseType;
                    entity.BaseComponents.AddItem(baseType, component);
                    // $"添加基础组件：{baseType.Name} --- {componentType.Name}--- {entity.EntityName}".Log();
                    RunAwakeSystemOfBase(entity, baseType);
                }

                components[componentType].Add(component);
                //Run System
                RunAwakeSystem(entity, componentType);

                startSystems.Add(component);

                if (singleComponents.Contains(componentType))
                {
                    foreach (var yuoComponent in components[componentType])
                    {
                        yuoComponent.Destroy();
                    }
                }
            }
        }

        public void SetComponent(YuoComponent component1, YuoComponent component2)
        {
            YuoEntity entity = component1.Entity;
            var list = components[component1.Type];
            list.Remove(component1);
            list.Add(component2);
            // list[list.IndexOf(component1)] = component2;

            foreach (var system in systemsOfComponent[component1.Type])
            {
                system.SetComponent(entity, component1.Type, component2);
            }

            RunSystemOfTag<ISwitchComponent>(component2);
        }

        /// <summary>
        ///  IStart需要在所有组件添加完成后,在每一帧的末尾才会调用
        /// </summary>
        private readonly List<YuoComponent> startSystems = new();

        #endregion

        /// <summary>
        ///  初始化系统
        /// </summary>
        /// <param name="system"></param>
        /// <param name="type"></param>
        internal void RegisterSystem(SystemBase system, Type type)
        {
            systemsOfComponent.AddItem(type, system);
        }

        internal void RegisterBaseSystem(SystemBaseBase system, Type type)
        {
            systemsOfBaseComponent.AddItem(type, system);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public YuoEntity GetEntity(long instanceId)
        {
            Entities.TryGetValue(instanceId, out YuoEntity entity);
            return entity;
        }

        public YuoEntity GetEntity(string entityName)
        {
            Entities.TryGetValue(entityName.GetHashCode(), out YuoEntity entity);
            return entity;
        }

        /// <summary>
        /// 尽量不修改父物体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        public void SetParent(YuoEntity entity, YuoEntity parent)
        {
            entity.SetParent(parent);
        }

        /// <summary>
        /// 根据ID获取组件
        /// </summary>
        /// <param name="instanceId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>(long instanceId) where T : YuoComponent
        {
            return GetEntity(instanceId)?.GetComponent<T>();
        }

        public HashSet<YuoComponent> GetAllComponents<T>() where T : YuoComponent
        {
            return components.ContainsKey(typeof(T)) ? components[typeof(T)] : null;
        }

        readonly Dictionary<string, Type> allComponentType = new();

        public Type GetComponentType(string typeName)
        {
            return allComponentType.TryGetValue(typeName, out var value) ? value : null;
        }

        /// <summary>
        /// 初始化主要数据
        /// </summary>
        private void Initialization()
        {
            //获取所有程序集
            //List<Type> types = new();
            //types.AddRange(Assembly.GetCallingAssembly().GetTypes());
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var yuoTool = Assembly.Load("YuoTools");
            //加载Assembly-CSharp程序集,获取所有的组件类型
            var assemblyCSharp = Assembly.Load("Assembly-CSharp");
            LoadAssembly(yuoTool);
            LoadAssembly(assemblyCSharp);
            //所有system和component的加载完成
            //计算需要覆盖的system
            CoverSystem();
            //开始排序并且将system添加到对应的池中
            SystemSort();
            //计算需要Link的组件
            GetLinkComponent();
            //计算仅需要一个的组件
            GetSingleComponent();
        }

        private short GetOrder(Type type)
        {
            return type.GetCustomAttribute<SystemOrderAttribute>()?.Order ?? 0;
        }

        public void LoadAssembly(Assembly assembly)
        {
            LoadTypes(assembly.GetTypes());
        }

        void CreateSystem(SystemBase system)
        {
            if (!allSystem.Contains(system))
            {
                allSystem.Add(system);
            }
        }

        void CreateComponent(Type type)
        {
            allComponentType.TryAdd(type.Name, type);
            if (!components.ContainsKey(type))
            {
                components.Add(type, new());
            }

            var baseType = type.BaseType;
            if (baseType != null && baseType.BaseType == typeof(YuoComponent) &&
                !baseType.Name.Contains("YuoComponentInstance") &&
                !baseType.Name.Contains("YuoComponentGet"))
            {
                baseComponents[type] = baseType;
            }
        }

        public void LoadTypes(Type[] types)
        {
            try
            {
                List<SystemBase> systems = new();

                foreach (var type in types)
                {
                    //获取系统类型
                    var systemBase = CheckSystem<SystemBase>(type);
                    //获取组件类型
                    var yuoComponent = CheckComponent<YuoComponent>(type);

                    //如果是System
                    if (systemBase != null)
                    {
                        systems.Add(systemBase);
                        systemBase.Type = type;
                    }

                    //如果是Component
                    else if (yuoComponent)
                    {
                        CreateComponent(type);
                    }
                }

                // $"初始化程序集 [{assembly.FullName.Replace(" [", "").Split(",")[0]}] 完成--已加载{systems.Count}个系统,{componentCount}个组件--共检索{types.Length}个类型"
                //     .Log();

                foreach (var system in systems)
                {
                    CreateSystem(system);
                }
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        public void SystemSort()
        {
            //从小到大排序
            allSystem.Sort((a, b) => GetOrder(a.Type) - GetOrder(b.Type));

            systemsOfComponent.Clear();

            systemsOfTag.Clear();

            foreach (var system in allSystem)
            {
                system.Clear();
                system.Init(this);
                foreach (var inter in system.Type.GetInterfaces())
                {
                    foreach (var iInter in inter.GetInterfaces())
                    {
                        //将Tag和系统进行关联
                        if (iInter == typeof(ISystemTag))
                        {
                            if (!systemsOfTag.ContainsKey(inter))
                                systemsOfTag.Add(inter, new ());
                            systemsOfTag[inter].Add(system);
                            //记录一下系统的Tag
                            system.systemTags.Add(inter);
                        }
                    }
                }
            }

            foreach (var systems in systemsOfComponent.Values)
            {
                systems.Sort((a, b) => GetOrder(a.Type) - GetOrder(b.Type));
            }

            //重新注册组件系统
            foreach (var system in allSystem)
            {
                foreach (var entity in Entities.Values)
                {
                    system.AddComponent(entity);
                }
            }

            "-所有系统排序完成-".Log();
        }

        public void GetLinkComponent()
        {
            foreach (var type in GetAllComponentOfType().Values)
            {
                var link = type.GetCustomAttributes<LinkComponentOfAttribute>();

                foreach (var linkComponentAttribute in link)
                {
                    linkComponent.AddItem(linkComponentAttribute.LinkType, type);
                }
            }
        }

        public void GetSingleComponent()
        {
            foreach (var type in GetAllComponentOfType().Values)
            {
                var single = type.GetCustomAttributes<SingleComponentAttribute>();
                if (single.Any())
                {
                    singleComponents.Add(type);
                }
            }
        }

        public Dictionary<string, Type> GetAllComponentOfType()
        {
            return allComponentType;
        }

        void RunAwakeSystem(YuoEntity entity, Type componentType)
        {
            foreach (var system in systemsOfComponent[componentType])
            {
                if (system.AddComponent(entity))
                {
                    if (system is IAwake)
                    {
                        system.RunType = SystemType.Awake;
#if UNITY_EDITOR
                        system.StartClock();
#endif
                        try
                        {
                            if (system.Enabled)
                                system.m_Run(system.EntityCount - 1);
#if UNITY_EDITOR
                            system.StopClock();
#endif
                        }
                        catch (Exception e)
                        {
                            e.LogError();
                        }
                    }
                }
            }
        }

        void RunAwakeSystemOfBase(YuoEntity entity, Type baseComponentType)
        {
            if (GetSystemOfBaseComponent(baseComponentType, out var systems))
            {
                foreach (var system in systems)
                {
                    if (system.AddComponent(entity))
                    {
                        if (system.systemTags.Contains(SystemType.Awake))
                        {
                            system.RunType = SystemType.Awake;
#if UNITY_EDITOR
                            system.StartClock();
#endif
                            try
                            {
                                if (system.Enabled)
                                    system.m_Run(system.EntityCount - 1);
#if UNITY_EDITOR
                                system.StopClock();
#endif
                            }
                            catch (Exception e)
                            {
                                e.LogError();
                            }
                        }
                    }
                }
            }
        }

        #region RunSystemForAllEntity

        /// <summary>
        ///  根据tag执行对应的系统(所有实体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RunSystemForAllEntity<T>() where T : ISystemTag
        {
            RunSystemForAllEntity(typeof(T));
        }

        private void RunSystemForAllEntity(Type tagType)
        {
            if (systemsOfTag.TryGetValue(tagType, out var list))
            {
                foreach (var system in list)
                {
                    system.RunType = tagType;
                    system.m_Run();
                }
            }
        }

        public static void RunSystemForAll<T>() where T : ISystemTag
        {
            Instance.RunSystemForAllEntity<T>();
        }

        public static void RunSystemForAll(Type tagType)
        {
            Instance.RunSystemForAllEntity(tagType);
        }

        #endregion

        #region RunSystemForEntity

        private void RunSystemOfTag(Type tagType, YuoEntity entity)
        {
            if(entity.IsDisposed)return;
            if (GetSystemOfTag(tagType, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = tagType;
                    system.m_Run(entity);
                }
            }
        }

        private void RunSystemOfTag<T>(List<YuoEntity> entities) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    foreach (var entity in entities)
                    {
                        if(entity.IsDisposed) continue;
                        system.m_Run(entity);
                    }
                }
            }
        }

        /// <summary>
        ///  根据tag执行对应的system
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void RunSystem<T>(YuoEntity entity) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(entity);
        }

        public static void RunSystem<T>(List<YuoEntity> entities) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(entities);
        }

        /// <summary>
        ///  根据tag类型执行对应的system
        /// </summary>
        /// <param name="tagType"></param>
        /// <param name="entity"></param>
        public static void RunSystem(Type tagType, YuoEntity entity)
        {
            Instance.RunSystemOfTag(tagType, entity);
        }

        /// <summary>
        ///  根据tag执行对应的系统(指定实体身上的所有组件)
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        private void RunSystemOfTag<T>(YuoEntity entity) where T : ISystemTag
        {
            RunSystemOfTag(typeof(T), entity);
        }

        #endregion

        #region RunSystemForComponent

        private void RunSystemOfTag(Type tagType, YuoComponent component)
        {
            if(component.IsDisposed) return; 
            if (GetSystemOfComponent(component.Type, out var systems))
            {
                foreach (var system in systems)
                {
                    if (system.HasTag(tagType))
                    {
                        system.RunType = tagType;
                        system.m_Run(component.Entity);
                    }
                }
            }
        }

        /// <summary>
        ///  根据tag执行对应的系统(指定实体身上的指定组件)
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        private void RunSystemOfTag<T>(YuoComponent component) where T : ISystemTag
        {
            RunSystemOfTag(typeof(T), component);
        }

        /// <summary>
        /// 根据tag执行对应的system,只有和这个组件有关的system才会执行
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public static void RunSystem<T>(YuoComponent component) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(component);
        }

        /// <summary>
        ///  根据tag类型执行对应的system,只有和这个组件有关的system才会执行
        /// </summary>
        /// <param name="tagType"></param>
        /// <param name="component"></param>
        public static void RunSystem(Type tagType, YuoComponent component)
        {
            Instance.RunSystemOfTag(tagType, component);
        }

        #endregion

        #region RunSystemForBaseComponent

        void RunSystemOfBaseComponent(Type tagType, YuoComponent component)
        {
            if(component.IsDisposed) return; 
            if (component.BaseComponentType != null)
            {
                if (GetSystemOfBaseComponent(component.BaseComponentType, out var systems))
                {
                    foreach (var system in systems)
                    {
                        if (system.HasTag(tagType))
                        {
                            system.RunType = tagType;
#if UNITY_EDITOR
                            system.StartClock();
#endif
                            system.m_Run(component);

#if UNITY_EDITOR
                            system.StopClock();
#endif
                        }
                    }
                }
            }
        }

        public static void RunSystemOfBase(Type tagType, YuoComponent component)
        {
            Instance.RunSystemOfBaseComponent(tagType, component);
        }

        public static void RunSystemOfBase<T>(YuoComponent component) where T : ISystemTag
        {
            Instance.RunSystemOfBaseComponent(typeof(T), component);
        }

        #endregion

        #region Get

        /// <summary>
        /// 通过Tag获取系统
        /// </summary>
        private bool GetSystemOfTag(Type type, out List<SystemBase> systems)
        {
            if (systemsOfTag.TryGetValue(type, out var value))
            {
                systems = value;
                return true;
            }

            systems = null;
            return false;
        }

        /// <summary>
        ///  通过组件获取系统
        /// </summary>
        private bool GetSystemOfComponent(Type type, out List<SystemBase> systems)
        {
            if (systemsOfComponent.TryGetValue(type, out var value))
            {
                systems = value;
                return true;
            }

            systems = null;
            return false;
        }

        bool GetSystemOfBaseComponent(Type type, out List<SystemBaseBase> systems)
        {
            if (systemsOfBaseComponent.TryGetValue(type, out var list))
            {
                systems = list;
                return true;
            }

            systems = null;
            return false;
        }

        public MultiHashSetMap<Type, YuoComponent> GetAllComponents() => components;

        public MultiMap<Type, SystemBase> GetAllSystemOfComponent() => systemsOfComponent;

        public MultiMap<Type, SystemBase> GetAllSystemOfTag() => systemsOfTag;

        public List<SystemBase> GetAllSystem => allSystem;

        #endregion

        public static void DisableSystem<T>() where T : SystemBase
        {
            var type = typeof(T);
            var system = Instance.systemDic[type];
            if (system != null)
                system.Enabled = false;
        }

        #region 反射获取组件

        static bool CheckComponent<Tp>(Type find) where Tp : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(Tp).Name) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }

        private static Tp CheckSystem<Tp>(Type find) where Tp : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(Tp).Name)
                {
                    try
                    {
                        object obj = Activator.CreateInstance(find);
                        if (obj != null)
                        {
                            Tp info = obj as Tp;
                            return info;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                }
                else
                {
                    baseType = baseType.BaseType;
                }
            }

            return null;
        }

        #endregion
    }

    public class SceneComponent : YuoComponent
    {
    }
}