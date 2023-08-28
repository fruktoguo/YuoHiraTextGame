using System;
using System.Collections.Generic;
using System.Linq;

namespace YuoTools.Main.Ecs
{
    public partial class YuoEntity : IDisposable
    {
        public YuoEntity Parent { get; private set; }

        public EntityComponent EntityData { get; private set; }
        public Dictionary<Type, YuoComponent> Components { get; } = new();
        public MultiHashSetMap<Type, YuoComponent> ChildComponents { get; private set; } = new();
        public MultiMap<Type, YuoComponent> BaseComponents { get; private set; } = new();

        public List<YuoEntity> Children { get; } = new();

        /// <summary>
        ///  获取所有子Entity,包括子Entity的子Entity
        /// </summary>
        /// <returns></returns>
        public List<YuoEntity> GetAllChildren()
        {
            var list = new List<YuoEntity>();
            foreach (var child in Children)
            {
                list.Add(child);
                list.AddRange(child.GetAllChildren());
            }

            return list;
        }

        public YuoEntity()
        {
            //EntityComponent为基础组件,无法移除,不会显示在组件列表中,但当销毁时会自动移除
            //可以通过获取EntityComponent是否为null来判断Entity是否释放
            EntityData = new EntityComponent();
            EntityData.Entity = this;

            EntityData.Id = IDGenerate.GetID(this);

            YuoWorld.Instance.AddComponent(this, EntityData);
            YuoWorld.Instance.RegisterEntity(this);
            // $"Add Entity{EntityData.Id}".Log();
        }

        public YuoEntity(long id)
        {
            //EntityComponent为基础组件,无法移除,不会显示在组件列表中,但当销毁时会自动移除
            //可以通过获取EntityComponent是否为null来判断Entity是否释放
            EntityData = new EntityComponent();
            EntityData.Entity = this;

            EntityData.Id = id;

            YuoWorld.Instance.AddComponent(this, EntityData);
            YuoWorld.Instance.RegisterEntity(this);
            // $"Add Entity{EntityData.Id}".Log();
        }

        public YuoEntity(string name) : this(name.GetHashCode())
        {
            EntityName = name;
        }

        public T GetComponent<T>() where T : YuoComponent
        {
            return GetComponent(typeof(T)) as T;
        }

        public bool TryGetComponent<T>(out T component) where T : YuoComponent
        {
            component = GetComponent<T>();
            return component != null;
        }

        public bool TryGetBaseComponent<T>(out T component) where T : YuoComponent
        {
            component = GetBaseComponent<T>();
            return component != null;
        }

        public YuoComponent GetComponent(Type type)
        {
            if (IsDisposed) return null;
            if (Components.TryGetValue(type, out var component))
                return component;
#if UNITY_EDITOR
            //为空的时候尝试获取BaseComponent,仅在编辑器下生效,防止脑子瓦特的时候用错了
            if (GetBaseComponent(type) is { } c)
            {
                "应该使用[GetBaseComponent]".LogError();
                return c;
            }
#endif
            return null;
        }

        public YuoComponent GetBaseComponent(Type type)
        {
            if (IsDisposed) return null;
            return BaseComponents.TryGetValue(type, out var component)
                ? component.Count > 0 ? component[0] : null
                : null;
        }

        public List<YuoComponent> GetBaseComponents(Type type)
        {
            if (IsDisposed) return null;
            return BaseComponents.TryGetValue(type, out var component) ? component : null;
        }

        /// <summary>
        ///  获取指定类型的子组件集合,可能会有gc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetBaseComponents<T>() where T : YuoComponent
        {
            if (IsDisposed) return null;
            return BaseComponents.TryGetValue(typeof(T), out var componentList)
                ? componentList.Cast<T>().ToList()
                : null;
        }

        public bool TryGetBaseComponents<T>(out List<T> components) where T : YuoComponent
        {
            if (IsDisposed)
            {
                components = null;
                return false;
            }

            if (BaseComponents.TryGetValue(typeof(T), out var componentList))
            {
                components = componentList.Cast<T>().ToList();
                return true;
            }

            components = null;
            return false;
        }

