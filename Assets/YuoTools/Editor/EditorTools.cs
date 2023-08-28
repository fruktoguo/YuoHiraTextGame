using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace YuoTools.YuoEditor
{
    [Obsolete("Obsolete")]
    public class EditorTools : UnityEditor.Editor
    {
        [MenuItem("GameObject/YuoUI_通用工具/精灵切换UI", false, -3)]
        public static void SwitchToUI()
        {
            GameObject[] selectObjs = Selection.gameObjects;
            foreach (var item in selectObjs)
            {
                var sr = item.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    var s = sr.sprite;
                    item.AddComponent<Image>().sprite = s;
                    DestroyImmediate(sr);
                }
            }
        }

        [MenuItem("GameObject/YuoUI_通用工具/获取文件路径", false, -3)]
        public static void GetFilePath()
        {
            foreach (var item in Selection.assetGUIDs)
            {
                Debug.Log(AssetDatabase.GUIDToAssetPath(item));
            }
        }

        public static List<T> GetAllSelectComponent<T>(bool andThis = false) where T : Component
        {
            List<T> list = new List<T>();

            foreach (var item in Selection.transforms)
            {
                if (andThis)
                {
                    T t = item.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }

                foreach (var item_1 in FindAll(item))
                {
                    T t = item_1.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }

        private static List<Transform> FindAll(Transform transform)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                list.Add(transform.GetChild(i));
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
            }

            return list;
        }

        [Obsolete("Obsolete")]
        public static void SwitchParticleAlpha(float alpha)
        {
            GameObject[] selectObjs = Selection.gameObjects;
            foreach (var item in selectObjs)
            {
                foreach (var item_1 in item.GetComponentsInChildren<Transform>())
                {
                    var ps = item_1.GetComponent<ParticleSystem>();
                    var sr = item_1.GetComponent<SpriteRenderer>();
                    if (ps)
                    {
                        ps.startColor = ps.startColor.RSetA(ps.startColor.a * alpha);
                    }

                    if (sr)
                    {
                        sr.color = sr.color.RSetA(sr.color.a * alpha);
                    }
                }
            }
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换选中/图片为精灵", false, -3)]
        private static void EditTexture()
        {
            Object[] selects;
            selects = Selection.objects;
            foreach (var item in selects)
            {
                if (item && item is Texture)
                {
                    string path = AssetDatabase.GetAssetPath(item);
                    TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                    texture.textureType = TextureImporterType.Sprite;
                    texture.alphaIsTransparency = true;
                    texture.spritePixelsPerUnit = 1;
                    texture.spriteImportMode = SpriteImportMode.Single;
                    texture.filterMode = FilterMode.Trilinear;
                    texture.mipmapEnabled = false;
                }
            }
        }

        private static void SetSortingOrder(int i)
        {
            foreach (var item in GetAllSelectComponent<SpriteRenderer>(true))
            {
                item.sortingOrder += i;
            }

            foreach (var item in GetAllSelectComponent<ParticleSystem>(true))
            {
                item.GetComponent<Renderer>().sortingOrder += i;
            }
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换物体显隐状态 &q", false, -3)]
        public static void SetObjActive()
        {
            GameObject[] selectObjs = Selection.gameObjects;
            Undo.SetCurrentGroupName($"切换物体显隐状态 [数量:{selectObjs.Length}]");
            Undo.RecordObjects(selectObjs, "切换物体显隐状态");
            int objCtn = selectObjs.Length;
            for (int i = 0; i < objCtn; i++)
            {
                bool isAcitve = selectObjs[i].activeSelf;
                selectObjs[i].SetActive(!isAcitve);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("Assets/导出精灵", false, -3)]
        public static void ExportSprite()
        {
            string resourcesPath = "Assets/Resources/";
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj);
                if (selectionPath.StartsWith(resourcesPath))
                {
                    string selectionExt = System.IO.Path.GetExtension(selectionPath);
                    if (selectionExt.Length == 0)
                    {
                        Debug.LogError($"资源{selectionPath}的扩展名不对，请选择图片资源");
                        continue;
                    }

                    // 如果selectionPath = "Assets/Resources/UI/Common.png"
                    // 那么loadPath = "UI/Common"
                    string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length);
                    loadPath = loadPath.Substring(resourcesPath.Length);
                    // 加载此文件下的所有资源
                    Sprite[] sprites = Resources.LoadAll<Sprite>(loadPath);
                    if (sprites.Length > 0)
                    {
                        // 创建导出目录
                        string exportPath = Application.dataPath + "/ExportSprite/" + loadPath;
                        System.IO.Directory.CreateDirectory(exportPath);

                        foreach (Sprite sprite in sprites)
                        {
                            Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height,
                                sprite.texture.format, false);
                            tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin,
                                (int)sprite.rect.width, (int)sprite.rect.height));
                            tex.Apply();

                            // 将图片数据写入文件
                            System.IO.File.WriteAllBytes(exportPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                        }

                        Debug.Log("导出精灵到" + exportPath);
                    }

                    Debug.Log("导出精灵完成");
                    // 刷新资源
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"请将资源放在{resourcesPath}目录下");
                }
            }
        }

        [MenuItem("Assets/名字全部大写", false, -3)]
        public static void RenameToUpper()
        {
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj);
                string selectionName = System.IO.Path.GetFileNameWithoutExtension(selectionPath);
                string newName = selectionName.ToUpper();
                AssetDatabase.RenameAsset(selectionPath, newName);
                AssetDatabase.Refresh();
            }
        }
    }
}