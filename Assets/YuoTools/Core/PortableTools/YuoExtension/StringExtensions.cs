using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YuoTools
{
    public static class StringExtension
    {
        /// <summary>
        /// 是否为身份证并且是18岁以上
        /// </summary>
        /// <param idNumber="身份证号码"></param>
        /// <returns></returns>
        public static string CheckIDCard18(this string idNumber)
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
        public static string RemovePrefixString(this string self, string str)
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
        public static string RemoveSuffixString(this string self, string str)
        {
            string strRegex = @"(" + str + ")" + "$";
            return Regex.Replace(self, strRegex, "");
        }

        /// <summary>
        /// 是否为Email
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEmail(this string self)
        {
            return self.RegexMatch(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        /// <summary>
        /// 是否为域名
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsDomain(this string self)
        {
            return self.RegexMatch(@"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?");
        }

        /// <summary>
        /// 是否为IP地址
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsIP(this string self)
        {
            return self.RegexMatch(@"((?:(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d))");
        }

        /// <summary>
        /// 是否为手机号码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(this string self)
        {
            return self.RegexMatch(@"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
        }

        /// <summary>
        /// 是否为中文
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsChinese(this string self)
        {
            return self.RegexMatch("[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        /// <param name="slef"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool RegexMatch(this string slef, string pattern)
        {
            Regex reg = new Regex(pattern);
            return reg.Match(slef).Success;
        }

        /// <summary>
        /// 转换为MD5, 加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string ConvertToMD5(this string self, string flag = "x2")
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
        public static string ConvertToMD5_32(this string self)
        {
            return ConvertToMD5(self, "x2");
        }

        /// <summary>
        /// 转换为48位MD5
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ConvertToMD5_48(this string self)
        {
            return ConvertToMD5(self, "x3");
        }

        /// <summary>
        /// 转换为64位MD5
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ConvertToMD5_64(this string self)
        {
            return ConvertToMD5(self, "x4");
        }

        /// <summary>
        /// 将十六位颜色字符串转为Color
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        public static Color Str16ToColor(this string colorName)
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

        public static bool IsInt(this string str)
        {
            return Regex.IsMatch(str, @"^[+-]?\d*$");
        }

        public static bool IsFloat(this string str)
        {
            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
        }

        public static bool IsDouble(this string str)
        {
            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
        }


        public static int SeedToInt(this string seed)
        {
            Temp.Int = int.MaxValue / 10 * (int)Mathf.PerlinNoise(seed.Length, seed.Length);
            foreach (var item in seed)
            {
                Temp.Int += item;
                Temp.Int %= (int.MaxValue / 2);
            }

            return Temp.Int;
        }

        public static decimal ToDecimal(this string value)
        {
            return decimal.Parse(value);
        }

        public static decimal ToDecimal(this string value, decimal defaultValue)
        {
            var result = defaultValue;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }

        public static decimal ToRoundDecimal(this string value, decimal defaultValue, int decimals)
        {
            var result = defaultValue;
            result = System.Math.Round(decimal.TryParse(value, out result) ? result : defaultValue, decimals);
            return result;
        }

        public static decimal? ToNullableDecimal(this string value)
        {
            decimal result;
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        ///  是否为null或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrSpace(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int index = 0; index < str.Length; ++index)
                {
                    if (!char.IsWhiteSpace(str[index]))
                        return false;
                }
            }

            return true;
        }
    }
}