        /// <summary>
        /// 获取指定类型的基础组件,获取第一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBaseComponent<T>() where T : YuoComponent
        {
            return GetBaseComponent(typeof(T)) as T;
        }


        /// <summary>
        /// 如果不存在该组件,则添加该组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrAddComponent<T>() where T : YuoComponent, new()
        {
            return TryGetComponent<T>(out var component) ? component : AddComponent<T>();
        }

        /// <summary>
        ///  替换Component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="componentType"></param>
        /// <typeparam name="T"></typeparam>
        public void SetComponent<T>(T component, Type componentType = null) where T : YuoComponent
        {
            if (componentType != null) component.SetComponentType(componentType);
            var componentTemp = GetComponent(component.Type);
            if (componentTemp == null)
            {
                component.Entity = this;
                Components.Add(component.Type, component);
                Parent?.ChildComponents.AddItem(component.Type, component);
                YuoWorld.Instance.AddComponent(this, component);
            }
            else
            {
                Components[component.Type] = component;
                component.Entity = this;
                YuoWorld.Instance.SetComponent(componentTemp, component);
            }
        }

        public YuoComponent AddComponent(Type type)
        {
            if (Components.ContainsKey(type)) return GetComponent(type);
            if (Activator.CreateInstance(type) is not YuoComponent component) return null;
            AddComponent(type, component);
            return component;
        }

        public YuoComponent AddComponent(Type type, Action<YuoComponent> onAwakeBefore)
        {
            if (Components.ContainsKey(type)) return GetComponent(type);
            if (Activator.CreateInstance(type) is not YuoComponent component) return null;
            onAwakeBefore?.Invoke(component);
            AddComponent(type, component);
            return component;
        }

        public T AddComponent<T>() where T : YuoComponent, new()
        {
            if (GetComponent<T>() != null) return GetComponent<T>();
            T component = new T();
            AddComponent(typeof(T), component);
            return component;
        }

        public T AddComponent<T>(Action<T> onAwakeBefore) where T : YuoComponent, new()
        {
            if (GetComponent<T>() != null) return GetComponent<T>();
            T component = new T();
            onAwakeBefore?.Invoke(component);
            AddComponent(typeof(T), component);
            return component;
        }

        private void AddComponent(Type type, YuoComponent component)
        {
            component.Entity = this;
            Components.TryAdd(type, component);
            YuoWorld.Instance.AddComponent(this, component);
            Parent?.AddChildComponent(type, component);
        }

        void AddChildComponent(Type type, YuoComponent component)
        {
            if (component == null) return;
            ChildComponents.AddItem(type, component);
            if (!Children.Contains(component.Entity)) Children.Add(component.Entity);
        }

        public void RemoveComponent<T>() where T : YuoComponent
        {
            RemoveComponent(GetComponent<T>());
        }

        public void RemoveComponent(Type type)
        {
            RemoveComponent(GetComponent(type));
        }
        
        public void DestroyComponent<T>() where T : YuoComponent
        {
            GetComponent<T>().Destroy();
        }

        public void RemoveComponent<T>(T component) where T : YuoComponent
        {
            if (component == null || !Components.ContainsKey(component.Type)) return;
            Components.Remove(component.Type);
            YuoWorld.Instance.RemoveComponent(this, component);
            Parent?.RemoveChildComponent(component);
        }

        // public T GetChild<T>(int index) where T : YuoComponent
        // {
        //     var cs = ChildComponents[typeof(T)];
        //     return cs is { Count: > 0 } ? cs[index] as T : null;
        // }

        public List<T> GetChildren<T>() where T : YuoComponent
        {
            var children = new List<T>();
            if (ChildComponents.TryGetValue(typeof(T), out var cs))
            {
                foreach (var item in cs)
                {
                    children.Add(item as T);
                }
            }

            return children;
        }

