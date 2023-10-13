using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using YuoTools;
using YuoTools.Extend.UI.YuoLayout;

namespace YuoTools
{
    public class YuoLoopList<T> : SerializedMonoBehaviour
    {
        /// <summary>
        /// 单元格尺寸（宽，高）
        /// </summary>
        [Header("大小")] public Vector2 CellSize;

        /// <summary>
        /// 单元格尺寸缩放
        /// </summary>
        [HorizontalGroup("")] [Header("缩放")] public Vector2 CellSizeScale = Vector2.one;

        /// <summary>
        /// 单元格间隙（水平，垂直）
        /// </summary>
        [Header("间距")] [HorizontalGroup("")] public Vector2 SpacingSize;

        /// <summary>
        /// 列数
        /// </summary>
        [Header("有多少列")] public int ColumnCount = 1;

        /// <summary>
        /// 单元格渲染器prefab
        /// </summary>
        [Header("预设体必须挂载YuoLoopListItem组件")] public YuoLoopListItem<T> RenderGO;

        /// <summary>
        /// 渲染格子数
        /// </summary>
        protected int mRendererCount;

        /// <summary>
        /// 父节点蒙版尺寸
        /// </summary>
        [HideInInspector] public Vector2 mMaskSize;

        /// <summary>
        /// 蒙版矩形
        /// </summary>
        [SerializeField] private Rect mRectMask;

        public ScrollRect mScrollRect;

        /// <summary>
        /// 转换器
        /// </summary>
        [HideInInspector] public RectTransform mRectTransformContainer;

        /// <summary>
        /// 渲染脚本集合
        /// </summary>
        [SerializeField] protected List<YuoLoopListItem<T>> mList_items = new List<YuoLoopListItem<T>>();

        /// <summary>
        /// 渲染格子字典
        /// </summary>
        [SerializeField]
        protected Dictionary<int, YuoLoopListRect> mDict_dRect = new Dictionary<int, YuoLoopListRect>();

        /// <summary>
        /// 数据提供者
        /// </summary>
        protected List<T> mDataProviders = new List<T>();

        protected bool mHasInited = false;

        private void Awake()
        {
            mRectTransformContainer = mScrollRect.transform.GetChild(0).GetChild(0) as RectTransform;
            InitRendererList();
        }

        private void Start()
        {
            mScrollRect.onValueChanged.AddListener(x => Update());
        }

        /// <summary>
        /// 初始化渲染脚本
        /// </summary>
        public virtual void InitRendererList(YuoLoopListItem<T>.OnSelect OnSelect = null,
            YuoLoopListItem<T>.OnUpdateData OnUpdate = null)
        {
            if (mHasInited) return;
            //获得蒙版尺寸
            mMaskSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;
            //通过蒙版尺寸和格子尺寸计算需要的渲染器个数
            mRendererCount = ColumnCount * (Mathf.CeilToInt(mMaskSize.y / GetBlockSizeY()) + 1);
            //CreatRects(mRendererCount);
            mList_items = new List<YuoLoopListItem<T>>();
            for (int i = 0; i < mRendererCount; ++i)
            {
                GameObject child = GameObject.Instantiate(RenderGO.gameObject, mScrollRect.content, true);
                child.name = $"{RenderGO.name}_{i}";
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                child.layer = gameObject.layer;
                ((RectTransform)child.transform).sizeDelta = CellSize * CellSizeScale;
                YuoLoopListItem<T> dfItem = child.GetComponent<YuoLoopListItem<T>>();
                if (dfItem == null)
                    throw new Exception("Render must extend DynamicInfinityItem");
                mList_items.Add(dfItem);
                mList_items[i].DRect = new YuoLoopListRect(1 * GetBlockSizeX(),
                    -i * GetBlockSizeY() - CellSize.y * CellSizeScale.y, CellSize.x * CellSizeScale.x,
                    CellSize.y * CellSizeScale.y, i);
                mList_items[i].DRect.index = -1;
                mList_items[i].OnSelectHandler = OnSelect;
                mList_items[i].OnUpdateDataHandler = OnUpdate;
                child.SetActive(false);
                UpdateChildTransformPos(dfItem, i);
            }

            SetListRenderSize(mRendererCount.Clamp(mDataProviders.Count));
            mHasInited = true;
        }

