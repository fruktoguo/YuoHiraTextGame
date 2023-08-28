using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools.Extend.Helper;

namespace YuoTools.Extend.UI.LoopList
{
    public class YuoLoopList : MonoBehaviour
    {
        /// <summary>
        /// ��Ԫ��ߴ磨���ߣ�
        /// </summary>
        [FoldoutGroup("��Ԫ��")] [LabelText("��С")]
        public Vector2 cellSize = Vector2.one * 100;

        /// <summary>
        /// ��Ԫ��ߴ�����
        /// </summary>
        [FoldoutGroup("��Ԫ��")] [LabelText("����")]
        public Vector2 cellSizeScale = Vector2.one;

        /// <summary>
        /// ��Ԫ���϶��ˮƽ����ֱ��
        /// </summary>
        [FoldoutGroup("��Ԫ��")] [LabelText("���")]
        public Vector2 spacingSize;

        /// <summary>
        /// ����
        /// </summary>
        [FoldoutGroup("��Ԫ��")] [LabelText("����")] [HorizontalGroup("��Ԫ��/1")]
        public bool horizontal = true;

        [FoldoutGroup("��Ԫ��")] [LabelText("��/����")] [HorizontalGroup("��Ԫ��/1")]
        public int columnCount = 4;

        /// <summary>
        /// ��Ԫ����Ⱦ��prefab
        /// </summary>
        [FoldoutGroup("��Ԫ��")] [LabelText("��Ԫ��Ԥ����")]
        public GameObject renderGo;

        [FoldoutGroup("��Ԫ��/��߾�")] public float top, bottom, left, right;

        /// <summary>
        /// ��Ⱦ������
        /// </summary>
        private int _mRendererCount;

        [LabelText("������Ⱦ������")] public int extraRenderingCount;

        /// <summary>
        /// ���ڵ��ɰ�ߴ�
        /// </summary>
        [HideInInspector] public Vector2 mMaskSize;

        /// <summary>
        /// �ɰ����
        /// </summary>
        private Rect _mRectMask;

        public ScrollRect mScrollRect;

        /// <summary>
        /// ת����
        /// </summary>
        public RectTransform mRectTransformContainer;

        /// <summary>
        /// ��Ⱦ�ű�����
        /// </summary>
        private List<YuoLoopListItem> _mListItems;

        /// <summary>
        /// �����ṩ��
        /// </summary>
        private List<Vector2Int> _mDataProviders;

        public List<Rect> rectData = new();

        /// <summary>
        /// ��ʼ����Ⱦ�ű�
        /// </summary>
        public void Init()
        {
            //����ɰ�ߴ�
            mMaskSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;

            _mRectMask.size = mMaskSize;

            //ͨ���ɰ�ߴ�͸��ӳߴ������Ҫ�ĵ�Ԫ�����
            _mRendererCount = columnCount * (Mathf.CeilToInt(mMaskSize.y / BlockSizeY) + 1 + extraRenderingCount * 2);
            _mListItems = new List<YuoLoopListItem>();
            SetAllRect();
            //���ɵ�Ԫ��
            for (int i = 0; i < _mRendererCount; ++i)
            {
                GameObject child = Instantiate(renderGo, mRectTransformContainer, true);

                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                child.layer = gameObject.layer;
                child.name = "Item_" + i;
                ((RectTransform)child.transform).sizeDelta = cellSize * cellSizeScale;
                YuoLoopListItem item = child.GetComponent<YuoLoopListItem>();

                if (item == null)
                {
                    item = child.AddComponent<YuoLoopListItem>();
                }

                _mListItems.Add(item);

                child.SetActive(true);
                var rectTransform = child.transform as RectTransform;
                item.rectTransform = rectTransform;
                rectTransform.pivot = Vector2.up;
                rectTransform.anchorMin = Vector2.up;
                rectTransform.anchorMax = Vector2.up;
                UpdateChildTransformPos(item, i);
            }

            mHasInited = true;
        }

        private void OnScroll(Vector2 pos)
        {
            UpdateRender();
        }

        /// <summary>
        /// ���¸�����Ⱦ���ӵ�λ��
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        private void UpdateChildTransformPos(YuoLoopListItem child, int index)
        {
            int row = index / columnCount;
            int column = index % columnCount;

            if (!horizontal) (row, column) = (column, row);

            Temp.V2.Set(column * BlockSizeX + left, -BlockSizeY * row - top);

            child.rectTransform.anchoredPosition3D = Vector3.zero;
            child.rectTransform.anchoredPosition = Temp.V2;
            child.rect = rectData[index];
        }

        private readonly Dictionary<int, Rect> _inOverlaps = new Dictionary<int, Rect>();

