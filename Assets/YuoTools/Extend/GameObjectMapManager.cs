using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    public class GameObjectMapTagComponent : YuoComponent
    {
        public override string Name => "GameObject映射组件";
        public GameObject GameObject;
        public string Group;
    }

    [AutoAddToMain()]
    public class GameObjectMapManager : YuoComponentInstance<GameObjectMapManager>
    {
        [ShowInInspector] Dictionary<GameObject, YuoEntity> gameObjectMap = new();

        public void AddMap(GameObject gameObject, YuoEntity entity)
        {
            gameObjectMap[gameObject] = entity;
        }

        public YuoEntity GetMap(GameObject gameObject)
        {
            return gameObjectMap.TryGetValue(gameObject, out var map) ? map : null;
        }

        MultiHashSetMap<string, GameObject> goPool = new();

        private bool TryGetGameObject(string group, YuoEntity entity, out GameObject go)
        {
            if (goPool.TryGetFirst(group, out go))
            {
                goPool.RemoveItem(group, go);
                var tag = entity.AddComponent<GameObjectMapTagComponent>();
                tag.GameObject = go;
                tag.Group = group;
                return true;
            }

            return false;
        }
        
        public static void TryGet(string group, YuoEntity entity, out GameObject go) => Get.TryGetGameObject(group, entity, out go);

        private void Instantiate(string group, YuoEntity entity, GameObject go)
        {
            var tag = entity.AddComponent<GameObjectMapTagComponent>();
            tag.GameObject = go;
            tag.Group = group;
        }
        
        public static void AddTag(string group, YuoEntity entity, GameObject go) => Get.Instantiate(group, entity, go);

        private void DestroyGameObject(GameObjectMapTagComponent go)
        {
            if (go.IsNull()) return;
            gameObjectMap.Remove(go.GameObject);
            goPool.AddItem(go.Group, go.GameObject);
        }
        
        public static void Destroy(GameObjectMapTagComponent go) => Get.DestroyGameObject(go);
    }


    public class GameObjectMapTagComponentDestroySystem : YuoSystem<GameObjectMapTagComponent>, IAwake, IDestroy
    {
        protected override void Run(GameObjectMapTagComponent go)
        {
            if (RunType == SystemTagType.Awake)
            {
                
            }

            if (RunType == SystemTagType.Destroy)
            {
                go.GameObject.Hide();
                GameObjectMapManager.Destroy(go);
            }
        }
    }
}