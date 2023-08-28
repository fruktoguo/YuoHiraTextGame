using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace YuoTools.Extend
{
    public class ComponentManagerAsset : SerializedScriptableObject
    {
        public Dictionary<string, bool> AutoAdd = new Dictionary<string, bool>();
    }
}