        public void SetItemAction(YuoLoopListItem<T>.OnSelect onSelect, YuoLoopListItem<T>.OnUpdateData onUpdate)
        {
            foreach (var item in mList_items)
            {
                item.OnSelectHandler = onSelect;
                item.OnUpdateDataHandler = onUpdate;
            }
        }

        /// <summary>
        /// 设置数据提供者,必须先初始化
        /// </summary>
        /// <param name="datas"></param>
        public void SetData(List<T> datas)
        {
            minStart = 0;
            CreatRects(datas.Count);
            SetListRenderSize(datas.Count);
            mDataProviders = datas;
            ClearAllListRenderDr();
        }

        public void SetDataCustomSize(List<T> datas, List<Vector2> rects)
        {
            ColumnCount = 1;
            minStart = 0;
            CreatRectsCustomSize(rects);
            float size = 0;
            foreach (var item in rects)
            {
                size += item.y;
            }

            SetListRenderCustomSize(size);
            mDataProviders = datas;
            ClearAllListRenderDr();
        }

        private float nowEndPos = 0;

        private float nowStartPos = 0;

        public void AddDataCSOnStart(List<T> data, List<Vector2> rects)
        {
            ColumnCount = 1;
            mRectTransformContainer.anchoredPosition = mRectTransformContainer.anchoredPosition.RAddY(-nowStartPos);
            int startTemp = minStart;
            minStart -= rects.Count;
            float size = 0;
            mDataProviders.AddRange(data);
            nowStartPos = mDict_dRect[startTemp].mRect.position.y;
            for (int i = startTemp - 1, j = rects.Count - 1, k = 0; j >= 0; i--, j--, k++)
            {
                nowStartPos += mDict_dRect[i + 1].mRect.height;
                // nowStartPos += rects[j].y;
                size += rects[j].y;
                YuoLoopListRect dRect = new YuoLoopListRect(GetBlockSizeX(), nowStartPos, rects[j].x, rects[j].y,
                    mDataProviders.Count - 1 - k);
                mDict_dRect[i] = dRect;
            }

            nowStartPos += mDict_dRect[minStart].mRect.height;
            mRectTransformContainer.sizeDelta = new Vector2(mRectTransformContainer.sizeDelta.x,
                mRectTransformContainer.sizeDelta.y + size);
            mRectTransformContainer.anchoredPosition = mRectTransformContainer.anchoredPosition.RAddY(nowStartPos);
            mScrollRect.verticalNormalizedPosition = 1 - mRectTransformContainer.anchoredPosition.y /
                (mRectTransformContainer.sizeDelta.y - (mScrollRect.transform as RectTransform).sizeDelta.y);
            foreach (var item in mList_items)
            {
                if (item.gameObject.activeSelf)
                {
                    var r = item.transform as RectTransform;
                    r.anchoredPosition = r.anchoredPosition.RSetY(item.DRect.mRect.position.y - nowStartPos);
                }
            }

            UpdateRender();
        }

        public void AddDataCSOnEnd(List<T> data, List<Vector2> rects)
        {
            ColumnCount = 1;
            int endTemp = mDict_dRect.Count + minStart - 1;
            float size = 0;
            mDataProviders.AddRange(data);
            nowEndPos = mDict_dRect[endTemp].mRect.position.y;
            //for (int i = startTemp - 1, j = rects.Count - 1, k = 0; j >= 0; i--, j--, k++)
            for (int i = endTemp + 1, k = 0; i <= endTemp + rects.Count; i++, k++)
            {
                nowEndPos -= rects[k].y;
                size += rects[k].y;
                YuoLoopListRect dRect = new YuoLoopListRect(GetBlockSizeX(), nowEndPos, rects[k].x, rects[k].y, i);
                mDict_dRect[i] = dRect;
            }

            //nowEndPos += mDict_dRect[MinStart].mRect.height;
            mRectTransformContainer.sizeDelta = new Vector2(mRectTransformContainer.sizeDelta.x,
                mRectTransformContainer.sizeDelta.y + size);
            UpdateRender();
        }

