using System;

namespace YuoTools.Main.Ecs
{
    /// <summary>
    ///  全局只有一个的组件,如果已经存在则会删除旧的
    /// </summary>
    public class SingleComponentAttribute : Attribute
    {
    }
}