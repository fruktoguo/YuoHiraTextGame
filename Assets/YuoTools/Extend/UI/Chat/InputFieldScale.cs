using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using YuoTools;
using UnityEngine.Pool;

public class InputFieldScale : MonoBehaviour
{
    private TMP_Text text;
    private TMP_InputField inputField;
    private RectTransform rect;

    [Obsolete("Obsolete")]
    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        rect = transform as RectTransform;
        text = inputField.textComponent;
        inputField.onValueChanged.AddListener(UpdateScale);
    }

    [Obsolete("Obsolete")]
    private void UpdateScale(string str)
    {
        text.text = str;
#if UNITY_2021_2_OR_NEWER
        text.enableWordWrapping = true;
#else
        text.textWrappingMode = TextWrappingModes.Normal;
#endif
        rect.sizeDelta = rect.sizeDelta.RSetY(LayoutUtility.GetPreferredHeight(rect).RClamp(60, 100000));
    }
}