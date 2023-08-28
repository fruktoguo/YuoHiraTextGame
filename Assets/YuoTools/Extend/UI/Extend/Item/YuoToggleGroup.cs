using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class YuoToggleGroup : UIBehaviour
{
    public UnityEvent<YuoToggle> onValueChanged;

    [ReadOnly] public YuoToggle NowToggle;
    [ReadOnly] [SerializeField] List<YuoToggle> _toggles = new List<YuoToggle>();

    public void UnSelectToggle(YuoToggle toggle)
    {
        if (toggle == NowToggle)
            NowToggle = null;
    }

    public void SelectToggle(YuoToggle toggle)
    {
        if (toggle != NowToggle)
        {
            NowToggle = toggle;
            onValueChanged.Invoke(toggle);
            _toggles.ForEach(t =>
            {
                if (t != toggle)
                    t.isOn = false;
            });
        }
    }

    public bool AllowSwitchOff;

    public bool Check(YuoToggle toggle, bool value)
    {
        if (AllowSwitchOff)
            return value;

        if (!value)
        {
            //如果是当前选中的Toggle，那么就不允许关闭
            if (toggle == NowToggle)
                return true;
        }

        return value;
    }

    public void RegisterToggle(YuoToggle toggle)
    {
        if (!_toggles.Contains(toggle))
            _toggles.Add(toggle);
    }

    public void UnregisterToggle(YuoToggle toggle)
    {
        if (_toggles.Contains(toggle))
            _toggles.Remove(toggle);
    }
}