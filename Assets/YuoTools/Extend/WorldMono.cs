using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    [DefaultExecutionOrder(int.MinValue)]
    public sealed class WorldMono : SerializedMonoBehaviour
    {
        public YuoWorld World;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void WorldInitBeforeSceneLoad()
        {
            if(YuoWorld.Instance != null)
                return;
            var worldMono = new GameObject("World").AddComponent<WorldMono>();
            worldMono.World = new YuoWorld();
            worldMono.World.OnInit();
            DontDestroyOnLoad(worldMono);
        }

        private void OnDestroy()
        {
            if (YuoWorld.Instance == World)
                World.OnDestroy();
        }

        private void Update()
        {
            World.Update();
        }

        private void FixedUpdate()
        {
            World.FixedUpdate();
        }
        
        private void LateUpdate()
        {
            World.LateUpdate();
        }
    }
}