using UnityEngine;
using UnityEngine.UI;

namespace YuoTools
{
    public class ClickableAreaMask : MonoBehaviour , ICanvasRaycastFilter
    {
        public Image Area;

        public Image Mask;

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var maskR = Mask.IsRaycastLocationValid(sp, eventCamera);
            var areaR = Area.IsRaycastLocationValid(sp, eventCamera);
            //area的区域是可以点击的
            return areaR && !maskR;        
        }
    }
}