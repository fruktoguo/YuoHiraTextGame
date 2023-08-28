using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SwitchTargetDisplay : MonoBehaviour
{
    public List<DisplayInfo> displayInfos { get; private set; } = new List<DisplayInfo>();

    private YuoDropDown dropDown;

    private void Start()
    {
        dropDown = GetComponent<YuoDropDown>();
        Screen.GetDisplayLayout(displayInfos);
        for (int i = 0; i < displayInfos.Count; i++)
        {
            var item = displayInfos[i];
            dropDown.AddItem($"{item.name}[{i + 1}]");
            if (item.Equals(Screen.mainWindowDisplayInfo))
            {
                dropDown.SetItem($"{Screen.mainWindowDisplayInfo.name}[{i + 1}]");
            }
        }
        dropDown.OnValueChanged += x =>
        {
            Screen.MoveMainWindowTo(displayInfos[x.Index], Vector2Int.zero);
        };
    }
}