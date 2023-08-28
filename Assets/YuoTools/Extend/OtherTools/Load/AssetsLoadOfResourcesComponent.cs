using System.Collections.Generic;
using ET;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D;

namespace YuoTools
{
    public class AssetsLoadOfResourcesComponent : AssetsLoadComponent
    {
        [ShowInInspector]
        private Dictionary<string, Sprite> spritePool = new();
        [ShowInInspector]
        private Dictionary<string, SpriteAtlas> spriteAtlasPool = new();
        [ShowInInspector]
        private Dictionary<string, AudioClip> audioClipPool = new();
        [ShowInInspector]
        private Dictionary<string, GameObject> prefabPool = new();

        public override GameObject LoadPrefab(string name, bool isPool = true)
        {
            if (isPool && prefabPool.TryGetValue(name, out var go)) return go;
            prefabPool.Add(name, Resources.Load<GameObject>(name));
            return prefabPool[name];
        }

        public override async ETTask<GameObject> LoadPrefabAsync(string name, bool isPool = true)
        {
            if (isPool && prefabPool.TryGetValue(name, out var go)) return go;
            var tcs = ETTask<GameObject>.Create();
            var request = Resources.LoadAsync<GameObject>(name);
            request.completed += operation => { tcs.SetResult(request.asset as GameObject); };
            return await tcs;
        }

        public override Sprite LoadSprite(string name, bool isPool = true)
        {
            if (isPool && spritePool.TryGetValue(name, out var sprite)) return sprite;
            spritePool.Add(name, Resources.Load<Sprite>(name));
            return spritePool[name];
        }

        public override async ETTask<Sprite> LoadSpriteAsync(string name, bool isPool = true)
        {
            if (isPool && spritePool.TryGetValue(name, out var sprite)) return sprite;
            var tcs = ETTask<Sprite>.Create();
            var request = Resources.LoadAsync<Sprite>(name);
            request.completed += operation => { tcs.SetResult(request.asset as Sprite); };
            return await tcs;
        }

        public override SpriteAtlas LoadSpriteAtlas(string name, bool isPool = true)
        {
            if (isPool && spriteAtlasPool.TryGetValue(name, out var sprite)) return sprite;
            spriteAtlasPool.Add(name, Resources.Load<SpriteAtlas>(name));
            return spriteAtlasPool[name];
        }

        public override async ETTask<SpriteAtlas> LoadSpriteAtlasAsync(string name, bool isPool = true)
        {
            if (isPool && spriteAtlasPool.TryGetValue(name, out var sprite)) return sprite;
            var tcs = ETTask<SpriteAtlas>.Create();
            var request = Resources.LoadAsync<SpriteAtlas>(name);
            request.completed += operation => { tcs.SetResult(request.asset as SpriteAtlas); };
            return await tcs;
        }

        public override AudioClip LoadAudioClip(string name, bool isPool = true)
        {
            if (isPool && audioClipPool.TryGetValue(name, out var audioClip)) return audioClip;
            audioClipPool.Add(name, Resources.Load<AudioClip>(name));
            return audioClipPool[name];
        }

        public override async ETTask<AudioClip> LoadAudioClipAsync(string name, bool isPool = true)
        {
            if (isPool && audioClipPool.TryGetValue(name, out var audioClip)) return audioClip;
            var tcs = ETTask<AudioClip>.Create();
            var request = Resources.LoadAsync<AudioClip>(name);
            request.completed += operation => { tcs.SetResult(request.asset as AudioClip); };
            return await tcs;
        }

        public override T LoadAsset<T>(string name)
        {
            return Resources.Load<T>(name);
        }

        public override async ETTask<T> LoadAssetAsync<T>(string name)
        {
            var tcs = ETTask<T>.Create();
            var request = Resources.LoadAsync<T>(name);
            request.completed += operation => { tcs.SetResult(request.asset as T); };
            return await tcs;
        }
    }
}