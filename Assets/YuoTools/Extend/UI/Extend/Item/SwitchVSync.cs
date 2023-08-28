using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using YuoTools;

public class SwitchVSync : MonoBehaviour
{
    private Toggle toggle;

    public static bool Open;

    public Toggle Toggle { get => toggle.YuoGet(this); set => toggle = value; }

    private void Awake()
    {
        Toggle.onValueChanged.AddListener(Switch);
        Switch(!(QualitySettings.vSyncCount == 0));
    }

    private void OnEnable()
    {
        Toggle.isOn = Open;
    }

    public void Switch(bool b)
    {
        Open = b;
        Toggle.SetIsOnWithoutNotify(b);
        QualitySettings.vSyncCount = b ? 1 : 0;
    }
}