        public T GetChild<T>() where T : YuoComponent
        {
            var cs = ChildComponents[typeof(T)];
            return cs is { Count: > 0 } ? cs.First() as T : null;
        }

        public T AddChild<T>(long entityID = long.MinValue) where T : YuoComponent, new()
        {
            return AddChild(typeof(T), entityID) as T;
        }

        public T AddChild<T>(string name) where T : YuoComponent, new()
        {
            var com = AddChild(typeof(T), name.GetHashCode()) as T;
            if (com != null) com.Entity.EntityName = name;
            return com;
        }

        public YuoComponent AddChild(Type type, long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new YuoEntity(entityID) : new YuoEntity();
            child.Parent = this;
            var component = child.AddComponent(type);
            AddChildComponent(type, component);
            return component;
        }

        public YuoEntity AddChild(long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new YuoEntity(entityID) : new YuoEntity();
            child.Parent = this;
            var type = typeof(EntityComponent);
            AddChildComponent(type, child.EntityData);
            return child;
        }

        public YuoEntity AddChild(string entityName)
        {
            var child = new YuoEntity(entityName.GetHashCode());
            child.EntityName = entityName;
            child.Parent = this;
            var type = typeof(EntityComponent);
            AddChildComponent(type, child.EntityData);
            return child;
        }

        public void RemoveChild(YuoEntity entity)
        {
            if (!IsDisposed)
            {
                if (Children.Contains(entity))
                    Children.Remove(entity);
                foreach (var item in entity.Components)
                {
                    RemoveChildComponent(item.Value);
                    ChildComponents.RemoveItem(item.Key, item.Value);
                }
            }
        }

        private void RemoveChildComponent(YuoComponent component)
        {
            if (component == null) return;
            ChildComponents.RemoveItem(component.Type, component);
        }

        /// <summary>
        ///  外部不允许调用,防止嵌套调用
        /// </summary>
        /// <param name="parent"></param>
        internal void SetParent(YuoEntity parent)
        {
            if (Parent != null)
            {
                Parent.RemoveChild(this);
            }

            Parent = parent;
        }

        public bool IsDisposed { get; private set; }

        private bool _isDestroy;
        /// <summary>
        /// 释放实体,不要在System中调用,如需释放请调用Destroy
        /// </summary>
        public void Dispose()
        {
            if (_isDestroy) return;
            IsDisposed = true;
            _isDestroy = true;
            if (Parent != null) Parent.RemoveChild(this);
            YuoWorld.Instance.UnRegisterEntity(this);
            foreach (var item in Components.Values)
            {
                YuoWorld.Instance.RemoveComponent(this, item);
                item.Dispose();
            }

            foreach (var yuoEntity in Children)
            {
                yuoEntity.Dispose();
            }

            Components.Clear();
            ChildComponents.Clear();
            ChildComponents = null;

            YuoWorld.Instance.RemoveComponent(this, EntityData);
            EntityData.Dispose();
            EntityData = null;
            IsDisposed = true;
        }

        /// <summary>
        /// 扔进回收站,这一帧结束后会被回收
        /// </summary>
        public void Destroy()
        {
            YuoWorld.DestroyEntity(this);
            IsDisposed = true;
            foreach (var component in Components.Values)
            {
                component.IsDisposed = true;
            }
        }

        #region Operator

        public static bool operator true(YuoEntity entity)
        {
            return entity is { IsDisposed: false };
        }

        public static bool operator false(YuoEntity entity)
        {
            return entity is not { IsDisposed: not true };
        }

        public static bool operator !(YuoEntity entity)
        {
            return entity is { IsDisposed: true };
        }

        // public static bool operator ==(YuoEntity left, object right)
        // {
        //     return right?.Equals(left) ?? left is null || left.IsDisposed;
        // }
        //
        // public static bool operator !=(YuoEntity left, object right)
        // {
        //     return !(left == right);
        // }

        #endregion

        /// <summary>
        ///  runSystem的简化调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RunSystem<T>() where T : ISystemTag
        {
            YuoWorld.RunSystem<T>(this);
        }
    }
}