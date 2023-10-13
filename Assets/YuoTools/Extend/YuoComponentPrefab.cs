using ET;

namespace YuoTools.Main.Ecs
{
    public static class YuoComponentPrefab
    {
        public static bool AddComponentOfPrefab(this YuoEntity entity, string path)
        {
            var prefab = YuoWorld.Main.GetBaseComponent<AssetsLoadComponent>().LoadAsset<YuoComponentPrefabData>(path);
            if (prefab == null) return false;
            prefab.Load(entity);
            return true;
        }

        public static async ETTask<bool> AddComponentOfPrefabAsync(this YuoEntity entity, string path)
        {
            var prefab = await YuoWorld.Main.GetBaseComponent<AssetsLoadComponent>()
                .LoadAssetAsync<YuoComponentPrefabData>(path);
            if (prefab == null) return false;
            prefab.Load(entity);
            return true;
        }
        

    }
}