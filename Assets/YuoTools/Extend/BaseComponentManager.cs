using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    [AutoAddToMain]
    public class BaseComponentManager : YuoComponentInstance<BaseComponentManager>
    {
        [ShowInInspector]
        private Dictionary<string,Dictionary<string,Type>> data = new();
        
        public void AddType(string key, string name, Type type)
        {
            if (!data.TryGetValue(key, out var value))
            {
                value = new Dictionary<string, Type>();
                data[key] = value;
            }

            value.TryAdd(name, type);
        }
        
        public Type GetType(string key, string name)
        {
            if (data.TryGetValue(key, out var value))
            {
                if (value.TryGetValue(name, out var type))
                {
                    return type;
                }
            }

            return null;
        }
        
        public bool TryGetType(string key, string name, out Type type)
        {
            if (data.TryGetValue(key, out var value))
            {
                if (value.TryGetValue(name, out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }
    }
}