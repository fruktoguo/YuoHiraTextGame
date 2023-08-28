using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    [DefaultExecutionOrder(int.MinValue)]
    public class WorldMono : SerializedMonoBehaviour
    {
        public YuoWorld yuoWorld;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void WorldInitBeforeSceneLoad()
        {
            if(YuoWorld.Instance != null)
                return;
            var worldMono = new GameObject("World").AddComponent<WorldMono>();
            worldMono.yuoWorld = new YuoWorld();
            worldMono.yuoWorld.OnInit();
            DontDestroyOnLoad(worldMono);
        }

        private void OnDestroy()
        {
            if (YuoWorld.Instance == yuoWorld)
                yuoWorld.OnDestroy();
        }

        private void Update()
        {
            yuoWorld.Update();
        }

        private void FixedUpdate()
        {
            yuoWorld.FixedUpdate();
        }
        
        private void LateUpdate()
        {
            yuoWorld.LateUpdate();
        }
    }
}