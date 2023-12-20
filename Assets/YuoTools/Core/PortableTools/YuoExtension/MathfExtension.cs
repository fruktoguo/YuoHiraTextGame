using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public static class MathfExtension
    {
        /// <summary>
        /// 获取个位数
        /// </summary>
        public static int SingleDigits(this int value) => value % 10;

        /// <summary>
        ///  获取十位数
        /// </summary>
        public static int TenDigits(this int value) => value / 10 % 10;

        /// <summary>
        /// 获取个位数
        /// </summary>
        public static int SingleDigits(this float value) => (int)(value % 10);

        /// <summary>
        ///  获取十位数
        /// </summary>
        public static int TenDigits(this float value) => (int)(value / 10 % 10);

        /// <summary>
        ///  小数点后几位
        /// </summary>
        public static int DecimalPlace(this float value, int count = 1) =>
            (int)(value * 10.Power(count) % 10.Power(count));

        #region Clamp Int

        public static int RClamp(this int i, int min, int max) => Mathf.Clamp(i, min, max);

        public static int RClamp(this int i, int max) => Mathf.Clamp(i, 0, max);

        public static int RClamp(this int i) => Mathf.Clamp(i, 0, i);

        public static int Clamp(ref this int i, int min, int max)
        {
            i = Mathf.Clamp(i, min, max);
            return i;
        }

        public static int Clamp(ref this int i, int max)
        {
            i = Mathf.Clamp(i, 0, max);
            return i;
        }

        public static int Clamp(ref this int i)
        {
            i = Mathf.Clamp(i, 0, i);
            return i;
        }

        #endregion

        #region Clamp float

        public static float Clamp(ref this float i, float min, float max)
        {
            i = Mathf.Clamp(i, min, max);
            return i;
        }

        public static float Clamp(ref this float i, float max)
        {
            i = Mathf.Clamp(i, 0, max);
            return i;
        }

        public static float Clamp(ref this float i)
        {
            i = Mathf.Clamp(i, 0, i);
            return i;
        }

        public static float RClamp(this float i, float min, float max) => Mathf.Clamp(i, min, max);

        public static float RClamp(this float i, float max) => Mathf.Clamp(i, 0, max);

        public static float RClamp(this float i) => Mathf.Clamp(i, 0, i);

        #endregion

        #region Clamp double

        public static double Clamp(ref this double i, double min, double max)
        {
            i = i < min ? min : i > max ? max : i;
            return i;
        }

        public static double Clamp(ref this double i, double max)
        {
            i = i > max ? max : i;
            return i;
        }

        public static double Clamp(ref this double i)
        {
            i = i < 0 ? 0 : i;
            return i;
        }

        public static double RClamp(this double i, double min, double max) => i < min ? min : i > max ? max : i;

        public static double RClamp(this double i, double max) => i > max ? max : i;

        public static double RClamp(this double i) => i < 0 ? 0 : i;

        #endregion

        #region Max Min Int

        public static int Max(ref this int i, int max)
        {
            i = Mathf.Max(i, max);
            return i;
        }

        public static int Min(ref this int i, int min)
        {
            i = Mathf.Min(i, min);
            return i;
        }

        public static int RMax(this int i, int max) => Mathf.Max(i, max);

        public static int RMin(this int i, int min) => Mathf.Min(i, min);

        #endregion

        #region Max Min float

        public static float Max(ref this float i, float max)
        {
            i = Mathf.Max(i, max);
            return i;
        }

        public static float Min(ref this float i, float min)
        {
            i = Mathf.Min(i, min);
            return i;
        }

        public static float RMax(this float i, float max) => Mathf.Max(i, max);

        public static float RMin(this float i, float min) => Mathf.Min(i, min);

        #endregion

        #region Max Min double

        public static double Max(ref this double i, double max)
        {
            i = i > max ? max : i;
            return i;
        }

        public static double Min(ref this double i, double min)
        {
            i = i < min ? min : i;
            return i;
        }

        public static double RMax(this double i, double max)
        {
            return i > max ? max : i;
        }

        public static double RMin(this double i, double min)
        {
            return i < min ? min : i;
        }

        public static double Min(ref this double i, double min1, double min2)
        {
            i = min1 < min2 ? min1 : min2;
            return i;
        }

        public static double Max(ref this double i, double max1, double max2)
        {
            i = max1 > max2 ? max1 : max2;
            return i;
        }

        #endregion

        public static float Abs(ref this float i)
        {
            i = Mathf.Abs(i);
            return i;
        }

        public static int Abs(ref this int i)
        {
            i = Mathf.Abs(i);
            return i;
        }

        public static float RAbs(this float i) => Mathf.Abs(i);

        public static int RAbs(this int i) => Mathf.Abs(i);

        public static bool IsSingle(this int i) => i % 2 == 1;

        /// <summary>
        /// 另一种取余
        /// 大于0就是正常的取余,小于0会取相反的,-15%4==7
        /// </summary>
        public static int Residual(this int value, int divisor) =>
            value % divisor >= 0 ? value % divisor : divisor + value % divisor;

        public static float Residual(this float value, float divisor) =>
            value >= 0 ? value % divisor : divisor + value % divisor;

        /// <summary>
        /// 交换两个数字
        /// </summary>
        public static void Swap(ref this float f1, ref float f2) => (f1, f2) = (f2, f1);

        /// <summary>
        /// 判断是否在范围内
        /// </summary>
        public static bool InRange(this float i, float min, float max) => i >= min && i <= max;

        /// <summary>
        /// 判断是否在范围内
        /// </summary>
        public static bool InRange(this int i, int min, int max) => i >= min && i <= max;

        /// <summary>
        /// 判断是否在范围内
        /// </summary>
        public static bool InRange(this Vector3 vector, Vector3 otherVector, float x, float y, float z) =>
            !(Mathf.Abs(vector.x - otherVector.x) > x) && !(Mathf.Abs(vector.y - otherVector.y) > y) &&
            !(Mathf.Abs(vector.z - otherVector.z) > z);

        /// <summary>
        /// 判断是否在范围内
        /// </summary>
        public static bool InRange(this Vector2 vector, Vector2 OtherVector, float x, float y) =>
            !(Mathf.Abs(vector.x - OtherVector.x) > x) && !(Mathf.Abs(vector.y - OtherVector.y) > y);

        /// <summary>
        /// 如果传入为0,则输出0.000001f
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static float NoZero(this float i) => i.ApEqual(0, 0.000001f) ? 0.000001f : i;

        /// <summary>
        /// 判断是否近似相等
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="accuracy">精确度</param>
        /// <returns></returns>
        public static bool ApEqual(this float value, float target, float accuracy = 0.01f) =>
            Mathf.Abs(value - target) < accuracy;

        /// <summary>
        /// 将一个bool变量变为相反的值
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Reverse(ref this bool b)
        {
            b = !b;
            return b;
        }

        /// <summary>
        /// 返回一个随机的bool值
        /// </summary>
        public static bool RandomBool<T>(this T random) => UnityEngine.Random.Range(0, 2).Equals(0);

        /// <summary>
        /// 返回一个随机的二维向量
        /// </summary>
        public static Vector3 RandomPos2D(float x, float y) =>
            Temp.V3.SetPos(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y), 0);

        /// <summary>
        /// 三维向量相乘
        /// </summary>
        public static Vector3 Multiply(this Vector3 a, Vector3 b) => Temp.V3.SetPos(a.x * b.x, a.y * b.y, a.z * b.z);

        public static float Power(this float p, float q) => Mathf.Pow(p, q);

        public static float Power(this int p, int q) => Mathf.Pow(p, q);

        /// <summary>
        /// 乘方
        /// </summary>
        public static float PowTwo(this float p)
        {
            return Mathf.Pow(p, 2);
        }

        /// <summary>
        /// 乘方
        /// </summary>
        public static int PowTwo(this int p)
        {
            return (int)Mathf.Pow(p, 2);
        }

        public static float PowThree(this float p)
        {
            return Mathf.Pow(p, 3);
        }

        public static int PowThree(this int p)
        {
            return (int)Mathf.Pow(p, 3);
        }

        public static int GainTo(this ref int i, int target, int unit = 1)
        {
            i = RGainTo(target, unit);
            return i;
        }

        public static int RGainTo(this int i, int target, int unit = 1)
        {
            if (i < target)
            {
                i += unit;
                if (i > target)
                    i = target;
            }
            else if (i > target)
            {
                i -= unit;
                if (i < target)
                    i = target;
            }

            return i;
        }


        public static float RGainTo(this float i, float target, float unit = 1)
        {
            if (i < target)
            {
                i += unit;
                if (i > target)
                    i = target;
            }
            else if (i > target)
            {
                i -= unit;
                if (i < target)
                    i = target;
            }

            return i;
        }

        public static float GainTo(this ref float i, float target, float unit = 1)
        {
            i = RGainTo(target, unit);
            return i;
        }

        /// <summary>
        /// float从一个区间映射到另一个区间
        /// </summary>
        /// <param name="i">需要映射的值</param>
        /// <param name="inMin">原区间最小值</param>
        /// <param name="inMax">原区间最大值</param>
        /// <param name="outMin">目标区间最小值</param>
        /// <param name="outMax">目标区间最大值</param>
        /// <returns></returns>
        public static float Remap(this float i, float inMin, float inMax, float outMin, float outMax)
        {
            return (i - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static float RemapClamp(this float i, float inMin, float inMax, float outMin, float outMax)
        {
            var result = i.Remap(inMin, inMax, outMin, outMax);
            return outMin <= outMax ? result.RClamp(outMin, outMax) : result.RClamp(outMax, outMin);
        }

        /// <summary>
        /// int从一个区间映射到另一个区间
        /// </summary>
        /// <param name="i">需要映射的值</param>
        /// <param name="inMin">原区间最小值</param>
        /// <param name="inMax">原区间最大值</param>
        /// <param name="outMin">目标区间最小值</param>
        /// <param name="outMax">目标区间最大值</param>
        /// <returns></returns>
        public static int Remap(this int i, int inMin, int inMax, int outMin, int outMax)
        {
            return (i - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static int RemapClamp(this int i, int inMin, int inMax, int outMin, int outMax)
        {
            var result = i.Remap(inMin, inMax, outMin, outMax);
            return outMin <= outMax ? result.RClamp(outMin, outMax) : result.RClamp(outMax, outMin);
        }

        /// <summary>
        /// 判断变量是否与另一组变量集合有交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool EqualsOr<T>(this T obj, T[] objs)
        {
            foreach (var item in objs)
            {
                if (Equals(item, obj))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 计算一堆三维坐标点中距离最远的线段(近似,适合位置数量较多的情况)
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static (Vector3 start, Vector3 end, float lenth) GetMaxLineAp(this List<Vector3> pointer)
        {
            if (pointer == null || pointer.Count < 2)
            {
                return (Vector3.zero, Vector3.zero, 0);
            }

            Vector3 Left = Vector3.zero;
            Vector3 Right = Vector3.zero;
            Vector3 Up = Vector3.zero;
            Vector3 down = Vector3.zero;
            Vector3 Front = Vector3.zero;
            Vector3 Back = Vector3.zero;
            foreach (var item in pointer)
            {
                if (item.x < Left.x)
                {
                    Left = item;
                }

                if (item.x > Right.x)
                {
                    Right = item;
                }

                if (item.y < down.y)
                {
                    down = item;
                }

                if (item.y > Up.y)
                {
                    Up = item;
                }

                if (item.z < Back.z)
                {
                    Back = item;
                }

                if (item.z > Front.z)
                {
                    Front = item;
                }
            }

            List<Vector3> temp = new List<Vector3>
            {
                Left,
                Right,
                Up,
                down,
                Front,
                Back
            };
            return GetMaxLine(temp);
        }

        /// <summary>
        /// 计算一堆三维坐标点中距离最远的线段(近似,适合位置数量较多的情况)
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static (Transform start, Transform end, float lenth) GetMaxLineAp(this List<Transform> pointer)
        {
            if (pointer == null || pointer.Count < 2)
            {
                return (null, null, 0);
            }

            var left = pointer[0];
            var right = pointer[0];
            var up = pointer[0];
            var down = pointer[0];
            var front = pointer[0];
            var back = pointer[0];
            foreach (var item in pointer)
            {
                if (item.position.x < left.position.x)
                {
                    left = item;
                }

                if (item.position.x > right.position.x)
                {
                    right = item;
                }

                if (item.position.y < down.position.y)
                {
                    down = item;
                }

                if (item.position.y > up.position.y)
                {
                    up = item;
                }

                if (item.position.z < back.position.z)
                {
                    back = item;
                }

                if (item.position.z > front.position.z)
                {
                    front = item;
                }
            }

            List<Transform> temp = new List<Transform>
            {
                left,
                right,
                up,
                down,
                front,
                back
            };
            return GetMaxLine(temp);
        }

        /// <summary>
        /// 计算一堆三维坐标点中距离最远的线段
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static (Vector3 start, Vector3 end, float lenth) GetMaxLine(this List<Vector3> pointer)
        {
            var start = Vector3.zero;
            var end = Vector3.zero;
            float maxLen = 0;
            for (int x = 0; x < pointer.Count; x++)
            {
                for (int y = pointer.Count - 1; y >= 0; y--)
                {
                    float len = (pointer[x] - pointer[y]).sqrMagnitude;
                    if (!(len > maxLen)) continue;
                    maxLen = len;
                    start = pointer[x];
                    end = pointer[y];
                }
            }

            return (start, end, Mathf.Sqrt(maxLen));
        }

        /// <summary>
        /// 计算一堆三维坐标点中距离最远的线段
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static (Transform start, Transform end, float lenth) GetMaxLine(this List<Transform> pointer)
        {
            var start = pointer[0];
            var end = pointer[0];
            float maxLen = 0;
            for (int x = 0; x < pointer.Count; x++)
            {
                for (int y = pointer.Count - 1; y >= 0; y--)
                {
                    float len = (pointer[x].position - pointer[y].position).sqrMagnitude;
                    if (len > maxLen)
                    {
                        maxLen = len;
                        start = pointer[x];
                        end = pointer[y];
                    }
                }
            }

            return (start, end, Mathf.Sqrt(maxLen));
        }
    }
}