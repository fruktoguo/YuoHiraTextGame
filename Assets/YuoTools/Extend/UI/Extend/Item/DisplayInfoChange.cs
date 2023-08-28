using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using YuoTools;

public class DisplayInfoChange : MonoBehaviour
{
    private YuoDropDown dropDown;
    public YuoDropDown DropDown { get => dropDown.YuoGet(this); set => dropDown = value; }

    private List<(string key, YuoDisplayInfo.DEVMODE value)> infos = new List<(string key, YuoDisplayInfo.DEVMODE value)>();
    public bool ChangeSystem;
    private void Start()
    {

        DropDown.Clear();
        var infosTemp = YuoDisplayInfo.GetScreenInfo();
        foreach (var item in infosTemp)
        {
            if (!DropDown.ContainsItem(item.key))
            {
                var dropItem = DropDown.AddItem(item.key);
                if (item.value.dmPelsWidth == Screen.width && item.value.dmPelsHeight == Screen.height)
                {
                    //print(item.value.dmDisplayFrequency.ToString());
                    int numerator = (int)Screen.mainWindowDisplayInfo.refreshRate.numerator;
                    if (numerator > 10000)
                    {
                        if (item.value.dmDisplayFrequency == numerator / 1000)
                        {
                            DropDown.nowItem = dropItem;
                        }
                    }
                    else
                    {
                        if (item.value.dmDisplayFrequency == numerator)
                        {
                            DropDown.nowItem = dropItem;
                        }
                    }
                }
                infos.Add(item);
            }
        }
        DropDown.OnValueChanged += x =>
        {
            // Debug.Log((infos[x.Index].value.dmPelsWidth,
            //     infos[x.Index].value.dmPelsHeight, infos[x.Index].value.dmDisplayFrequency));
            if (ChangeSystem)
            {
                YuoDisplayInfo.Change(infos[x.Index].value.dmPelsWidth,
                    infos[x.Index].value.dmPelsHeight, infos[x.Index].value.dmDisplayFrequency);
            }
            else
            {
                Screen.SetResolution(infos[x.Index].value.dmPelsWidth, infos[x.Index].value.dmPelsHeight,Screen.fullScreenMode);
                Application.targetFrameRate = infos[x.Index].value.dmDisplayFrequency;
            }
        };
        DropDown.SetItem($"{Screen.width}*{Screen.height}@{Screen.mainWindowDisplayInfo.refreshRate.numerator}Hz");
    }
}