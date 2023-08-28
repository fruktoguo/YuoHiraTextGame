using System;
using YuoTools.Main.Ecs;

namespace YuoTools.Main.Ecs
{
    public interface ISystemTag
    {
    }

    /// <summary>
    ///  Component在被创建时会立即调用,无法提前赋值
    /// </summary>
    public interface IAwake : ISystemTag
    {
    }

    /// <summary>
    ///  Component在被创建之后,在当前帧的末尾会统一调用一次,可以提前赋值
    /// </summary>
    public interface IStart : ISystemTag
    {
    }

    /// <summary>
    ///  同Unity的Update
    /// </summary>
    public interface IUpdate : ISystemTag
    {
    }

    /// <summary>
    /// 同Unity的FixedUpdate
    /// </summary>
    public interface IFixedUpdate : ISystemTag
    {
    }

    /// <summary>
    ///  同Unity的LateUpdate
    /// </summary>
    public interface ILateUpdate : ISystemTag
    {
    }

    /// <summary>
    ///  Component在被销毁时会立即调用
    /// </summary>
    public interface IDestroy : ISystemTag
    {
    }

    /// <summary>
    ///  当游戏退出时调用
    /// </summary>
    public interface IExitGame : ISystemTag
    {
    }

    /// <summary>
    ///  当组件被移动到一个新的Entity时调用
    /// </summary>
    public interface ISwitchComponent : ISystemTag
    {
    }

    internal static class SystemType
    {
        public static readonly Type Awake = typeof(IAwake);
        public static readonly Type Start = typeof(IStart);
        public static readonly Type FixedUpdate = typeof(IFixedUpdate);
        public static readonly Type Update = typeof(IUpdate);
        public static readonly Type LateUpdate = typeof(ILateUpdate);
        public static readonly Type Destroy = typeof(IDestroy);
        public static readonly Type ExitGame = typeof(IExitGame);
        public static readonly Type SwitchComponent = typeof(ISwitchComponent);
    }
}