using System.Collections.Generic;
using DefaultNamespace.Draw;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;

public class YuoDrawTool : MonoBehaviour
{
    public List<DrawingBoard> Layers { get; private set; } = new List<DrawingBoard>();

    private RectTransform layerRoot;

    public YuoDrawLayerManager LayerManager { get; set; }

    private bool isInit;

    private void Awake()
    {
        if (!isInit) Init();
        AddLayer();
    }

    public void SetColor(Color color)
    {
        Layers.ForEach(layer => layer.DrawColor = color);
    }

    public void SetLineWidth(float width)
    {
        Layers.ForEach(layer => layer.DrawLineWidth = (int)width);
    }

    private void Init()
    {
        "DrawToolInit".Log();
        isInit = true;
        layerRoot = transform.Find("Layers") as RectTransform;
        LayerManager = transform.Find("Tools/LayerManager").GetComponent<YuoDrawLayerManager>();
    }

    [Button]
    public void AddLayer()
    {
        if (!isInit) Init();
        var go = new GameObject($"Layer_{Layers.Count}", typeof(DrawingBoard), typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        var board = go.GetComponent<DrawingBoard>();

        SetLayerRectTransform(rect);
        SetBoardComponent(board, go, rect);

        Layers.Add(board);
        LayerManager.AddLayerPreview(board);
    }

    private void SetLayerRectTransform(RectTransform rectTransform)
    {
        rectTransform.SetParent(layerRoot);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = layerRoot.rect.size;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void SetBoardComponent(DrawingBoard board, GameObject go, RectTransform rect)
    {
        var rawImage = go.AddComponent<RawImage>();
        board.Init(rawImage);
        rect.sizeDelta = rawImage.rectTransform.rect.size;
    }

    [Button]
    public void ClearLayers()
    {
        Layers.ForEach(layer => DestroyImmediate(layer.gameObject));
        Layers.Clear();
    }

    [Button]
    public void SetLayerScale(float scale = 1)
    {
        LayerManager.CurrentLayer.LayerScale = scale;
    }
}