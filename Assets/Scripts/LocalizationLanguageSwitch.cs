using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class LocalizationLanguageSwitch : MonoBehaviour
{
    void Start()
    {
        var dropDown = GetComponent<YuoDropDown>();
        var allLanguage = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in allLanguage)
        {
          YuoDropDown.DropItem item =  dropDown.AddItem(locale.LocaleName);
          item.Action = locale;
        }

        dropDown.OnValueChanged += item =>
        {
            LocalizationSettings.SelectedLocale = (Locale)item.Action;
        };
        dropDown.SetItem(LocalizationSettings.SelectedLocale.LocaleName);
    }

    // Update is called once per frame
    void Update()
    {
    }
}