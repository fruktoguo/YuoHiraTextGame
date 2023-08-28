using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using YuoTools;

public class RGBInputFiled : MonoBehaviour
{
    [SerializeField]
    private Color _color = new(0, 0, 0, 1);

    public InputField R;
    public InputField G;
    public InputField B;
    public UnityEvent<Color> OnValueChange;
    public Button colorPreview;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            R.text = ((int)(value.r * 255)).ToString();
            G.text = ((int)(value.g * 255)).ToString();
            B.text = ((int)(value.b * 255)).ToString();
            OnValueChange?.Invoke(value);
        }
    }

    private void Awake()
    {
        OnValueChange.AddListener(c => colorPreview.image.color = c);
        colorPreview.onClick.AddListener(GetColor);
        R.onEndEdit.AddListener(ChangeR);
        G.onEndEdit.AddListener(ChangeG);
        B.onEndEdit.AddListener(ChangeB);
    }

    private async void GetColor()
    {
        ColorInput.Instance.colorRing.SetColorWithoutNotify(Color);
        Color = await ColorInput.Instance.GetColorAsync();
    }

    public void ChangeR(string str)
    {
        var value = int.Parse(str);
        value = Mathf.Clamp(value, 0, 255);
        _color.r = value / 255f;
        R.SetTextWithoutNotify(value.ToString());
        OnValueChange?.Invoke(Color);
    }

    public void ChangeG(string str)
    {
        var value = int.Parse(str);
        value = Mathf.Clamp(value, 0, 255);
        _color.g = value / 255f;
        G.SetTextWithoutNotify(value.ToString());
        OnValueChange?.Invoke(Color);
    }

    public void ChangeB(string str)
    {
        var value = int.Parse(str);
        value = Mathf.Clamp(value, 0, 255);
        _color.b = value / 255f;
        B.SetTextWithoutNotify(value.ToString());
        OnValueChange?.Invoke(Color);
    }
}