        public void AddDataOnEnd(T data, Vector2 rect)
        {
            ColumnCount = 1;
            int endTemp = mDict_dRect.Count + minStart;
            mDataProviders.Add(data);

            float height = rect.y * CellSizeScale.y + SpacingSize.y;

            nowEndPos = mDict_dRect.Count != 0 ? mDict_dRect[endTemp - 1].mRect.position.y : 0;
            nowEndPos -= height;
            YuoLoopListRect dRect = new YuoLoopListRect(GetBlockSizeX(), nowEndPos, rect.x, rect.y, endTemp);
            mDict_dRect[endTemp] = dRect;
            var sizeDelta = mRectTransformContainer.sizeDelta;
            sizeDelta.Set(sizeDelta.x, sizeDelta.y + height);
            mRectTransformContainer.sizeDelta = sizeDelta;
            UpdateRender();
        }

        /// <summary>
        /// 设置渲染列表的尺寸
        /// </summary>
        private void SetListRenderCustomSize(float size)
        {
            ColumnCount = 1;
            mRectTransformContainer.sizeDelta =
                new Vector2(mRectTransformContainer.sizeDelta.x, size);
            mRectMask = new Rect(0, -mMaskSize.y, mMaskSize.x, mMaskSize.y);
            mScrollRect.vertical = mRectTransformContainer.sizeDelta.y > mMaskSize.y;
        }

        private void CreatRectsCustomSize(List<Vector2> rects)
        {
            mDict_dRect = new Dictionary<int, YuoLoopListRect>();
            nowEndPos = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                int column = i % ColumnCount;
                nowEndPos -= rects[i].y;
                YuoLoopListRect dRect =
                    new YuoLoopListRect(column * GetBlockSizeX(), nowEndPos, rects[i].x, rects[i].y, i);
                mDict_dRect[i] = dRect;
            }
        }

        private void SetListRenderSize(int count)
        {
            mRectTransformContainer.sizeDelta =
                new Vector2(mRectTransformContainer.sizeDelta.x,
                    (Mathf.CeilToInt((count * 1.0f / ColumnCount)) * GetBlockSizeY()));
            mRectMask = new Rect(0, -mMaskSize.y, mMaskSize.x, mMaskSize.y);
            // mScrollRect.vertical = mRectTransformContainer.sizeDelta.y > mMaskSize.y;
        }

        private Vector2 updateChildTransformPosTemp;

        /// <summary>
        /// 更新各个渲染格子的位置
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        private void UpdateChildTransformPos(YuoLoopListItem<T> child, int index)
        {
            int row = index / ColumnCount;
            int column = index % ColumnCount;
            updateChildTransformPosTemp = Vector2.zero;
            //UpdateChildTransformPosTemp.x = column * GetBlockSizeX() + CellSize.x * CellSizeScale.x / 2;
            //UpdateChildTransformPosTemp.y = mDict_dRect[index].mRect.y;
            updateChildTransformPosTemp = child.DRect.mRect.position;
            updateChildTransformPosTemp.x = column * GetBlockSizeX() + CellSize.x * CellSizeScale.x / 2;
            updateChildTransformPosTemp.y -= nowStartPos;
            //UpdateChildTransformPosTemp.y = -CellSize.y * CellSizeScale.y - row * GetBlockSizeY();
            (child.transform as RectTransform).anchoredPosition3D = updateChildTransformPosTemp;
        }

        /// <summary>
        /// 获得格子块尺寸
        /// </summary>
        /// <returns></returns>
        protected float GetBlockSizeY()
        {
            return CellSize.y * CellSizeScale.y + SpacingSize.y;
        }

        protected float GetBlockSizeX()
        {
            return CellSize.x * CellSizeScale.x + SpacingSize.x;
        }

