namespace YuoTools.Extend.Helper
{
    public static class GameValueHelper
    {
        /// <summary>
        ///  将数值转换为百分比
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        public static float ValueToPercent(float value, float baseValue = 100)
        {
            if (value >= 0)
                return 1 - value / (value + baseValue);
            value = -value;
            return -(1 - value / (value + baseValue));
        }

        public static double ValueToPercent(double value, double baseValue = 100)
        {
            if (value >= 0)
                return 1 - value / (value + baseValue);
            value = -value;
            return -(1 - value / (value + baseValue));
        }
    }
}