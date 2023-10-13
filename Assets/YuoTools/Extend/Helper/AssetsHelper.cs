using UnityEditor;
using UnityEngine;

namespace YuoTools.Extend.Helper
{
    public class AssetsHelper
    {
        public static Texture LoadTexture(string guid)
        {
            TextureAssets assets = Resources.Load<TextureAssets>("TextureAssets");
            return assets.Textures[guid];            
        }
    }
}