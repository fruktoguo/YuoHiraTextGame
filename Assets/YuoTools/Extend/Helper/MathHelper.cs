using System;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace YuoTools.Extend.Helper
{
    public static class MathHelper
    {
        /// <summary>
        ///  弧度转角度
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float RadToDeg(float radians)
        {
            return (float)(radians * 180 / Math.PI);
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float DegToRad(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        /// <summary>
        ///  勾股定理
        /// </summary>
        public static float PythagoreanTheorem(float a, float b)
        {
            return (float)Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        ///  平方差公式
        /// </summary>
        public static float SquareDifference(float a, float b)
        {
            return (float)Math.Sqrt(Math.Pow(a, 2) - Math.Pow(b, 2));
        }

        /// <summary>
        /// 阶乘
        /// </summary>
        public static long Factorial(int a, int b = 1)
        {
            int sum = 1;
            for (; a > b; a--)
            {
                sum *= a;
            }

            return sum;
        }

        public static string RatioToString(float value)
        {
            return $"{value * 100}%";
        }

        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        public static Quaternion EulerToQuaternion(Vector3 eulerAngles) =>
            EulerToQuaternion(eulerAngles.x, eulerAngles.y, eulerAngles.z);

        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        public static Quaternion EulerToQuaternion(float x, float y, float z)
        {
            x *= Mathf.Deg2Rad * 0.5f;
            y *= Mathf.Deg2Rad * 0.5f;
            z *= Mathf.Deg2Rad * 0.5f;

            var sinX = Mathf.Sin(x);
            var cosX = Mathf.Cos(x);
            var sinY = Mathf.Sin(y);
            var cosY = Mathf.Cos(y);
            var sinZ = Mathf.Sin(z);
            var cosZ = Mathf.Cos(z);

            return new Quaternion(
                sinX * cosY * cosZ - cosX * sinY * sinZ,
                cosX * sinY * cosZ + sinX * cosY * sinZ,
                cosX * cosY * sinZ - sinX * sinY * cosZ,
                cosX * cosY * cosZ + sinX * sinY * sinZ
            );
        }

        /// <summary>
        /// 四元数转欧拉角
        /// </summary>
        public static Vector3 QuaternionToEuler(Quaternion quaternion)
        {
            float x, y, z;
            float quaternionX = quaternion.X;
            float quaternionY = quaternion.Y;
            float quaternionZ = quaternion.Z;
            float quaternionW = quaternion.W;
            float t0 = 2.0f * (quaternionW * quaternionX + quaternionY * quaternionZ);
            float t1 = 1.0f - 2.0f * (quaternionX * quaternionX + quaternionY * quaternionY);
            x = Mathf.Atan2(t0, t1);

            float t2 = 2.0f * (quaternionW * quaternionY - quaternionZ * quaternionX);
            t2 = t2 > 1.0f ? 1.0f : t2;
            t2 = t2 < -1.0f ? -1.0f : t2;
            y = Mathf.Asin(t2);

            float t3 = 2.0f * (quaternionW * quaternionZ + quaternionX * quaternionY);
            float t4 = 1.0f - 2.0f * (quaternionY * quaternionY + quaternionZ * quaternionZ);
            z = Mathf.Atan2(t3, t4);

            return new Vector3(x, y, z) * Mathf.Rad2Deg;
        }
    }
}