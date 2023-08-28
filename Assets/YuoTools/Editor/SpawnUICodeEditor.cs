using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.Editor;
using YuoTools.Extend.UI;
using YuoTools.YuoEditor;
using Object = UnityEngine.Object;

[Obsolete("Obsolete")]
public class SpawnUICodeEditor
{
    // [MenuItem("GameObject/YuoUI/生成窗口UI代码", false, -2)]
    // public static void CreateUICode()
    // {
    //     foreach (var go in Selection.gameObjects)
    //     {
    //         SpawnUICode.SpawnCode(go);
    //     }
    // }

    [MenuItem("GameObject/YuoUI/创建UI", false, -2)]
    public static void CreateUI()
    {
        GameObject go = Resources.Load<GameObject>("YuoUI/UI_Window");
        go = GameObject.Instantiate(go, Selection.activeGameObject.transform);
        go.name = "New_UI_Window";
    }

    [MenuItem("GameObject/YuoUI/将Text改成TMP", false, -2)]
    public static void ChangeTextToTMP()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var text in EditorTools.GetAllSelectComponent<Text>(true))
        {
            var go = text.gameObject;
            var message = text.text;
            Object.DestroyImmediate(text);
            go.AddComponent<TextMeshProUGUI>().text = message;
        }
    }

    [MenuItem("GameObject/YuoUI/将TMP改成Text", false, -2)]
    public static void ChangeTMPToText()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var text in EditorTools.GetAllSelectComponent<TextMeshProUGUI>(true))
        {
            var go = text.gameObject;
            var message = text.text;
            Undo.DestroyObjectImmediate(text);
            Undo.AddComponent<Text>(go).text = message;
        }
    }

    [MenuItem("GameObject/YuoUI命名/将UI的名字改成图片的名字", false, -2)]
    public static void ChangeNameForSprite()
    {
        var sprites = EditorTools.GetAllSelectComponent<Image>(true);
        Undo.RecordObjects(sprites.ToArray(), "ChangeNameForSprite");
        foreach (var image in sprites)
        {
            image.name = image.sprite?.name;
        }
    }

    [MenuItem("GameObject/YuoUI命名/将UI的名字改成文本", false, -2)]
    public static void ChangeNameForText()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        var texts = EditorTools.GetAllSelectComponent<Text>(true);
        var tmps = EditorTools.GetAllSelectComponent<TextMeshProUGUI>(true);
        var all = new List<Object>();
        all.AddRange(texts);
        all.AddRange(tmps);
        Undo.RecordObjects(all.ToArray(), "ChangeNameForText");
        foreach (var text in texts)
        {
            text.name = text.text;
        }

        foreach (var text in tmps)
        {
            text.name = text.text;
        }
    }

    private static long _lastTime = long.MinValue;

    static void ChangeUITag(string thisTag, Object go)
    {
        foreach (var tag in SpawnUICodeConfig.AllTag)
        {
            if (go.name.StartsWith(tag))
            {
                if (tag == thisTag)
                {
                    go.name = go.name.Replace(thisTag, "");
                }
                else
                {
                    go.name = go.name.Replace(tag, "");
                    go.name = thisTag + go.name;
                }

                return;
            }
        }

        go.name = thisTag + go.name;
    }

    [MenuItem("GameObject/YuoUI命名/切换是否被框架检索_C", false, -2)]
    public static void ChangeNameForFrame()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;

        Object[] selections = Selection.objects;
        Undo.SetCurrentGroupName($"切换是否被框架检索_C [数量:{selections.Length}]");
        Undo.RecordObjects(selections, "切换是否被框架检索_C");
        foreach (Object go in selections)
        {
            ChangeUITag(SpawnUICodeConfig.UIComponentTag, go);
        }

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    [MenuItem("GameObject/YuoUI命名/切换是否为变体的组件_CV", false, -2)]
    public static void ChangeNameForFrameVariant()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        Object[] selections = Selection.objects;
        Undo.SetCurrentGroupName($"切换是否为变体的组件_CV [数量:{selections.Length}]");
        Undo.RecordObjects(selections, "切换是否为变体的组件_CV");
        foreach (var go in selections)
        {
            ChangeUITag(SpawnUICodeConfig.VariantChildComponentTag, go);
        }

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    [MenuItem("GameObject/YuoUI命名/切换UI子面板_D", false, -2)]
    public static void ChangeNameForChild()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var go in Selection.gameObjects)
        {
            ChangeUITag(SpawnUICodeConfig.ChildUITag, go);
        }
    }

    [MenuItem("GameObject/YuoUI命名/切换UI子面板变体_DV", false, -2)]
    public static void ChangeNameForChildVariant()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var go in Selection.gameObjects)
        {
            ChangeUITag(SpawnUICodeConfig.VariantChildUITag, go);
        }
    }

    [MenuItem("GameObject/YuoUI命名/切换公共UI_G", false, -2)]
    public static void ChangeNameForG()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var go in Selection.gameObjects)
        {
            ChangeUITag(SpawnUICodeConfig.GeneralUITag, go);
        }
    }

    [MenuItem("GameObject/YuoUI命名/移除空格", false, -2)]
    public static void ChangeNameRemoveSpace()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;
        foreach (var go in Selection.gameObjects)
        {
            go.name = go.name.Replace(" ", "");
        }
    }

    [MenuItem("GameObject/YuoUI命名/正则批量修改", false, -2)]
    public static void ChangeNameRegular()
    {
        if (_lastTime.Equals(System.DateTime.Now.Ticks / 10000000))
        {
            return;
        }

        _lastTime = System.DateTime.Now.Ticks / 10000000;

        RegularRenameObjectsWindow.ShowWindow();
    }
}
