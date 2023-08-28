using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YuoTools.Extend.Helper;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.Extend
{
    [SystemOrder(short.MinValue)]
    public class SceneInitSystem : YuoSystem<SceneComponent>, IAwake
    {
        public override string Group => SystemGroupConst.Main;
        public class UnityEngineLog : Main.Ecs.YuoLog.LogComponent
        {
            public override T Log<T>(T msg)
            {
                Debug.Log(msg);
                return msg;
            }

            public override T Error<T>(T msg)
            {
                Debug.LogError(msg);
                return msg;
            }
        }

        protected override void Run(SceneComponent component)
        {
            Main.Ecs.YuoLog.Open(new UnityEngineLog());

            $"Init In {Time.frameCount} Frame".Log();

            component.Entity.EntityName = $"Scene {YuoWorld.Instance.AllScenes.Count - 1}";
            var componentManager = YuoWorld.Main.GetOrAddComponent<ComponentManager>();
            var autoAddTo = YuoWorld.Instance.GetAllComponentOfType();

            foreach (var item in autoAddTo)
            {
                //auto to main entity
                var autoAddToMain = item.Value.GetCustomAttribute<AutoAddToMainAttribute>();
                var autoAddToScene = item.Value.GetCustomAttribute<AutoAddToSceneAttribute>();

                if (autoAddToMain != null || autoAddToScene != null)
                {
                    $"自动挂载组件 [ {item.Key} ] \n".MergeLog();

                    if (componentManager.Asset.AutoAdd.TryGetValue(item.Key, out var value))
                    {
                        if (!value)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        componentManager.Asset.AutoAdd.Add(item.Key,
                            autoAddToMain?.AutoAdd ?? autoAddToScene?.AutoAdd ?? false);
                    }
                }

                if (autoAddToMain is { AutoAdd: true })
                {
                    YuoWorld.Main.AddComponent(item.Value);
                }

                //auto to scene entity
                if (autoAddToScene is { AutoAdd: true })
                {
                    YuoWorld.Scene.AddComponent(item.Value);
                }
            }

            YuoLog.MergeLogOutput();
        }
    }

    public class SceneDestroySystem : YuoSystem<SceneComponent>, IDestroy
    {
        public override string Group => SystemGroupConst.Main;
        protected override void Run(SceneComponent component)
        {
            YuoWorld.Instance.AllScenes.Remove(component.Entity);
        }
    }

    public class SceneComponentHelper
    {
        public static void LoadScene(string sceneName)
        {
            DestroyAndCreateScene();
            SceneManager.LoadScene(sceneName);
        }

        public static void DestroyAndCreateScene()
        {
            YuoWorld.Scene.Destroy();
            var scene = new YuoEntity();
            YuoWorld.Instance.AllScenes[0] = scene;
            scene.AddComponent<SceneComponent>();
        }

        public static void ReloadScene()
        {
            DestroyAndCreateScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 场景退出时调用
    /// </summary>
    public interface ISceneExit : ISystemTag
    {
    }
}