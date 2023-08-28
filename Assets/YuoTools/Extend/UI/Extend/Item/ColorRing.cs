using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ColorRing : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IDragHandler
{
    public RectTransform Ring;
    public RectTransform Block;
    public RectTransform RingPointer;
    public RectTransform BlockPointer;

    private Image RingImage;
    private Image BlockImage;
    private EditType curType;
    private Color HSVColor = new Color(1, 1, 1, 1);

    public UnityEvent<Color> OnValueChange;
    private Color _color = new Color(1, 1, 1, 1);

    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                SetColorWithoutNotify(value);
                OnValueChange?.Invoke(_color);
            }
        }
    }

    private void Start()
    {
        RingImage = Ring.GetComponent<Image>();
        BlockImage = Block.GetComponent<Image>();
        Texture2D texture = new Texture2D(500, 500);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, RingColor(new Vector2((float)x / texture.width, (float)y / texture.height)));
            }
        }
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        RingImage.sprite = sprite;
    }

    private Color RingColor(Vector2 uv)
    {
        Vector2 d = uv * 2 - Vector2.one;
        float lenth = math.length(d);
        float a = math.smoothstep(0.8f, 0.81f, lenth) - math.smoothstep(0.99f, 1, lenth);
        var color = PosToColor(d);
        color.a = a;
        return color;
    }

    /// <summary>
    /// 基于中心点
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Color PosToColor(Vector2 pos)
    {
        pos = pos.normalized;
        var arc = math.acos(math.dot(Vector2.right, pos));
        if (pos.y < 0) arc = math.PI * 2 - arc;
        HSVColor.r = arc * 1f / math.PI / 2f;
        return Color.HSVToRGB(HSVColor.r, 1, 1);
    }

    public void SetColorWithoutNotify(Color value)
    {
        _color = value;
        _color.a = 1;

        Color.RGBToHSV(value, out HSVColor.r, out HSVColor.g, out HSVColor.b);
        BlockImage.color = Color.HSVToRGB(HSVColor.r, 1, 1);
        RingPointer.localEulerAngles = Vector3.forward * (HSVColor.r * 360 - 180);
        V2Temp.Set(HSVColor.g, HSVColor.b);
        BlockPointer.anchorMin = V2Temp;
        BlockPointer.anchorMax = V2Temp;
        BlockPointer.anchoredPosition = Vector2.zero;
    }

    private void OnRing(Vector2 pos)
    {
        BlockImage.color = PosToColor(pos - Ring.rect.center);
        RingPointer.localEulerAngles = Vector3.forward * (HSVColor.r * 360 - 180);
        Color = Color.HSVToRGB(HSVColor.r, HSVColor.g, HSVColor.b);
    }

    private void OnBlock(Vector2 pos)
    {
        pos.x = math.clamp(pos.x, 0, 1);
        pos.y = math.clamp(pos.y, 0, 1);
        HSVColor.g = pos.x;
        HSVColor.b = pos.y;
        BlockPointer.anchorMin = pos;
        BlockPointer.anchorMax = pos;
        BlockPointer.anchoredPosition = Vector2.zero;
        Color = Color.HSVToRGB(HSVColor.r, HSVColor.g, HSVColor.b);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Check(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Check(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPos = transform.worldToLocalMatrix.MultiplyPoint(Camera.main.ScreenToWorldPoint(eventData.position));
        Vector2 dir = localPos - Ring.rect.center;
        dir.Set(dir.x * 2 / Ring.rect.width, dir.y * 2 / Ring.rect.height);
        float r = dir.magnitude;
        if (r >= (1 - RingLen) && r <= 1)
        {
            curType = EditType.Ring;
            OnRing(dir);
        }
        else
        {
            dir += Vector2.one * 0.5f;
            if (dir.x > 0 && dir.x < 1 && dir.y > 0 && dir.y < 1)
            {
                curType = EditType.Block;
                OnBlock(dir);
            }
            else
            {
                curType = EditType.None;
            }
        }
    }

    private const float RingLen = 0.27f;

    private Vector2 V2Temp;

    private void Check(PointerEventData eventData)
    {
        if (curType == EditType.None) return;
        Vector2 localPos = transform.worldToLocalMatrix.MultiplyPoint(Camera.main.ScreenToWorldPoint(eventData.position));
        Vector2 dir = localPos - Ring.rect.center;
        dir.Set(dir.x * 2 / Ring.rect.width, dir.y * 2 / Ring.rect.height);
        if (curType == EditType.Ring)
        {
            OnRing(dir);
        }
        else
        {
            dir += Vector2.one * 0.5f;
            OnBlock(dir);
        }
    }

    private enum EditType
    {
        None,
        Ring,
        Block,
    }
}