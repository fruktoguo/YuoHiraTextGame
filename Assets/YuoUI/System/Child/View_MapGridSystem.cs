﻿using DG.Tweening;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Extend.YuoMathf;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_MapGridComponent
    {
        public MapGridData GridData { get; private set; }

        public void SetGridData(string name, YuoVector2 position, float size)
        {
            GridData = new MapGridData { Name = name, Position = position, Size = size };
            TextMeshProUGUI_GridText.text = name;
            rectTransform.sizeDelta = Vector2.one * size * 100;
            rectTransform.anchoredPosition = position;
        }
    }

    public class MapGridData
    {
        public string Name = "未知地点";
        public YuoVector2 Position;
        public float Size = 1;
    }

    public class View_MapGridCreateSystem : YuoSystem<View_MapGridComponent>, IUICreate
    {
        public override string Group => "UI/MapGrid";

        protected override void Run(View_MapGridComponent view)
        {
            view.FindAll();
        }
    }
}