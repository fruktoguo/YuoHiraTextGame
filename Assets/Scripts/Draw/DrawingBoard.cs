using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YuoTools;
using UnityEngine.EventSystems;
using YuoTools.Extend.Helper;

public class DrawingBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Texture2D Texture;
    [SerializeField] private RawImage image;
    public Color DrawColor = Color.black;

    private Vector2 previousPoint;

    private bool isInit;

    private void Start()
    {
        if (!isInit)
        {
            var component = GetComponent<RawImage>();
            if (component != null)
            {
                Init(component);
            }
        }
    }

    public void Init(RawImage rawImage)
    {
        isInit = true;
        image = rawImage;
        // 初始化texture
        var rect = image.rectTransform.rect;
        rect.size.Log();
        Texture = new Texture2D((int)rect.size.x, (int)rect.size.y);
        Color[] pixels = Texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        Texture.SetPixels(pixels);
        Texture.Apply();

        image.texture = Texture;
    }

    public int DrawLineWidth = 1;

    void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        // 计算线段的所有点
        foreach (Vector2 point in BresenhamLine(from, to, DrawLineWidth))
        {
            var v = point;
            if (OnDrawPointer != null)
                v = OnDrawPointer.Invoke(v);
            Texture.SetPixel((int)v.x, (int)v.y, color);
        }

        Texture.Apply();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        previousPoint = RectTransformHelper.GetSelectPointer(image.rectTransform, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var currentPoint = RectTransformHelper.GetSelectPointer(image.rectTransform, eventData);
        // 在前一个点和当前点之间画线
        DrawLine(previousPoint, currentPoint, DrawColor);

        previousPoint = currentPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        previousPoint = Vector2.zero;
    }

    // Bresenham线算法
    IEnumerable<Vector2> BresenhamLine(Vector2 p0, Vector2 p1, int lineWidth)
    {
        int dx = Mathf.Abs((int)p1.x - (int)p0.x), sx = (int)p0.x < (int)p1.x ? 1 : -1;
        int dy = -Mathf.Abs((int)p1.y - (int)p0.y), sy = (int)p0.y < (int)p1.y ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            for (int i = -lineWidth / 2; i <= lineWidth / 2; i++)
            {
                for (int j = -lineWidth / 2; j <= lineWidth / 2; j++)
                {
                    Vector2 point = new Vector2((int)p0.x + i, (int)p0.y + j);
                    if (point.x >= 0 && point.x < Texture.width && point.y >= 0 && point.y < Texture.height)
                    {
                        yield return point;
                    }
                }
            }

            if ((int)p0.x == (int)p1.x && (int)p0.y == (int)p1.y) break;
            e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                p0.x += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                p0.y += sy;
            }
        }
    }

    public delegate Vector2 DrawPointDelegate(Vector2 point);

    public DrawPointDelegate OnDrawPointer = null;

    private float layerScale;

    public float LayerScale
    {
        get => layerScale;
        set
        {
            layerScale = value;
            image.rectTransform.localScale = new Vector3(layerScale, layerScale, 1);
        }
    }
}