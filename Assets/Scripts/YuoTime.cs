using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools;

public class YuoTime
{
    public double Time { get; private set; }

    public long Year => (long)(Time / 360);
    public long Month => (long)(Time % 360 / 30);
    public long Day => (long)(Time % 360 % 12);
    public long Hour => (long)(Time % 1 * 24);
    public long Minute => (long)(Time % 1 * 24 % 1 * 60);

    public void AddTime(double time)
    {
        Time += time;
    }

    public void AddYear(long year)
    {
        Time += 360;
    }

    public void AddMonth(long month)
    {
        Time += 30;
    }

    public void AddDay(long day)
    {
        Time++;
    }

    public void AddHours(long hours)
    {
        Time += 1d / 24;
    }

    public void AddMinutes(long minutes)
    {
        Time += 1d / 24 / 60;
    }

    public override string ToString()
    {
        return $"{Year}年/{Month}月/{Day}日/{Hour}时/{Minute}分";
    }
}