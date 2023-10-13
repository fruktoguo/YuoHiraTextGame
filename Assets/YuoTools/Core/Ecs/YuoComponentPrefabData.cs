using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoTools.Main.Ecs
{
    [CreateAssetMenu(fileName = "YuoComponentPrefabData", menuName = "YuoComponentPrefabData")]
    [HideMonoScript]
    public class YuoComponentPrefabData : SerializedScriptableObject
    {
        public Type ComponentType;
        [HideLabel] [SerializeField] private YuoComponent component;

        public static YuoComponentPrefabData Create(YuoComponent component)
        {
            var data = CreateInstance<YuoComponentPrefabData>();
            data.ComponentType = component.Type;
            data.component = component;
            return data;
        }

        public void Load(YuoEntity entity)
        {
            var newSo = Instantiate(this);
            entity.SetComponent(newSo.component, newSo.ComponentType);
        }
    }
}