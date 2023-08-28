using System.Collections.Generic;

using UnityEngine;

namespace YuoTools
{
    public static class YuoTool
    {
        /// <summary>
        /// 根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="start"></param>起始点
        /// <param name="con"></param>控制点
        /// <param name="end"></param>目标点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        public static Vector3 CubicBezierPoint(float t, Vector3 start, Vector3 con, Vector3 end)
        {
            return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * con + t * t * end;
        }

        public static Vector2 BezierPoint2D(float t, Vector2 start, Vector2 con, Vector2 end)
        {
            return
                (1 - t) * (1 - t) * start + 2 * (1 - t) * t * con + t * t * end;
        }
        
        public static float BezierNum(float t, float start, float con, float end)
        {
            return
                (1 - t) * (1 - t) * start + 2 * (1 - t) * t * con + t * t * end;
        }

        /// <summary>
        /// 二维贝塞尔
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="con">控制点</param>
        /// <param name="end">终点</param>
        /// <param name="segments">采样数量</param>
        /// <returns>返回的数组</returns>
        public static Vector2[] Bezier(Vector2 start, Vector2 con, Vector2 end, int segments)
        {
            float d = 1f / segments;
            Vector2[] points = new Vector2[segments - 1];
            for (int i = 0; i < points.Length; i++)
            {
                float t = d * (i + 1);
                points[i] = (1 - t) * (1 - t) * con + 2 * t * (1 - t) * start + t * t * end;
            }
            List<Vector2> rps = new List<Vector2>();
            rps.Add(con);
            rps.AddRange(points);
            rps.Add(end);
            return rps.ToArray();
        }

        /// <summary>
        /// 三维塞尔尔
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint"></param>控制点
        /// <param name="endPoint"></param>目标点
        /// <param name="segment"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static Vector3[] Bezier(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segment)
        {
            Vector3[] path = new Vector3[segment];
            for (int i = 1; i <= segment; i++)
            {
                float t = i / (float)segment;
                Vector3 pixel = CubicBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path[i - 1] = pixel;
            }
            return path;
        }

        /// <summary>
        ///复制内容到剪切板
        /// </summary>
        /// <param name="input"></param>
        public static void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
        CopyTextToClipboard_iOS(input);
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass tool = new AndroidJavaClass("com.my.ugcf.Tool");
        tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif
        }

        ///<summary>
        ///用最小二乘法拟合二元多次曲线
        ///例如y=ax+b
        ///其中MultiLine将返回a，b两个参数。
        ///a对应MultiLine[1]
        ///b对应MultiLine[0]
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        public static float[] MultiLine(float[] arrX, float[] arrY, int length, int dimension)
        {
            int n = dimension + 1;                  //dimension次方程需要求 dimension+1个 系数
            float[,] Guass = new float[n, n + 1];      //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumPowerArr(arrX, j + i, length);
                }
                Guass[i, j] = SumPowerArrs(arrX, i, arrY, 1, length);
            }

            return ComputGauss(Guass, n);

            /// <summary>
            /// 求数组的元素的n次方的和
            /// </summary>
            /// <param name="arr"></param>
            /// <param name="power"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            float SumPowerArr(float[] arr, int power, int length)
            {
                float s = 0;
                for (int i = 0; i < length; i++)
                {
                    if (arr[i] != 0 || power != 0)
                        s = s + Mathf.Pow(arr[i], power);
                    else
                        s = s + 1;
                }
                return s;
            }
            /// <summary>
            ///  求数组的元素的n次方的乘积的和
            /// </summary>
            /// <param name="arr1"></param>
            /// <param name="power1"></param>
            /// <param name="arr2"></param>
            /// <param name="power2"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            float SumPowerArrs(float[] arr1, int power1, float[] arr2, int power2, int length)
            {
                float s = 0;
                for (int i = 0; i < length; i++)
                {
                    if ((arr1[i] != 0 || power1 != 0) && (arr2[i] != 0 || power2 != 0))
                        s = s + Mathf.Pow(arr1[i], power1) * Mathf.Pow(arr2[i], power2);
                    else
                        s = s + 1;
                }
                return s;
            }
            /// <summary>
            /// 返回函数的系数
            /// </summary>
            /// <param name="Guass"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            float[] ComputGauss(float[,] Guass, int n)
            {
                int i, j;
                int k, m;
                float temp;
                float max;
                float s;
                float[] x = new float[n];

                for (i = 0; i < n; i++) x[i] = 0;//初始化

                for (j = 0; j < n; j++)
                {
                    max = 0;

                    k = j;
                    for (i = j; i < n; i++)
                    {
                        if (Mathf.Abs(Guass[i, j]) > max)
                        {
                            max = Guass[i, j];
                            k = i;
                        }
                    }

                    if (k != j)
                    {
                        for (m = j; m < n + 1; m++)
                        {
                            temp = Guass[j, m];
                            Guass[j, m] = Guass[k, m];
                            Guass[k, m] = temp;
                        }
                    }

                    if (0 == max)
                    {
                        // "此线性方程为奇异线性方程"

                        return x;
                    }

                    for (i = j + 1; i < n; i++)
                    {
                        s = Guass[i, j];
                        for (m = j; m < n + 1; m++)
                        {
                            Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                        }
                    }
                }//结束for (j=0;j<n;j++)

                for (i = n - 1; i >= 0; i--)
                {
                    s = 0;
                    for (j = i + 1; j < n; j++)
                    {
                        s = s + Guass[i, j] * x[j];
                    }

                    x[i] = (Guass[i, n] - s) / Guass[i, i];
                }

                return x;
            }
        }
    }
}