using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using YuoTools;

namespace YuoTools.Chat
{
    public class YuoChatLoopListItem : YuoLoopListItem<MessageData>
    {
        public Image Head;
        public TextMeshProUGUI Message;
        public Image image;
        public TextMeshProUGUI UserName;
        private RectTransform rect;
        public RectTransform bubble;

        private void Awake()
        {
            rect = transform as RectTransform;
        }


        protected override void OnRenderer()
        {
            if (mData.user.UserName == ChatManager.Instance.Player)
            {
                rect.pivot = Vector2.right;
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.one;

                var r = Head.rectTransform;
                if (r != null)
                {
                    r.pivot = Vector2.one;
                    r.anchorMax = Vector2.one;
                    r.anchorMin = Vector2.one;
                }

                r = Message.rectTransform;
                if (r != null)
                {
                    r.pivot = Vector2.zero;
                    r.anchorMax = Vector2.zero;
                    r.anchorMin = Vector2.zero;
                }

                r = bubble;
                if (r != null)
                {
                    r.pivot = Vector2.zero;
                    r.anchorMax = Vector2.zero;
                    r.anchorMin = Vector2.zero;
                }

                UserName.gameObject.Hide();
            }
            else
            {
                rect.pivot = Vector2.zero;
                rect.anchorMax = Vector2.up;
                rect.anchorMin = Vector2.up;

                UserName.gameObject.Show();
                var r = Head.rectTransform;
                if (r != null)
                {
                    r.pivot = Vector2.up;
                    r.anchorMax = Vector2.up;
                    r.anchorMin = Vector2.up;
                }

                r = Message.rectTransform;
                if (r != null)
                {
                    r.pivot = Vector2.right;
                    r.anchorMax = Vector2.right;
                    r.anchorMin = Vector2.right;
                }

                r = bubble;
                if (r != null)
                {
                    r.pivot = Vector2.right;
                    r.anchorMax = Vector2.right;
                    r.anchorMin = Vector2.right;
                }
            }

            rect.SetAnchoredPosX(0);
            Message.text = mData.Message;
            UserName.text = mData.user.UserName;
            image.sprite = mData.image;
            image.gameObject.SetActive(image.sprite);
            Head.sprite = mData.user.Head;
#if UNITY_2021_3_OR_NEWER
            Message.enableWordWrapping = true;
#else
            Message.textWrappingMode = TextWrappingModes.Normal;
#endif
            Message.enableWordWrapping = true;
            Message.rectTransform.sizeDelta = mData.TextRect;
            var bubbleOffset = ChatManager.Instance.bubbleOffset;
            bubble.sizeDelta = mData.TextRect + Vector2.one * bubbleOffset;

            Message.rectTransform.SetAnchoredPosX(bubbleOffset / 2 *
                                                  (mData.user.UserName == ChatManager.Instance.Player ? 1 : -1));
            Message.rectTransform.SetAnchoredPosY(bubbleOffset / 2);
            rect.sizeDelta = mData.Rect;
            // LayoutRebuilder.ForceRebuildLayoutImmediate(Message.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(Message.transform.parent as RectTransform);
        }
    }
}