        /// <summary>
        /// 更新动态渲染格
        /// </summary>
        /// <param name="count"></param>
        private void CreatRects(int count)
        {
            mDict_dRect = new Dictionary<int, YuoLoopListRect>();
            for (int i = 0; i < count; ++i)
            {
                int row = i / ColumnCount;
                int column = i % ColumnCount;
                YuoLoopListRect dRect = new YuoLoopListRect(column * GetBlockSizeX(),
                    -row * GetBlockSizeY() - CellSize.y * CellSizeScale.y, CellSize.x * CellSizeScale.x,
                    CellSize.y * CellSizeScale.y, i);
                mDict_dRect[i] = dRect;
            }
        }

        /// <summary>
        /// 清理可复用渲染格
        /// 不需要public
        /// </summary>
        private void ClearAllListRenderDr()
        {
            if (mList_items != null)
            {
                int len = mList_items.Count;
                for (int i = 0; i < len; ++i)
                {
                    YuoLoopListItem<T> item = mList_items[i];
                    item.DRect = null;
                }
            }
        }

        /// <summary>
        /// 获得数据提供者
        /// </summary>
        /// <returns></returns>
        public IList GetDataProvider()
        {
            return mDataProviders;
        }

        /// <summary>
        /// 数据发生变化 供外部调用刷新列表
        /// </summary>
        [ContextMenu("RefreshDataProvider")]
        public void RefreshDataProvider()
        {
            if (mDataProviders == null)
                throw new Exception("dataProviders 为空！请先使用SetDataProvider ");
            CreatRects(mDataProviders.Count);
            SetListRenderSize(mDataProviders.Count);
            ClearAllListRenderDr();
            lastPos = new Vector2(-9999999, -999999);
        }

        /// <summary>
        /// 重新生成Grid
        /// </summary>
        public void RefreshGrid()
        {
            if (mDataProviders == null)
                throw new Exception("dataProviders 为空！请先使用SetDataProvider ");
            //获得蒙版尺寸
            mMaskSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;
            //通过蒙版尺寸和格子尺寸计算需要的渲染器个数
            mRendererCount = ColumnCount * (Mathf.CeilToInt(mMaskSize.y / GetBlockSizeY()) + 1);
            CreatRects(mDataProviders.Count);
            mList_items.ForEach(x => Destroy(x.gameObject));
            mList_items.Clear();
            for (int i = 0; i < mRendererCount; ++i)
            {
                GameObject child = Instantiate(RenderGO.gameObject);
                child.transform.SetParent(transform);
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                child.layer = gameObject.layer;
                (child.transform as RectTransform).sizeDelta = CellSize * CellSizeScale;
                YuoLoopListItem<T> dfItem = child.GetComponent<YuoLoopListItem<T>>();
                if (dfItem == null)
                    throw new Exception("Render must extend DynamicInfinityItem");
                mList_items.Add(dfItem);
                mList_items[i].DRect = mDict_dRect[i];
                //child.SetActive(false);
                UpdateChildTransformPos(dfItem, i);
            }

            SetListRenderSize(mDataProviders.Count);
        }

        #region 移动至数据

        /// <summary>
        /// 移动列表使之能定位到给定数据的位置上
        /// </summary>
        /// <param name="target"></param>
        public virtual void LocateRenderItemAtTarget(T target, float delay)
        {
            LocateRenderItemAtIndex(mDataProviders.IndexOf(target), delay);
        }

        public virtual void LocateRenderItemAtIndex(int index, float delay)
        {
            float target = index / ColumnCount / (float)((mDataProviders.Count - 1) / ColumnCount);
            target = 1 - target;
            target.Clamp();
            mScrollRect.verticalNormalizedPosition.To(target, delay,
                x => { mScrollRect.verticalNormalizedPosition = x; });
        }

        public virtual void LocateRenderItemAtIndex1(int index, float delay)
        {
            if (index < 0 || index > mDataProviders.Count - 1)
                throw new Exception("Locate Index Error " + index);
            index = Math.Min(index, mDataProviders.Count - mRendererCount.Log() + 2);
            index = Math.Max(0, index);
            Vector2 pos = mRectTransformContainer.anchoredPosition;
            int row = index / ColumnCount;
            row.Log();
            Vector2 v2Pos = new Vector2(pos.x, row * GetBlockSizeY() + CellSize.y * CellSizeScale.y * 0.5f + 8);
            StartCoroutine(TweenMoveToPos(pos, v2Pos, delay));
        }

