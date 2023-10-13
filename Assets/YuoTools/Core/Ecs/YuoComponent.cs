using System;

namespace YuoTools.Main.Ecs
{
    public partial class YuoComponent : IDisposable
    {
        [Newtonsoft.Json.JsonIgnore] 
        public YuoEntity Entity { get; set; }
        
        [Newtonsoft.Json.JsonIgnore] 
        public virtual string Name => Type.Name;

        public Type BaseComponentType { get; internal set; } = null;

        public bool IsDisposed { get; internal set; }

        public YuoComponent()
        {
            Type = GetType();
        }

        internal void SetComponentType(Type type)
        {
            Type = type;
        }

        [Newtonsoft.Json.JsonIgnore] public Type Type { get; private set; }

        public T GetComponent<T>() where T : YuoComponent
        {
            return Entity.GetComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : YuoComponent
        {
            return Entity.TryGetComponent(out component);
        }

        /// <summary>
        /// 添加组件到实体
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : YuoComponent, new()
        {
            return Entity.AddComponent<T>();
        }

        /// <summary>
        /// 释放组件,不要在System中调用,如需释放请调用Destroy
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        ~YuoComponent()
        {
            Dispose(false);
        }

        private bool _isDestroy;
        private void Dispose(bool disposing)
        {
            if (_isDestroy) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放本对象中管理的托管资源
            }

            //TODO:释放非托管资源
            Entity = null;
            IsDisposed = true;
            _isDestroy = true;
        }

        /// <summary>
        /// 扔进回收站,这一帧结束后会被回收
        /// </summary>
        public void Destroy()
        {
            YuoWorld.DestroyComponent(this);
            IsDisposed = true;
        }

        public static bool operator true(YuoComponent component)
        {
            return component is { IsDisposed: false };
        }

        public static bool operator false(YuoComponent component)
        {
            return component is not { IsDisposed: not true };
        }

        public static bool operator !(YuoComponent component)
        {
            return !(component is { IsDisposed: false });
        }

        /// <summary>
        ///  runSystem的简化调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RunSystem<T>() where T : ISystemTag
        {
            YuoWorld.RunSystem<T>(this);
        }

        public void RunSystemOfBaseComponent<T>() where T : ISystemTag
        {
            YuoWorld.RunSystemOfBase<T>(this);
        }

        //
        // public static bool operator ==(YuoComponent left, object right)
        // {
        //     return right?.Equals(left) ?? left is null || left.IsDisposed;
        // }
        //
        // public static bool operator !=(YuoComponent left, object right)
        // {
        //     return !(left == right);
        // }
    }

    public static class EntityEx
    {
        public static bool IsNull(this YuoComponent component)
        {
            return component is null || component.IsDisposed;
        }

        public static bool IsNull(this YuoEntity entity)
        {
            return entity is null || entity.IsDisposed;
        }
    }

    public class EntityComponent : YuoComponent
    {
        public override string Name => "核心组件";
        public long Id { get; set; }
    }

    public partial class YuoEntity
    {
        public long ID => EntityData.Id;

        string _entityName = null;

        public string EntityName
        {
            get
            {
                if (_entityName == null)
                {
                    return ID.ToString();
                }

                return _entityName;
            }
            set => _entityName = value;
        }

        public override string ToString()
        {
            return EntityName + (IsDisposed ? "(已释放)" : "");
        }
    }
}