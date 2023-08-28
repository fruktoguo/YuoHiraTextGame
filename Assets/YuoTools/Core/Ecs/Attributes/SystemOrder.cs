using System;

namespace YuoTools.Main.Ecs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemOrderAttribute : Attribute
    {
        public short Order { get; } = 0;

        public SystemOrderAttribute(short order = 0)
        {
            Order = order;
        }
    }
}