        protected IEnumerator TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
        {
            bool running = true;
            float passedTime = 0f;
            while (running)
            {
                yield return null;
                passedTime += Time.deltaTime;
                Vector2 vCur;
                if (passedTime >= delay)
                {
                    vCur = v2Pos;
                    running = false;
                    mRectTransformContainer.anchoredPosition = vCur;
                    yield break;
                }
                else
                {
                    vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
                }

                mRectTransformContainer.anchoredPosition = vCur;
            }
        }

        #endregion 移动至数据

        [SerializeField] private Dictionary<int, YuoLoopListRect> inOverlaps = new Dictionary<int, YuoLoopListRect>();
        private int minStart = 0;

        protected void UpdateRender()
        {
            mRectMask.y = -mMaskSize.y - mRectTransformContainer.anchoredPosition.y + nowStartPos;
            inOverlaps.Clear();
            int Count = mDict_dRect.Count;
            Count.Clamp(mDataProviders.Count / ColumnCount.RClamp(1, ColumnCount));
            int min = (int)(Count * (1 - mScrollRect.verticalNormalizedPosition) -
                            ColumnCount * mRectMask.height / CellSize.y / CellSizeScale.y * 2);
            int max = (int)(Count * (1 - mScrollRect.verticalNormalizedPosition) +
                            ColumnCount * mRectMask.height / CellSize.y / CellSizeScale.y * 2);
            min += minStart;
            max += minStart;
            //print("min:" + min + " max:" + max + "minStart:" + MinStart + " mDict_dRect.Count:" + Count);
            min.Clamp(minStart, Count + minStart - 1);
            max.Clamp(minStart, Count + minStart - 1);
            for (int i = min; i <= max; i++)
            {
                if (mDict_dRect[i].Overlaps(mRectMask))
                {
                    inOverlaps.Add(mDict_dRect[i].index, mDict_dRect[i]);
                }
            }

            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                YuoLoopListItem<T> item = mList_items[i];
                if (item.DRect != null && !inOverlaps.ContainsKey(item.DRect.index))
                    item.DRect = null;
            }

            foreach (YuoLoopListRect dR in inOverlaps.Values)
            {
                if (GetDynmicItem(dR) == null)
                {
                    YuoLoopListItem<T> item = GetWaitItem();
                    item.DRect = dR;
                    UpdateChildTransformPos(item, dR.index);

                    if (mDataProviders != null && dR.index < mDataProviders.Count)
                    {
                        item.SetData(mDataProviders[dR.index]);
                    }
                }
            }

            inOverlaps.Clear();
        }

        /// <summary>
        /// 获得待渲染的渲染器
        /// </summary>
        /// <returns></returns>
        private YuoLoopListItem<T> GetWaitItem()
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                YuoLoopListItem<T> item = mList_items[i];
                if (item.DRect == null)
                    return item;
            }

            throw new Exception("Error");
        }

        /// <summary>
        /// 通过动态格子获得动态渲染器
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private YuoLoopListItem<T> GetDynmicItem(YuoLoopListRect rect)
        {
            int len = mList_items.Count;
            for (int i = 0; i < len; ++i)
            {
                YuoLoopListItem<T> item = mList_items[i];
                if (item.DRect == null)
                {
                    continue;
                }

                if (rect.index == item.DRect.index)
                {
                    return item;
                }
            }

            return null;
        }

        private Vector2 lastPos = new Vector2(-9999999, -999999);

        private void Update()
        {
            if (mHasInited)
            {
                //如果没有运动.几帧后会停止刷新
                if (!lastPos.x.ApEqual(mRectTransformContainer.anchoredPosition.x) ||
                    !lastPos.y.ApEqual(mRectTransformContainer.anchoredPosition.y))
                {
                    UpdateRender();
                    lastPos = Vector2.Lerp(lastPos, mRectTransformContainer.anchoredPosition, 0.9f);
                }
            }
        }
    }
}