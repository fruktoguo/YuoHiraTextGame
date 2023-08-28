using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Extend.Helper;
using Object = UnityEngine.Object;

namespace YuoTools.Main.Ecs
{
    public abstract class BuffComponent : YuoComponent
    {
        /// <summary>
        ///  buff的层数
        /// </summary>
        public int BuffCount;

        /// <summary>
        /// buff的最大层数,默认为1
        /// </summary>
        public virtual int BuffMaxCount { get; set; } = 1;

        /// <summary>
        ///  buff的名字
        /// </summary>
        [ShowInInspector]
        public abstract string BuffName { get; }

        /// <summary>
        ///  buff的描述
        /// </summary>
        [ShowInInspector]
        public virtual string BuffDescription { get; } = "";

        /// <summary>
        ///  buff添加时的时间
        /// </summary>
        public float StartTime;

        /// <summary>
        ///  上次buff改变的时间
        /// </summary>
        public float RefreshTime;

        /// <summary>
        ///  预计buff结束的时间
        /// </summary>
        public float EndTime;

        /// <summary>
        ///  移除时是否移除所有层
        /// </summary>
        public virtual bool RemoveAllCountOnRemove { get; set; } = false;

        /// <summary>
        /// buff的持续时间,默认为1秒
        /// </summary>
        public virtual float Duration { get; set; } = 1;

        /// <summary>
        ///  buff的剩余时间比例
        /// </summary>
        public float ResidualRatio
        {
            get
            {
                var durationLenght = EndTime - RefreshTime;
                if (durationLenght > Duration + 0.1f)
                {
                    return (EndTime - Time.time) / durationLenght;
                }

                return (EndTime - Time.time) / Duration;
            }
        }

        /// <summary>
        ///  如果Buff有绑定特效,则在这里添加,在Buff移除时会自动销毁
        /// </summary>
        public Object Effect;

        public void Remove(int count = 1)
        {
            BuffManagerComponent.Get.RemoveBuff(this, count);
        }

        public virtual bool DontShow => false;
    }

    public class BuffManagerComponent : YuoComponentGet<BuffManagerComponent>
    {
        /// <summary>
        /// 所有buff的索引,用于统一处理
        /// </summary>
        public DicList<YuoEntity, BuffComponent> buffComponents = new();

        public DicList<YuoEntity, BuffComponent> enableBuffComponents = new();

        public T AddBuff<T>(YuoEntity entity, float duration = 0, int count = 0)
            where T : BuffComponent, new()
        {
            var buff = entity.GetComponent<T>();
            if (buff == null)
            {
                buff = entity.AddComponent<T>();
                buff.StartTime = Time.time;
                YuoWorld.RunSystem<IBuffCreateBefore>(buff);
                YuoWorld.RunSystem<IBuffCreate>(buff);
                buffComponents.AddItem(entity, buff);
                if (!buff.DontShow) enableBuffComponents.AddItem(entity, buff);
            }

            // Debug.Log($"AddBuff{buff.BuffName}");
            buff.RefreshTime = Time.time;

            if (duration <= 0.0001f)
            {
                buff.EndTime = buff.RefreshTime + buff.Duration;
            }
            else
            {
                var newTime = buff.RefreshTime + duration;
                if (buff.EndTime < newTime)
                {
                    buff.EndTime = newTime;
                }
            }


            if (count == 0)
            {
                if (buff.BuffCount < buff.BuffMaxCount)
                {
                    buff.BuffCount++;
                    YuoWorld.RunSystem<IBuffAdd>(buff);
                    YuoWorld.RunSystem<IBuffChange>(buff);
                }
            }
            else if (count > 0)
            {
                buff.BuffCount += count;
                buff.BuffCount.Clamp(buff.BuffMaxCount);
                YuoWorld.RunSystem<IBuffAdd>(buff);
                YuoWorld.RunSystem<IBuffChange>(buff);
            }

            return buff;
        }

        public void RemoveBuff<T>(YuoEntity entity) where T : BuffComponent, new()
        {
            RemoveBuff(entity.GetComponent<T>());
        }

