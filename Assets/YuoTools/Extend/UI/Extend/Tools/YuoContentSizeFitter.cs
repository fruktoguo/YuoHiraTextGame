using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YuoTools
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class YuoContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [System.NonSerialized] private RectTransform _mRect;

        [SerializeField] private RectTransform targetRect;

        private RectTransform rectTransform
        {
            get
            {
                if (_mRect == null)
                    _mRect = GetComponent<RectTransform>();
                return _mRect;
            }
        }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        private YuoContentSizeFitter()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        public Vector2 minSize;
        public Vector2 maxSize;

        private void HandleSelfFittingAlongAxis(int axis)
        {
            var a = (RectTransform.Axis)axis;
            rectTransform.SetSizeWithCurrentAnchors(a,
                a == RectTransform.Axis.Horizontal
                    ? targetRect.rect.width.RClamp(minSize.x, maxSize.x < 0.001f ? float.MaxValue : maxSize.x)
                    : targetRect.rect.height.RClamp(minSize.y, maxSize.y < 0.001f ? float.MaxValue : maxSize.y));
        }

        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }
}