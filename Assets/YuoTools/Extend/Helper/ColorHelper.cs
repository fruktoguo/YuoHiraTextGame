using UnityEngine;

namespace YuoTools.Extend.Helper
{
    public static class ColorHelper
    {
        public static Color RandomLovelyColor()
        {
            float r = 0;
            float g = 0;
            float b = 0;
            //定义3个颜色备用
            float c1 = 1f;
            float c2 = 150 / 255f;
            float c3 = Random.Range(150 / 255f, 1f);
            //将3个颜色随机分配给R,G,B
            int choose = Random.Range(0, 6);
            switch (choose)
            {
                case 0:
                    r = c1;
                    g = c2;
                    b = c3;
                    break;
                case 1:
                    r = c1;
                    g = c3;
                    b = c2;
                    break;
                case 2:
                    r = c2;
                    g = c1;
                    b = c3;
                    break;
                case 3:
                    r = c2;
                    g = c3;
                    b = c1;
                    break;
                case 4:
                    r = c3;
                    g = c1;
                    b = c2;
                    break;
                case 5:
                    r = c3;
                    g = c2;
                    b = c1;
                    break;
            }
            Temp.color.Set(r, g, b,1);
            return Temp.color;
        }
    }
}