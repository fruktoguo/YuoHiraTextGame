using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Burst;

[BurstCompile]
public struct YuoBigNum
{
    private const int Base = 1000000000; // 基数，用于分割大数
    private const int Width = 9; // 基数的位数
    private List<long> data; // 存储大数的数组
    private int dotIndex; // 小数点的位置
    private string num;

    private bool isNegative;

    public YuoBigNum(string num)
    {
        isNegative = false;
        if (num.StartsWith("-"))
        {
            isNegative = true;
            num = num.Substring(1);
        }

        data = new List<long>();
        dotIndex = num.IndexOf('.');
        if (dotIndex != -1)
        {
            // 从字符串的末尾移除所有的 '0'
            while (num.EndsWith('0'))
            {
                num = num.Remove(num.Length - 1);
            }

            num = num.Replace(".", "");
            dotIndex = num.Length - dotIndex;
        }
        else
        {
            dotIndex = 0;
        }

        this.num = num;

        for (int i = num.Length; i > 0; i -= Width)
        {
            if (i < Width)
            {
                data.Add(long.Parse(num.Substring(0, i)));
            }
            else
            {
                data.Add(long.Parse(num.Substring(i - Width, Width)));
            }
        }
    }

    public static YuoBigNum operator +(YuoBigNum a, YuoBigNum b)
    {
        // 如果 a 和 b 都是负数
        if (a.isNegative && b.isNegative)
        {
            a.isNegative = false;
            b.isNegative = false;
            YuoBigNum result = a + b;
            result.isNegative = true;
            return result;
        }
        // 如果 a 是负数，b 是正数

        if (a.isNegative)
        {
            a.isNegative = false;
            if (a > b)
            {
                YuoBigNum result = a - b;
                result.isNegative = true;
                return result;
            }
            else
            {
                return b - a;
            }
        }
        // 如果 a 是正数，b 是负数

        if (b.isNegative)
        {
            b.isNegative = false;
            if (b > a)
            {
                YuoBigNum result = b - a;
                result.isNegative = true;
                return result;
            }

            return a - b;
        }

        // 确保 a 的小数点位置在 b 的左边
        if (a.dotIndex < b.dotIndex)
        {
            (a, b) = (b, a);
        }

        YuoBigNum r = new YuoBigNum("0")
        {
            data = new List<long>(new long[Math.Max(a.data.Count, b.data.Count) + 1]),
            dotIndex = a.dotIndex
        };

        int carry = 0;
        for (int i = 0; i < a.data.Count; i++)
        {
            carry += (int)a.data[i];
            if (i < b.data.Count)
                carry += (int)b.data[i];
            r.data[i] = carry % Base;
            carry /= Base;
        }

        if (carry != 0)
            r.data[a.data.Count] = carry;
        return r;
    }

    public static YuoBigNum operator -(YuoBigNum a, YuoBigNum b)
    {
        // 如果 a 和 b 都是负数
        if (a.isNegative && b.isNegative)
        {
            a.isNegative = false;
            b.isNegative = false;
            return b - a;
        }
        // 如果 a 是负数，b 是正数

        if (a.isNegative)
        {
            a.isNegative = false;
            YuoBigNum r = a + b;
            r.isNegative = true;
            return r;
        }
        // 如果 a 是正数，b 是负数

        if (b.isNegative)
        {
            b.isNegative = false;
            return a + b;
        }
        // 如果 a 小于 b

        if (a < b)
        {
            YuoBigNum r = b - a;
            r.isNegative = true;
            return r;
        }

        // 确保 a 的小数点位置在 b 的左边
        if (a.dotIndex < b.dotIndex)
        {
            (a, b) = (b, a);
        }

        YuoBigNum result = new YuoBigNum("0")
        {
            data = new List<long>(new long[a.data.Count]),
            dotIndex = a.dotIndex
        };

        int borrow = 0;
        for (int i = 0; i < a.data.Count; i++)
        {
            result.data[i] = a.data[i] - borrow;
            if (i < b.data.Count)
                result.data[i] -= b.data[i];
            if (result.data[i] < 0)
            {
                result.data[i] += Base;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }
        }

        return result;
    }

    public static YuoBigNum operator *(YuoBigNum a, YuoBigNum b)
    {
        YuoBigNum result = new YuoBigNum("0")
        {
            data = new List<long>(new long[a.data.Count + b.data.Count]),
            dotIndex = a.dotIndex + b.dotIndex
        };

        for (int i = 0; i < a.data.Count; i++)
        {
            for (int j = 0; j < b.data.Count; j++)
            {
                result.data[i + j] += a.data[i] * b.data[j];
                if (result.data[i + j] >= Base)
                {
                    result.data[i + j + 1] += result.data[i + j] / Base;
                    result.data[i + j] %= Base;
                }
            }
        }

        result.isNegative = a.isNegative != b.isNegative;

        return result;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (long i in data.Reverse<long>())
        {
            sb.Append(i.ToString().PadLeft(Width, '0'));
        }

        string result = sb.ToString().TrimStart('0');
        if (dotIndex < result.Length)
        {
            result = result.Insert(result.Length - dotIndex, ".");
        }

        if (isNegative)
        {
            result = "-" + result;
        }

        return result;
    }

    #region 运算符重载

    public static bool operator ==(YuoBigNum a, YuoBigNum b)
    {
        return a.isNegative == b.isNegative && a.ToString() == b.ToString();
    }

    public static bool operator !=(YuoBigNum a, YuoBigNum b)
    {
        return a.isNegative != b.isNegative || a.ToString() != b.ToString();
    }

    public static implicit operator string(YuoBigNum a)
    {
        return a.ToString();
    }

    public static implicit operator YuoBigNum(string a)
    {
        return new YuoBigNum(a);
    }

    public static bool operator >(YuoBigNum a, YuoBigNum b)
    {
        // 如果 a 是正数，b 是负数
        if (!a.isNegative && b.isNegative)
        {
            return true;
        }
        // 如果 a 是负数，b 是正数
        else if (a.isNegative && !b.isNegative)
        {
            return false;
        }

        // 如果 a 和 b 都是正数
        if (!a.isNegative)
        {
            // 比较小数点位置
            if (a.dotIndex != b.dotIndex)
            {
                return a.dotIndex > b.dotIndex;
            }

            // 比较长度
            if (a.data.Count != b.data.Count)
            {
                return a.data.Count > b.data.Count;
            }

            // 比较每一位
            for (int i = a.data.Count - 1; i >= 0; i--)
            {
                if (a.data[i] != b.data[i])
                {
                    return a.data[i] > b.data[i];
                }
            }
        }
        // 如果 a 和 b 都是负数
        else
        {
            // 比较小数点位置
            if (a.dotIndex != b.dotIndex)
            {
                return a.dotIndex < b.dotIndex;
            }

            // 比较长度
            if (a.data.Count != b.data.Count)
            {
                return a.data.Count < b.data.Count;
            }

            // 比较每一位
            for (int i = a.data.Count - 1; i >= 0; i--)
            {
                if (a.data[i] != b.data[i])
                {
                    return a.data[i] < b.data[i];
                }
            }
        }

        // 如果 a 和 b 完全相同
        return false;
    }

    public static bool operator <(YuoBigNum a, YuoBigNum b)
    {
        return !(a > b) && a != b;
    }

    public bool Equals(YuoBigNum other)
    {
        return Equals(data, other.data) && dotIndex == other.dotIndex && num == other.num;
    }

    public override bool Equals(object obj)
    {
        return obj is YuoBigNum other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(data, dotIndex, num);
    }

    #endregion
}