        void UpdateRender()
        {
            _mRectMask.y = -mMaskSize.y - mRectTransformContainer.anchoredPosition.y;
            _inOverlaps.Clear();

            int min = (int)(rectData.Count * (1 - mScrollRect.verticalNormalizedPosition) - _mRendererCount);
            min.Clamp();
            int max = (int)(rectData.Count * (1 - mScrollRect.verticalNormalizedPosition) + _mRendererCount);
            max.Clamp(rectData.Count);
            var newMask = _mRectMask;
            if (extraRenderingCount > 0)
            {
                Temp.V2.Set(BlockSizeX * extraRenderingCount * 2, BlockSizeY * extraRenderingCount * 2);
                newMask.size += Temp.V2;
                Temp.V2.Set(BlockSizeX * extraRenderingCount, BlockSizeY * extraRenderingCount);
                newMask.position -= Temp.V2;
            }

            //��ȡ���ܻ�ˢ�µĸ���
            for (int i = min; i < max; i++)
            {
                if (rectData[i].Overlaps(newMask))
                {
                    _inOverlaps.Add(i, rectData[i]);
                }
            }

            WaitItems.Clear();

            foreach (var item in _mListItems)
            {
                if (!item.rect.Overlaps(_mRectMask))
                {
                    WaitItems.Add(item);
                }
            }

            (WaitItems.Count, _inOverlaps.Count).Log();
            foreach (var dR in _inOverlaps)
            {
                if (GetItem(dR.Value) == null)
                {
                    YuoLoopListItem item = GetWaitItem();
                    if (item != null)
                    {
                        item.rect = dR.Value;
                        UpdateChildTransformPos(item, dR.Key);

                        if (_mDataProviders != null && dR.Key < _mDataProviders.Count)
                        {
                            item.SetData(dR.Key);
                        }
                    }
                    else
                    {
                        Debug.LogError($"û�п��õĵ�Ԫ��{WaitItems.Count}");
                    }
                }
                else
                {
                    //Debug.Log("��⵽����Ҫˢ�µĸ���");
                }
            }
        }

        public List<YuoLoopListItem> WaitItems = new();

        YuoLoopListItem GetWaitItem()
        {
            if (WaitItems.Count > 0)
            {
                YuoLoopListItem item = WaitItems[0];
                WaitItems.RemoveAt(0);
                return item;
            }

            return null;
        }

        YuoLoopListItem GetItem(Rect rect)
        {
            foreach (var item in _mListItems)
            {
                if (item.rect == rect)
                {
                    return item;
                }
            }

            return null;
        }

        public void SetData(List<Vector2Int> data)
        {
            _mDataProviders = data;
            if (mMaskSize == Vector2.zero) Init();

            UpdateRender();
        }

        void SetAllRect()
        {
            rectData.Clear();
            for (int i = 0; i < _mDataProviders.Count; i++)
            {
                int row = i / columnCount;
                int column = i % columnCount;
                Rect dRect = new Rect(column * BlockSizeX,
                    -row * BlockSizeY - cellSize.y * cellSizeScale.y, cellSize.x * cellSizeScale.x,
                    cellSize.y * cellSizeScale.y);
                rectData.Add(dRect);
            }

            //����������С
            if (horizontal)
            {
                mRectTransformContainer.sizeDelta = new Vector2(left + right + BlockSizeX * columnCount, -top - bottom +
                    Mathf.CeilToInt(_mDataProviders.Count / (float)columnCount) * BlockSizeY);
            }
            else
            {
                mRectTransformContainer.sizeDelta = new Vector2(
                    left + right + Mathf.CeilToInt(_mDataProviders.Count / (float)columnCount) * BlockSizeY,
                    -top - bottom + mRectTransformContainer.sizeDelta.y);
            }

            //���ù�����
            mScrollRect.verticalNormalizedPosition = 1;
        }

        private Vector2 lastPos = new Vector2(-9999999, -999999);
        protected bool mHasInited = false;

        private void Update()
        {
            if (mHasInited)
            {
                //���û���˶�.��֡���ֹͣˢ��
                if (!lastPos.x.ApEqual(mRectTransformContainer.anchoredPosition.x) ||
                    !lastPos.y.ApEqual(mRectTransformContainer.anchoredPosition.y))
                {
                    UpdateRender();
                    lastPos = Vector2.Lerp(lastPos, mRectTransformContainer.anchoredPosition, 0.9f);
                }
            }
        }

        /// <summary>
        /// ��ø��ӿ�ߴ�
        /// </summary>
        /// <returns></returns>
        private float BlockSizeY =>
            cellSize.y * cellSizeScale.y + spacingSize.y;

        private float BlockSizeX =>
            cellSize.x * cellSizeScale.x + spacingSize.x;
    }
}