using System;
using System.Collections.Generic;

namespace YuoTools.Main.Ecs
{
    public abstract class SystemBase
    {
        public bool Enabled { get; set; } = true;

        public readonly List<YuoEntity> Entities = new();

        public int EntityCount => Entities.Count;

        public virtual string Name => Type.Name;

        public virtual string Group => "";

#if UNITY_EDITOR

        public double TimeConsuming { get; private set; }

        public double TotalTimeConsuming { get; private set; }

        public long TotalRunCount { get; private set; }

#endif

        protected SystemBase()
        {
#if UNITY_EDITOR
            _sw = new System.Diagnostics.Stopwatch();
            TimeConsuming = 0;
            TotalTimeConsuming = 0;
            TotalRunCount = 0;
#endif
        }

#if UNITY_EDITOR

        internal void StartClock()
        {
            _sw.Restart();
        }

        internal void StopClock()
        {
            _sw.Stop();
            SetTimeConsuming(_sw.Elapsed.TotalMilliseconds);
        }


        private void SetTimeConsuming(double time)
        {
            TimeConsuming = time;
            TotalTimeConsuming += time;
            TotalRunCount++;
        }

        private readonly System.Diagnostics.Stopwatch _sw;

#endif

        public abstract void Init(YuoWorld yuoWorld);

        public abstract List<Type> InfluenceTypes();

        public Type Type { get; internal set; }

        public Type RunType;

        internal void m_Run()
        {
            if (!Enabled) return;
            if (Entities.Count == 0)
            {
                return;
            }
#if UNITY_EDITOR
            StartClock();
#endif
            for (int i = 0; i < Entities.Count; i++)
            {
                // try
                // {
                    m_Run(i);
                // }
                // catch (Exception e)
                // {
                //     e.LogError();
                // }
            }

#if UNITY_EDITOR
            StopClock();
#endif
        }

        internal virtual void Clear()
        {
            Entities.Clear();
            systemTags.Clear();
        }

        internal abstract void m_Run(int entityIndex);

        internal bool m_Run(YuoEntity entity)
        {
            if (!Enabled || !Entities.Contains(entity)) return false;
#if UNITY_EDITOR
            StartClock();
#endif
            try
            {
                m_Run(Entities.IndexOf(entity));
            }
            catch (Exception e)
            {
                e.LogError();
            }
#if UNITY_EDITOR
            StopClock();
#endif
            return true;
        }

        internal abstract void SetComponent(YuoEntity entity, Type type, YuoComponent component2);

        internal abstract bool AddComponent(YuoEntity entity);

        internal abstract void RemoveComponent(YuoEntity entity);

        internal List<Type> systemTags = new List<Type>();

        public bool HasTag<T>() where T : ISystemTag
        {
            return systemTags.Contains(typeof(T));
        }

        public bool HasTag(Type type)
        {
            return systemTags.Contains(type);
        }
    }

    public abstract class SystemBaseBase : SystemBase
    {
        internal abstract bool CheckIsBaseComponent();
        internal abstract void m_Run(YuoComponent baseComponent);
    }
}