using System;

namespace YuoTools.Main.Ecs
{
    public class CoverSystemAttribute : Attribute
    {
        public Type CoverType { get; }
        public CoverSystemAttribute(Type coverType)
        {
            CoverType = coverType;
        }
    }
}