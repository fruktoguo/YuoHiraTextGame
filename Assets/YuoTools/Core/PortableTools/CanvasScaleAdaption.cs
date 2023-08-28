using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace YuoTools
{
    public class CanvasScaleAdaption : MonoBehaviour
    {
        private CanvasScaler scaler;
        public Vector2 DefSize = new Vector2(2160, 1080);
        public Vector2 MinSize = new Vector2(1920, 1920);
        public Vector2 MaxSize = new Vector2(3840, 1080);

        [Range(0, 1)]
        public float ScalerValue_Def = 0.5f;

        [Range(0, 1)]
        public float ScalerValue_Min = 0.35f;

        [Range(0, 1)]
        public float ScalerValue_Max = 0.65f;

        private void Start()
        {
            scaler = GetComponent<CanvasScaler>();
            float def = DefSize.x / DefSize.y;
            float min = MinSize.x / MinSize.y;
            float max = MaxSize.x / MaxSize.y;
            float now = Screen.width / (float)Screen.height;
            float scalerValue;
            if (now >= def)
            {
                scalerValue = now.Remap(def, max, ScalerValue_Def, ScalerValue_Max);
            }
            else
            {
                scalerValue = now.Remap(def, min, ScalerValue_Def, ScalerValue_Min);
            }
            scaler.matchWidthOrHeight = scalerValue.RClamp(0, 1f);
        }
    }
}