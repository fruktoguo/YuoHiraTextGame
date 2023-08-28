using System;
using System.Collections.Generic;

namespace YuoTools.Main.Ecs
{
    public class EntitySystem : YuoSystem<EntityComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Main;

        protected override void Run(EntityComponent component)
        {
        }
    }

    [Serializable]
    public abstract class YuoSystem<T1> : SystemBase where T1 : YuoComponent
    {
        private List<T1> _components1 = new();

        private Type _type1;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(T1);

            _influenceTypes.Add(_type1);

            yuoWorld.RegisterSystem(this, typeof(T1));
        }

        List<Type> _influenceTypes = new();

        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            Entities.Add(entity);
            _components1.Add(t1 as T1);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            _components1[index] = component2 as T1;
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            Run(_components1[entityIndex]);
        }

        internal override void Clear()
        {
            _components1.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component);
    }

    public abstract class YuoSystem<T1, T2> : SystemBase where T1 : YuoComponent where T2 : YuoComponent
    {
        private Dictionary<YuoEntity,int> _entityIndex = new();
        private List<T1> _components1 = new();
        private List<T2> _components2 = new();

        private Type _type1;
        private Type _type2;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);

            _influenceTypes.Add(_type1);
            _influenceTypes.Add(_type2);

            yuoWorld.RegisterSystem(this, _type1);
            yuoWorld.RegisterSystem(this, _type2);
        }

        List<Type> _influenceTypes = new();
        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            Entities.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            return true;
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex], _components2[entityIndex]);
            }
            catch (Exception e)
            {
                // $"{entityIndex}-- {_type1} count {_components1.Count} {_type2} count {_components2.Count}".LogError();
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2);
    }

    public abstract class YuoSystem<T1, T2, T3> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
    {
        private readonly List<T1> _components1 = new();
        private readonly List<T2> _components2 = new();
        private readonly List<T3> _components3 = new();

        private Type _type1;
        private Type _type2;
        private Type _type3;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);

            _influenceTypes.Add(_type1);
            _influenceTypes.Add(_type2);
            _influenceTypes.Add(_type3);

            yuoWorld.RegisterSystem(this, _type1);
            yuoWorld.RegisterSystem(this, _type2);
            yuoWorld.RegisterSystem(this, _type3);
        }

        List<Type> _influenceTypes = new();
        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            Entities.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex]);
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3);
    }

    public abstract class YuoSystem<T1, T2, T3, T4> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
        where T4 : YuoComponent
    {
        private readonly List<T1> _components1 = new();
        private readonly List<T2> _components2 = new();
        private readonly List<T3> _components3 = new();
        private readonly List<T4> _components4 = new();
        private Type _type1;
        private Type _type2;
        private Type _type3;
        private Type _type4;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);
            _type4 = typeof(T4);

            _influenceTypes.Add(_type1);
            _influenceTypes.Add(_type2);
            _influenceTypes.Add(_type3);
            _influenceTypes.Add(_type4);

            yuoWorld.RegisterSystem(this, _type1);
            yuoWorld.RegisterSystem(this, _type2);
            yuoWorld.RegisterSystem(this, _type3);
            yuoWorld.RegisterSystem(this, _type4);
        }

        List<Type> _influenceTypes = new();
        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            var t4 = entity.GetComponent(_type4);
            if (t4 == null) return false;
            Entities.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            _components4.Add(t4 as T4);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
            else if (_type4 == type)
            {
                _components4[index] = (T4)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
            _components4.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex],
                _components4[entityIndex]);
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            _components4.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3, T4 component4);
    }

    public abstract class YuoSystem<T1, T2, T3, T4, T5> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
        where T4 : YuoComponent
        where T5 : YuoComponent
    {
        private readonly List<T1> _components1 = new();

        private readonly List<T2> _components2 = new();

        private readonly List<T3> _components3 = new();

        private readonly List<T4> _components4 = new();

        private readonly List<T5> _components5 = new();

        private Type _type1;
        private Type _type2;
        private Type _type3;
        private Type _type4;
        private Type _type5;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);
            _type4 = typeof(T4);
            _type5 = typeof(T5);

            _influenceTypes.Add(_type1);
            _influenceTypes.Add(_type2);
            _influenceTypes.Add(_type3);
            _influenceTypes.Add(_type4);
            _influenceTypes.Add(_type5);

            yuoWorld.RegisterSystem(this, _type1);
            yuoWorld.RegisterSystem(this, _type2);
            yuoWorld.RegisterSystem(this, _type3);
            yuoWorld.RegisterSystem(this, _type4);
            yuoWorld.RegisterSystem(this, _type5);
        }

        List<Type> _influenceTypes = new();
        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            var t4 = entity.GetComponent(_type4);
            if (t4 == null) return false;
            var t5 = entity.GetComponent(_type5);
            if (t5 == null) return false;
            Entities.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            _components4.Add(t4 as T4);
            _components5.Add(t5 as T5);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
            else if (_type4 == type)
            {
                _components4[index] = (T4)component2;
            }
            else if (_type5 == type)
            {
                _components5[index] = (T5)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
            _components4.RemoveAt(index);
            _components5.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex],
                _components4[entityIndex], _components5[entityIndex]);
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            _components4.Clear();
            _components5.Clear();
            Entities.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);
    }

    /// <summary>
    ///  用于执行BaseComponent的系统
    /// </summary>
    /// <typeparam name="TB">这个类型必须是BaseComponent</typeparam>
    public abstract class YuoSystemOfBase<TB> : SystemBaseBase where TB : YuoComponent
    {
        public List<TB> baseComponents = new();

        private Type _type1;
        private List<Type> _influenceTypes = new();

        internal override bool CheckIsBaseComponent()
        {
            if (_type1.BaseType == null || _type1.BaseType == typeof(YuoComponent) ||
                _type1.BaseType.BaseType == typeof(YuoComponent))
            {
                $"类型错误-[{_type1}]-必须是YuoComponent的第二重子类".LogError();
                return false;
            }

            return true;
        }

        public override List<Type> InfluenceTypes() => _influenceTypes;

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(TB);

            _influenceTypes.Add(_type1);

            yuoWorld.RegisterSystem(this, _type1);

            yuoWorld.RegisterBaseSystem(this, _type1);
        }

        internal override bool AddComponent(YuoEntity entity)
        {
            if (entity.BaseComponents.TryGetValue(_type1, out var list) && list.Count > 0)
            {
                baseComponents.Add(list[0] as TB);
                Entities.Add(entity);
            }
            else
            {
                return false;
            }

            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                baseComponents[index] = (TB)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            baseComponents.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            var component1 = baseComponents[entityIndex];
            var entity = Entities[entityIndex];
            var list = entity.BaseComponents[component1.BaseComponentType];
            foreach (var component in list)
            {
                Run(component as TB);
            }
        }

        internal override void m_Run(YuoComponent baseComponent)
        {
            Run(baseComponent as TB);
        }

        internal override void Clear()
        {
            baseComponents.Clear();
            Entities.Clear();
        }

        protected abstract void Run(TB baseComponent);
    }

    public abstract class YuoSystemOfBase<TB, T2> : SystemBaseBase where TB : YuoComponent where T2 : YuoComponent
    {
        private readonly List<TB> _components1 = new();
        private readonly List<T2> _components2 = new();

        private Type _type1;
        private Type _type2;
        List<Type> _influenceTypes = new();
        public override List<Type> InfluenceTypes() => _influenceTypes;

        internal override bool CheckIsBaseComponent()
        {
            if (_type1.BaseType == null || _type1.BaseType == typeof(YuoComponent) ||
                _type1.BaseType.BaseType == typeof(YuoComponent))
            {
                $"类型错误-[{_type1}]-必须是YuoComponent的第二重子类".LogError();
                return false;
            }

            return true;
        }

        public override void Init(YuoWorld yuoWorld)
        {
            _type1 = typeof(TB);
            _type2 = typeof(T2);

            _influenceTypes.Add(_type1);

            _influenceTypes.Add(_type2);

            yuoWorld.RegisterBaseSystem(this, _type1);

            yuoWorld.RegisterSystem(this, _type1);

            yuoWorld.RegisterSystem(this, _type2);
        }

        internal override bool AddComponent(YuoEntity entity)
        {
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;

            if (entity.BaseComponents.TryGetValue(_type1, out var list) && list.Count > 0)
            {
                _components1.Add(list[0] as TB);
            }
            else
            {
                return false;
            }

            _components2.Add(t2 as T2);

            Entities.Add(entity);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entities.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (TB)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entities.IndexOf(entity);
            if (index == -1) return;
            Entities.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            var component1 = _components1[entityIndex];
            var component2 = _components2[entityIndex];
            if (component1.BaseComponentType != null)
            {
                var entity = Entities[entityIndex];
                var list = entity.BaseComponents[component1.BaseComponentType];
                foreach (var component in list)
                {
                    Run(component as TB, component2);
                }
            }
        }

        internal override void m_Run(YuoComponent baseComponent)
        {
            var index = Entities.IndexOf(baseComponent.Entity);
            if (index == -1) return;
            Run(baseComponent as TB, _components2[index]);
        }

        protected abstract void Run(TB baseComponent, T2 component2);
    }
}