        public void RemoveBuff<T>(YuoEntity entity, int count) where T : BuffComponent, new()
        {
            RemoveBuff(entity.GetComponent<T>(), count);
        }

        public void RemoveBuff(BuffComponent buff, int count = 1)
        {
            if (buff != null)
            {
                // Debug.Log($"MoveBuff{buff.BuffName}");
                if (!buff.RemoveAllCountOnRemove && count >= 1)
                {
                    buff.BuffCount -= count;
                    buff.BuffCount.Clamp(buff.BuffMaxCount);

                    buff.RefreshTime = Time.time;
                    buff.EndTime = buff.RefreshTime + buff.Duration;

                    YuoWorld.RunSystem<IBuffRemove>(buff);
                    YuoWorld.RunSystem<IBuffChange>(buff);
                    if (buff.BuffCount <= 0)
                    {
                        DeleteBuff(buff);
                    }
                }
                else
                {
                    YuoWorld.RunSystem<IBuffRemove>(buff);
                    YuoWorld.RunSystem<IBuffChange>(buff);
                    DeleteBuff(buff);
                }
            }
        }

        /// <summary>
        /// 删除buff
        /// </summary>
        public void DeleteBuff(BuffComponent buff)
        {
            if (!tempDelete.Contains(buff))
            {
                tempDelete.Add(buff);
            }
        }

        private List<BuffComponent> tempDelete = new();

        public void Update()
        {
            foreach (var entity in buffComponents)
            {
                foreach (var buff in entity.Value)
                {
                    //检查是否过期
                    if (buff.EndTime < Time.time)
                    {
                        RemoveBuff(buff);
                        YuoWorld.RunSystem<IBuffRemove>(buff);
                    }
                }
            }

            foreach (var buff in tempDelete)
            {
                buffComponents.RemoveItem(buff.Entity, buff);
                if (!buff.DontShow) enableBuffComponents.RemoveItem(buff.Entity, buff);
                if (buff.Effect != null)
                {
                    Object.Destroy(buff.Effect);
                }

                YuoWorld.RunSystem<IBuffDelete>(buff);
                buff.Destroy();

                buff.Entity.RemoveComponent(buff);
            }

            tempDelete.Clear();
        }

        public Dictionary<string, Type> buffTypes = new();

        public int GetBuffCount(YuoEntity entity, string buffName)
        {
            // Debug.Log(buffName);
            if (buffTypes.TryGetValue(buffName, out var type))
            {
                var buff = entity.GetComponent(type);
                if (buff != null)
                {
                    return ((BuffComponent)buff).BuffCount;
                }
            }

            return 0;
        }
    }

    public class BuffManagerComponentAwakeSystem : YuoSystem<BuffManagerComponent>, IAwake
    {
        public override string Group => "BuffManager";

        protected override void Run(BuffManagerComponent component)
        {
            var types = ReflexHelper.GetTypesOfBase<BuffComponent>();
            component.buffTypes.Clear();
            foreach (var type in types)
            {
                component.buffTypes.Add(type.Name, type);
            }
        }
    }

    public class BuffManagerSystem : YuoSystem<BuffManagerComponent>, IUpdate
    {
        public override string Group => "BuffManager";

        protected override void Run(BuffManagerComponent component)
        {
            component.Update();
        }
    }

    /// <summary>
    /// buff增加时
    /// </summary>
    public interface IBuffAdd : ISystemTag
    {
    }

    /// <summary>
    /// buff减少时
    /// </summary>
    public interface IBuffRemove : ISystemTag
    {
    }

    /// <summary>
    ///  当Buff层数产生任意改变时
    /// </summary>
    public interface IBuffChange : ISystemTag
    {
    }

    /// <summary>
    /// buff添加时
    /// </summary>
    public interface IBuffCreate : ISystemTag
    {
    }

    /// <summary>
    /// buff添加前
    /// </summary>
    public interface IBuffCreateBefore : ISystemTag
    {
    }

    /// <summary>
    /// buff添加失败
    /// </summary>
    public interface IBuffCreateError : ISystemTag
    {
    }

    /// <summary>
    /// buff删除时
    /// </summary>
    public interface IBuffDelete : ISystemTag
    {
    }
}