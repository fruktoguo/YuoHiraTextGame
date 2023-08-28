using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.Extend;
using YuoTools.Main.Ecs;

namespace Tools
{
    public class TranslateComponent : MonoBehaviour
    {
        [OnValueChanged("OnTranslateTypeChange")] [HorizontalGroup(width: 400)]
        public TranslateType TranslateType;

        [ReadOnly] [HideLabel] [HorizontalGroup(width: 0)]
        public TranslateType LastType;

        [OnValueChanged("OnIDChange")] public string ID;

        Text textComponent;
        TextMeshProUGUI textMeshProUGUIComponent;
        Image imageComponent;
#if UNITY_EDITOR

        [OnInspectorInit]
        void OnInspectorInitialization()
        {
            if (!Application.isPlaying)
            {
                if (TranslateType == TranslateType.None)
                {
                    if (TryGetComponent(out textComponent))
                    {
                        TranslateType = TranslateType.Text;
                    }
                    else if (TryGetComponent(out textMeshProUGUIComponent))
                    {
                        TranslateType = TranslateType.TextMeshProUGUI;
                    }
                    else if (TryGetComponent(out imageComponent))
                    {
                        TranslateType = TranslateType.Image;
                    }

                    LastType = TranslateType;
                }
            }
        }

        void OnIDChange(string str)
        {
            //转小写
            ID = str.ToLower();
            //去掉空格
            ID = ID.Replace(" ", "");
        }

#endif

        private void Start()
        {
            Translate();
        }

        public void Translate()
        {
            switch (TranslateType)
            {
                case TranslateType.None:
                    break;
                case TranslateType.Text:
                    GetComponent<Text>().text = TranslateManagerComponent.Get.LoadText(ID);
                    break;
                case TranslateType.TextMeshProUGUI:
                    GetComponent<TextMeshProUGUI>().text = TranslateManagerComponent.Get.LoadText(ID);
                    break;
                case TranslateType.Image:
                    imageComponent = GetComponent<Image>();
                    var sprite = TranslateManagerComponent.Get.LoadSprite(ID);
                    //如果没找到图片，就用默认图片
                    if (sprite == null) break;
                    imageComponent.sprite = sprite;
                    if (imageComponent.rectTransform.anchorMin.x.ApEqual(0.5f) &&
                        imageComponent.rectTransform.anchorMax.x.ApEqual(0.5f))
                    {
                        imageComponent.SetNativeSize();
                    }

                    break;
                case TranslateType.Sound:
                    break;
                case TranslateType.Video:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnTranslateTypeChange(TranslateType translateType)
        {
            switch (translateType)
            {
                case TranslateType.Text:
                    if (TryGetComponent(out textComponent))
                    {
                        LastType = translateType;
                        return;
                    }

                    break;

                case TranslateType.TextMeshProUGUI:
                    if (TryGetComponent(out textMeshProUGUIComponent))
                    {
                        LastType = translateType;
                        return;
                    }

                    break;

                case TranslateType.Image:

                    if (TryGetComponent(out imageComponent))
                    {
                        LastType = translateType;
                        return;
                    }

                    break;
                case TranslateType.Sound:
                    break;
                case TranslateType.Video:
                    break;
            }

            TranslateType = LastType;
        }
    }

    public enum TranslateType
    {
        None = 0,
        Text = 10,
        TextMeshProUGUI = 11,
        Image = 20,
        Sound = 30,
        Video = 40,
    }
}