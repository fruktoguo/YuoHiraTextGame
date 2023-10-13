using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using YuoTools;

[CreateAssetMenu(fileName = "TextureAssets", menuName = "Assets/TextureAssets")]
public class TextureAssets : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "GUID", ValueLabel = "Texture")]
    public Dictionary<string, Texture> Textures = new();

    [DictionaryDrawerSettings(KeyLabel = "spriteID", ValueLabel = "Sprite")]
    public Dictionary<string, Sprite> SpriteAssets = new();

    [Button]
    public void LoadAllAssets()
    {
        Textures = new();
        foreach (var texture in Resources.FindObjectsOfTypeAll<Texture>())
        {
            var path = AssetDatabase.GetAssetPath(texture).Log();
            if (!path.IsNullOrSpace() && path.StartsWith("Assets"))
            {
                Textures.TryAdd(AssetDatabase.GUIDFromAssetPath(path).ToString(), texture);
            }
        }
        foreach (var texture in Resources.FindObjectsOfTypeAll<Sprite>())
        {
            var path = AssetDatabase.GetAssetPath(texture).Log();
            SpriteAssets.TryAdd(AssetDatabase.GUIDFromAssetPath(path).ToString(), texture);
        }
    }
    //
    // [OnValueChanged("LoadAllAssets")] [Sirenix.OdinInspector.FilePath]
    // public string Path;
}