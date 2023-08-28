using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using YuoTools;

public static class Shielding
{
    private static Dictionary<string, bool> shiled = new Dictionary<string, bool>();

    public static bool GetShilding(string str)
    {
        if (!shiled.ContainsKey(str))
        {
            shiled.Add(str, true);
        }
        return shiled[str];
    }

    public static async void Shield(string str, float time)
    {
        if (!shiled.ContainsKey(str))
        {
            shiled.Add(str, true);
        }
        shiled[str] = false;
        await YuoWait.WaitTimeAsync(time);
        shiled[str] = true;
    }
}