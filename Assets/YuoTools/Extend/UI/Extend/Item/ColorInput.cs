using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.UI;

using YuoTools;

public class ColorInput : SingletonMono<ColorInput>
{
    private RectTransform rect;
    public Button Confirm;
    public Button Cancell;
    public ColorRing colorRing;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        Hide();
    }

    public async ETTask<Color> GetColorAsync()
    {
        Color tempColor = colorRing.Color;
        rect.anchoredPosition = Input.mousePosition;
        Show();
        var tcs = ETTask<Color>.Create();
        Confirm.onClick.RemoveAllListeners();
        Confirm.onClick.AddListener(Hide);
        Cancell.onClick.RemoveAllListeners();
        Cancell.onClick.AddListener(Hide);
        Confirm.onClick.AddListener(() => tcs.SetResult(colorRing.Color));
        Cancell.onClick.AddListener(() => tcs.SetResult(tempColor));
        return await tcs;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}