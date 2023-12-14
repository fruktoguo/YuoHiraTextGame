using UnityEngine;
using UnityEngine.EventSystems;

namespace YuoTools.Extend.Helper
{
    public class RectTransformHelper
    {
        /// <summary>
        /// 返回点击到的位置,位置零点永远为左下角
        /// </summary>
        public static Vector2 GetSelectPointer(RectTransform rect, PointerEventData eventData, bool clamp = true)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position,
                eventData.pressEventCamera, out var localPoint);

            var size = rect.rect.size;
            var pivot = rect.pivot;

            float x = (localPoint.x + size.x * pivot.x) / size.x * size.x;
            float y = (localPoint.y + size.y * pivot.y) / size.y * size.y;

            if (clamp)
            {
                x = Mathf.Clamp(x, 0, size.x);
                y = Mathf.Clamp(y, 0, size.y);
            }

            return new Vector2(x, y);
        }

        public static Vector2 GetSelectPointerRatio(RectTransform rect, PointerEventData eventData, bool clamp = true)
        {
            var pos = GetSelectPointer(rect, eventData, clamp);
            return new Vector2(pos.x / rect.rect.size.x, pos.y / rect.rect.size.y);
        }

        /// <summary>
        /// 返回点击到的位置,以自身坐标系零点为零点
        /// </summary>
        public static Vector2 GetSelectPointerForLocal(RectTransform rect, PointerEventData eventData,
            bool clamp = true)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position,
                    eventData.pressEventCamera, out var localPoint))
            {
                if (clamp)
                {
                    localPoint.x = Mathf.Clamp(localPoint.x, 0, rect.rect.size.x);
                    localPoint.y = Mathf.Clamp(localPoint.y, 0, rect.rect.size.y);
                }

                return localPoint;
            }

            return Vector2.zero;
        }
    }
}