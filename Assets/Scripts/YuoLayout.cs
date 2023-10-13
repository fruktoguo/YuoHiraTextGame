using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
using YuoTools;

public class YuoLayout : UIBehaviour, ILayoutElement, ILayoutGroup
{
    public float GridHeight;

    public Vector2 Spacing;

    [System.NonSerialized] private RectTransform m_Rect;

    protected RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }


    [SerializeField] private List<RectTransform> rectChildren = new List<RectTransform>();
    [SerializeField] protected RectOffset m_Padding = new RectOffset();

    /// <summary>
    /// The padding to add around the child layout elements.
    /// </summary>
    public RectOffset padding
    {
        get { return m_Padding; }
        set { SetProperty(ref m_Padding, value); }
    }

    /// <summary>
    /// Helper method used to set a given property if it has changed.
    /// </summary>
    /// <param name="currentValue">A reference to the member value.</param>
    /// <param name="newValue">The new value.</param>
    protected void SetProperty<T>(ref T currentValue, T newValue)
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            return;
        currentValue = newValue;
        SetDirty();
    }

    /// <summary>
    /// Mark the LayoutGroup as dirty.
    /// </summary>
    protected void SetDirty()
    {
        if (!IsActive())
            return;

        if (!CanvasUpdateRegistry.IsRebuildingLayout())
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        else
            StartCoroutine(DelayedSetDirty(rectTransform));
    }

    IEnumerator DelayedSetDirty(RectTransform rectTransform)
    {
        yield return null;
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public void SetLayoutHorizontal()
    {
    }

    public void SetLayoutVertical()
    {
        var rectChildrenCount = rectChildren.Count;
        for (int i = 0; i < rectChildrenCount; i++)
        {
            RectTransform rect = rectChildren[i];

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.SetSizeY(GridHeight);
        }

        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;
        int verIndex = 0;
        float horPos = padding.left;
        for (int i = 0; i < rectChildrenCount; i++)
        {
            var rect = rectChildren[i];
            var itemWidth = rect.rect.size.x;
            var itemHeight = rect.rect.size.y;
            if (horPos + itemWidth > width)
            {
                horPos = padding.left;
                verIndex++;
            }

            SetChildAlongAxis(rect, 0, horPos, itemWidth);
            SetChildAlongAxis(rect, 1, verIndex * (GridHeight + Spacing.y) + padding.top, itemHeight);

            horPos += itemWidth + Spacing.x;
        }
    }

    protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
    {
        if (rect == null)
            return;

        Vector2 anchoredPosition = rect.anchoredPosition;
        anchoredPosition[axis] =
            axis == 0 ? pos + size * rect.pivot[axis] : -pos - size * (1f - rect.pivot[axis]);
        rect.anchoredPosition = anchoredPosition;
    }

    public void CalculateLayoutInputHorizontal()
    {
        rectChildren.Clear();
        var toIgnoreList = ListPool<Component>.Get();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var rect = rectTransform.GetChild(i) as RectTransform;
            if (rect == null || !rect.gameObject.activeInHierarchy)
                continue;

            rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);

            if (toIgnoreList.Count == 0)
            {
                rectChildren.Add(rect);
                continue;
            }

            for (int j = 0; j < toIgnoreList.Count; j++)
            {
                var ignorer = (ILayoutIgnorer)toIgnoreList[j];
                if (!ignorer.ignoreLayout)
                {
                    rectChildren.Add(rect);
                    break;
                }
            }
        }

        ListPool<Component>.Release(toIgnoreList);
    }

    public void CalculateLayoutInputVertical()
    {
        rectChildren.Clear();
        var toIgnoreList = ListPool<Component>.Get();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var rect = rectTransform.GetChild(i) as RectTransform;
            if (rect == null || !rect.gameObject.activeInHierarchy)
                continue;

            rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);

            if (toIgnoreList.Count == 0)
            {
                rectChildren.Add(rect);
                continue;
            }

            for (int j = 0; j < toIgnoreList.Count; j++)
            {
                var ignorer = (ILayoutIgnorer)toIgnoreList[j];
                if (!ignorer.ignoreLayout)
                {
                    rectChildren.Add(rect);
                    break;
                }
            }
        }

        ListPool<Component>.Release(toIgnoreList);
    }

    public float minWidth => rectTransform.rect.width;
    public float preferredWidth => rectTransform.rect.width;
    public float flexibleWidth => rectTransform.rect.width;
    public float minHeight => rectTransform.rect.height;
    public float preferredHeight => rectTransform.rect.height;
    public float flexibleHeight => rectTransform.rect.height;
    public int layoutPriority { get; }
}