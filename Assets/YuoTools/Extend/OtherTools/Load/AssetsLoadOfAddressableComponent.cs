using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
//如果不用Addressable,就直接删除这个文件
namespace YuoTools
{
    public class AssetsLoadOfAddressableComponent : AssetsLoadComponent
    {
        private Dictionary<string, Sprite> spritePool = new();
        private Dictionary<string, SpriteAtlas> spriteAtlasPool = new();
        private Dictionary<string, AudioClip> audioClipPool = new();
        private Dictionary<string, GameObject> prefabPool = new();

        public override GameObject LoadPrefab(string name, bool isPool = true)
        {
            if (isPool && prefabPool.TryGetValue(name, out var go)) return go;
            prefabPool.Add(name, Addressables.LoadAssetAsync<GameObject>(name).Result);
            return prefabPool[name];
        }

        public override async ETTask<GameObject> LoadPrefabAsync(string name, bool isPool = true)
        {
            if (isPool && prefabPool.TryGetValue(name, out var go)) return go;
            var tcs = ETTask<GameObject>.Create();
            var request = Addressables.LoadAssetAsync<GameObject>(name);
            request.Completed += operation => { tcs.SetResult(request.Result); };
            return await tcs;
        }

        public override Sprite LoadSprite(string name, bool isPool = true)
        {
            if (isPool && spritePool.TryGetValue(name, out var sprite)) return sprite;
            spritePool.Add(name, Addressables.LoadAssetAsync<Sprite>(name).Result);
            return spritePool[name];
        }

        public override async ETTask<Sprite> LoadSpriteAsync(string name, bool isPool = true)
        {
            if (isPool && spritePool.TryGetValue(name, out var sprite)) return sprite;
            var tcs = ETTask<Sprite>.Create();
            var request = Addressables.LoadAssetAsync<Sprite>(name);
            request.Completed += operation => { tcs.SetResult(request.Result); };
            return await tcs;
        }

        public override SpriteAtlas LoadSpriteAtlas(string name, bool isPool = true)
        {
            if (isPool && spriteAtlasPool.TryGetValue(name, out var sprite)) return sprite;
            spriteAtlasPool.Add(name, Addressables.LoadAssetAsync<SpriteAtlas>(name).Result);
            return spriteAtlasPool[name];
        }

        public override async ETTask<SpriteAtlas> LoadSpriteAtlasAsync(string name, bool isPool = true)
        {
            if (isPool && spriteAtlasPool.TryGetValue(name, out var sprite)) return sprite;
            var tcs = ETTask<SpriteAtlas>.Create();
            var request = Addressables.LoadAssetAsync<SpriteAtlas>(name);
            request.Completed += operation => { tcs.SetResult(request.Result); };
            return await tcs;
        }

        public override AudioClip LoadAudioClip(string name, bool isPool = true)
        {
            if (isPool && audioClipPool.TryGetValue(name, out var audioClip)) return audioClip;
            audioClipPool.Add(name, Addressables.LoadAssetAsync<AudioClip>(name).Result);
            return audioClipPool[name];
        }

        public override async ETTask<AudioClip> LoadAudioClipAsync(string name, bool isPool = true)
        {
            if (isPool && audioClipPool.TryGetValue(name, out var audioClip)) return audioClip;
            var tcs = ETTask<AudioClip>.Create();
            var request = Addressables.LoadAssetAsync<AudioClip>(name);
            request.Completed += operation => { tcs.SetResult(request.Result); };
            return await tcs;
        }

        public override T LoadAsset<T>(string name)
        {
            return Addressables.LoadAssetAsync<T>(name).Result;
        }

        public override async ETTask<T> LoadAssetAsync<T>(string name)
        {
            var tcs = ETTask<T>.Create();
            var request = Addressables.LoadAssetAsync<T>(name);
            request.Completed += operation => { tcs.SetResult(request.Result); };
            return await tcs;
        }
    }
}