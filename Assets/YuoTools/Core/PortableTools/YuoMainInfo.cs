using UnityEngine;

public static class YuoMainInfo
{
    public static class ScreenInfo
    {
        //程序刷新率
        static double targetScreenRefreshRate;

        public static double TargetRefreshRate
        {
            get
            {
                if (targetScreenRefreshRate == 0)
                {
                    targetScreenRefreshRate = Screen.currentResolution.refreshRateRatio.value;
                    if (targetScreenRefreshRate > 1000)
                    {
                        targetScreenRefreshRate /= 1000;
                    }
                }

                return targetScreenRefreshRate;
            }
            set
            {
                targetScreenRefreshRate = value;
                Application.targetFrameRate = (int)targetScreenRefreshRate;
            }
        }

        //屏幕刷新率
        static double screenRefreshRate;

        public static double RefreshRate
        {
            get
            {
                if (screenRefreshRate == 0)
                {
                    screenRefreshRate = Screen.currentResolution.refreshRateRatio.value;
                    if (screenRefreshRate > 1000)
                    {
                        screenRefreshRate /= 1000;
                    }
                }

                return screenRefreshRate;
            }
        }

        //屏幕大小
        static float screenSize;

        public static float ScreenSize
        {
            get
            {
                if (screenSize == 0)
                {
                    screenSize = new Vector2(Screen.width, Screen.height).magnitude;
                }

                return screenSize;
            }
        }
    }
}