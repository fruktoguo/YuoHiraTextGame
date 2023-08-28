using System;

namespace YuoTools.Main.Ecs
{
    /// <summary>
    ///  被Link的组件会在添加会将此组件自动添加
    /// </summary>
    public class LinkComponentOfAttribute : Attribute
    {
        public Type LinkType { get; }

        public LinkComponentOfAttribute(Type type)
        {
            LinkType = type;
        }
    }
}