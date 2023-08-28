using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using System.IO;
using ET;
using UnityEngine.AddressableAssets;
using YuoTools.Main.Ecs;

namespace YuoTools
{
    public abstract class AssetsLoadComponent : YuoComponent
    {
        public abstract GameObject LoadPrefab(string name, bool isPool = true);

        public abstract ETTask<GameObject> LoadPrefabAsync(string name, bool isPool = true);
        public abstract Sprite LoadSprite(string name, bool isPool = true);

        public abstract ETTask<Sprite> LoadSpriteAsync(string name, bool isPool = true);
        public abstract SpriteAtlas LoadSpriteAtlas(string name, bool isPool = true);

        public abstract ETTask<SpriteAtlas> LoadSpriteAtlasAsync(string name, bool isPool = true);
        public abstract AudioClip LoadAudioClip(string name, bool isPool = true);

        public abstract ETTask<AudioClip> LoadAudioClipAsync(string name, bool isPool = true);
        public abstract T LoadAsset<T>(string name) where T : Object;

        public abstract ETTask<T> LoadAssetAsync<T>(string name) where T : Object;
    }
}