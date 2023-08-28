using System;
using YuoTools.Main.Ecs;

public static partial class SystemTagType
{
    public static readonly Type Awake = typeof(IAwake);
    public static readonly Type Start = typeof(IStart);
    public static readonly Type Update = typeof(IUpdate);
    public static readonly Type FixedUpdate = typeof(IFixedUpdate);
    public static readonly Type Destroy = typeof(IDestroy);
    public static readonly Type ExitGame = typeof(IExitGame);
    public static readonly Type SwitchComponent = typeof(ISwitchComponent);
}