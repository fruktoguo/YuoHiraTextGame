using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuoTools;
using YuoTools.Extend.YuoMathf;
using YuoTools.Main.Ecs;

public class MapManager : YuoComponentInstance<MapManager>
{
}

public class TranComponent : YuoComponent
{
    public YuoVector2 Pos = new();
    public float PosZ = 0;
    public YuoVector2 Size = new();

    public Transform Transform;
    public GameObject GameObject;

    public bool InScreen(ScreenComponent screen)
    {
        return screen.InScreen(Pos, Size);
    }

    public void Move(YuoVector2 target)
    {
        Pos = target;
        Transform.SetPos(Pos.x, Pos.y, PosZ);
    }
}

public class ScreenComponent : YuoComponentInstance<ScreenComponent>
{
    public YuoVector2 ScreenSize = new(2560, 1440);
    public YuoVector2 ScreenPos = new();

    public bool InScreen(YuoVector2 pos, YuoVector2 size)
        => pos.x + size.x >= ScreenPos.x &&
           pos.x <= ScreenPos.x + ScreenSize.x &&
           pos.y + size.y >= ScreenPos.y &&
           pos.y <= ScreenPos.y + ScreenSize.y;
}