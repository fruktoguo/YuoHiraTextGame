using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YuoTools.Extend.Helper
{
    public static class StringHelper
    {
        public static byte[] ToBytes(string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToByteArray(string str)
        {
            var byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToUtf8(string str)
        {
            var byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

        public static byte[] HexToBytes(string hexString)
        {
            if (hexString.Length % 2 != 0)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The binary key cannot have an odd number of digits: {0}", hexString));

            var hexAsBytes = new byte[hexString.Length / 2];
            for (var index = 0; index < hexAsBytes.Length; index++)
            {
                var byteValue = "";
                byteValue += hexString[index * 2];
                byteValue += hexString[index * 2 + 1];
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }

        public static string Fmt(string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ListToString<T>(this List<T> list)
        {
            var sb = new StringBuilder();
            foreach (var t in list)
            {
                sb.Append(t);
                sb.Append(",");
            }

            return sb.ToString();
        }

        public static string ArrayToString<T>(this T[] args)
        {
            if (args == null) return "";

            var argStr = " [";
            for (var arrIndex = 0; arrIndex < args.Length; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1) argStr += ", ";
            }

            argStr += "]";
            return argStr;
        }

        public static string ArrayToString<T>(this T[] args, int index, int count)
        {
            if (args == null) return "";

            var argStr = " [";
            for (var arrIndex = index; arrIndex < count + index; arrIndex++)
            {
                argStr += args[arrIndex];
                if (arrIndex != args.Length - 1) argStr += ", ";
            }

            argStr += "]";
            return argStr;
        }

        /// <summary>
        /// 是否为身份证并且是18岁以上
        /// </summary>
        /// <param idNumber="身份证号码"></param>
        /// <returns></returns>
        public static string CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (idNumber.Length != 18) return "身份证号码长度错误";
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return "身份证号码信息错误"; //数字验证
            }

            string address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return "身份证号码省份信息错误"; //省份验证
            }

            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }

            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return "身份证号码校验码错误"; //校验码验证
            }

            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return "身份证号码出生信息错误"; //生日验证
            }

            if (DateTime.Now.Year - time.Year < 18)
            {
                return "年龄未满18岁"; //生日验证
            }
            //18岁当年
            else if (DateTime.Now.Year - time.Year == 18)
            {
                if (DateTime.Now.Month - time.Month >= 0)
                {
                    //18岁当月
                    if (DateTime.Now.Month - time.Month == 0)
                    {
                        if (DateTime.Now.Day - time.Day < 0)
                        {
                            return "年龄未满18岁";
                        }
                    }
                }

                return "年龄未满18岁";
            }

            return "正确"; //符合GB11643-1999标准
        }

        /// <summary>
        /// 移除前缀字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemovePrefixString(string self, string str)
        {
            string strRegex = @"^(" + str + ")";
            return Regex.Replace(self, strRegex, "");
        }

        /// <summary>
        /// 移除后缀字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSuffixString(string self, string str)
        {
            string strRegex = @"(" + str + ")" + "$";
            return Regex.Replace(self, strRegex, "");
        }

        /// <summary>
        /// 是否为Email
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEmail(string self)
        {
            return self.RegexMatch(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        /// <summary>
        /// 是否为域名
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsDomain(string self)
        {
            return self.RegexMatch(@"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?");
        }

        /// <summary>
        /// 是否为IP地址
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsIP(string self)
        {
            return self.RegexMatch(@"((?:(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d))");
        }

        /// <summary>
        /// 是否为手机号码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string self)
        {
            return self.RegexMatch(@"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
        }

        /// <summary>
        /// 是否为中文
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsChinese(string self)
        {
            return self.RegexMatch("[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool RegexMatch(string self, string pattern)
        {
            Regex reg = new Regex(pattern);
            return reg.Match(self).Success;
        }

        /// <summary>
        /// 转换为MD5, 加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string ConvertToMD5(string self, string flag = "x2")
        {
            byte[] sor = Encoding.UTF8.GetBytes(self);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString(flag));
            }

            return strbul.ToString();
        }

        /// <summary>
        /// 转换为32位MD5
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ConvertToMD5_32(string self)
        {
            return ConvertToMD5(self, "x2");
        }

        /// <summary>
        /// 转换为48位MD5
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ConvertToMD5_48(string self)
        {
            return ConvertToMD5(self, "x3");
        }

        /// <summary>
        /// 转换为64位MD5
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ConvertToMD5_64(string self)
        {
            return ConvertToMD5(self, "x4");
        }

        /// <summary>
        /// 将十六位颜色字符串转为Color
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        public static Color Str16ToColor(string colorName)
        {
            if (colorName.Length < 8)
                colorName += "FF";
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                r = Convert.ToByte((v >> 24) & 255) / 255f,
                g = Convert.ToByte((v >> 16) & 255) / 255f,
                b = Convert.ToByte((v >> 8) & 255) / 255f,
                a = Convert.ToByte((v >> 0) & 255) / 255f,
            };
        }

        public static bool IsNumber(string str)
        {
            return Regex.IsMatch(str, @"^[+-]?\d*$");
        }

        public static int SeedToInt(string seed)
        {
            Temp.Int = int.MaxValue / 10 * (int)Mathf.PerlinNoise(seed.Length, seed.Length);
            foreach (var item in seed)
            {
                Temp.Int += item;
                Temp.Int %= (int.MaxValue / 2);
            }

            return Temp.Int;
        }

        public static decimal ToDecimal(string value)
        {
            return decimal.Parse(value);
        }

        public static decimal ToDecimal(string value, decimal defaultValue)
        {
            var result = defaultValue;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }

        public static decimal ToRoundDecimal(string value, decimal defaultValue, int decimals)
        {
            var result = defaultValue;
            result = System.Math.Round(decimal.TryParse(value, out result) ? result : defaultValue, decimals);
            return result;
        }

        public static decimal? ToNullableDecimal(string value)
        {
            decimal result;
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// 字符串中字符出现次数
        /// </summary>
        /// <param name="s"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static int SubStringCount(string s, string sub)
        {
            if (s.Contains(sub))
            {
                string sReplaced = s.Replace(sub, "");
                return (s.Length - sReplaced.Length) / sub.Length;
            }

            return 0;
        }

        public static int StringNameSort(string x, string y)
        {
            return String.Compare(x, y, StringComparison.Ordinal);
        }

        /// <summary>
        /// 获取某个限定之后的字符串 如果限定存在多个则取最后一个的位置
        /// </summary>
        public static string GetAfterKeyData(this string str, string key)
        {
            int index = str.LastIndexOf(key) + key.Length;
            int length = str.Length - index;
            return str.Substring(index, length);
        }

        /// <summary>
        /// 获取某个限定之前的字符串 如果限定存在多个则取最后一个的位置
        /// </summary>
        public static string GetBeforeKeyData(this string str, string key)
        {
            int index = str.LastIndexOf(key);
            return str.Substring(0, index);
        }

        /// <summary>
        /// 获取限定范围内的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="star"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string GetLimitStr(this string str, string star, string end)
        {
            if (star.IndexOf(end, 0) != -1)
                return "";
            //转换为在字符串第几位
            int i = str.LastIndexOf(star);
            //第二个与第一个index差值
            int j = str.LastIndexOf(end);
            if (i == -1 || j == -1)
                return "";
            //截取的位置就是第一个位置加上他的长度 到 第二个位置与第一个位置的差值
            return str.Substring(i + star.Length, j - i - star.Length);
        }

        /// <summary>
        /// 支持负数转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return str.Contains("-") ? -int.Parse(str.Replace("-", "")) : int.Parse(str);
        }

        /// <summary>
        ///  判断一个字符串是什么类型,数字,布尔,字符串等
        /// </summary>
        public static StringValueType GetStringType(string str)
        {
            //bool
            if (bool.TryParse(str, out var boolValue))
            {
                return StringValueType.Bool;
            }

            //int
            if (int.TryParse(str, out var intValue))
            {
                return StringValueType.Int;
            }

            //float
            if (float.TryParse(str, out var floatValue))
            {
                return StringValueType.Float;
            }

            return StringValueType.String;
        }

        public enum StringValueType
        {
            Bool,
            Int,
            Float,
            String
        }

        /// <summary>
        /// 字符串相加优化用于大循环
        /// </summary>
        /// <param name="str"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static string StrBuild(this string str, Action<StringBuilder> callBack)
        {
            StringBuilder sb = new StringBuilder(str);
            callBack?.Invoke(sb);
            return sb.ToString();
        }

        #region Format

        public static string StringFormat(this string str, object arg0)
        {
            return string.Format(str, arg0);
        }

        public static string StringFormat(this string str, object arg0, object arg1)
        {
            return string.Format(str, arg0, arg1);
        }

        public static string StringFormat(this string str, object arg0, object arg1, object arg2)
        {
            return string.Format(str, arg0, arg1, arg2);
        }

        public static string StringFormat(this string str, object arg0, object arg1, object arg2, object arg3)
        {
            return string.Format(str, arg0, arg1, arg2, arg3);
        }

        public static string StringFormat(this string str, object arg0, object arg1, object arg2, object arg3,
            object arg4)
        {
            return string.Format(str, arg0, arg1, arg2, arg3, arg4);
        }

        public static string StringFormat(this string str, object arg0, object arg1, object arg2, object arg3,
            object arg4,
            object arg5)
        {
            return string.Format(str, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static string StringFormat(this string str, object arg0, object arg1, object arg2, object arg3,
            object arg4,
            object arg5, object arg6)
        {
            return string.Format(str, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string StringFormat(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        #endregion
    }
}