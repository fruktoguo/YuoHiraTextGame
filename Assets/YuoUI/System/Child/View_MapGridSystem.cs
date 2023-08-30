using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Extend.YuoMathf;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_MapGridComponent : IDataView
    {
        [ShowInInspector] public MapGridData GridData { get; private set; }

        public void SetGridData(string name, YuoVector2 position, float size)
        {
            GridData = new MapGridData { Name = name, Position = position, Size = size };
            TextMeshProUGUI_GridText.text = name;
            rectTransform.sizeDelta = Vector2.one * size * 100;
            rectTransform.anchoredPosition = position;
            Button_BG.SetUIOpen(ViewType.InfoPanel);
            View_InfoPanelComponent.GetView().ShowInfo(this);
        }

        public List<string> GetData()
        {
            var data = new List<string>();
            data.Add($"名字:{GridData.Name}");
            return data;
        }
    }

    public interface IDataView
    {
        public List<string> GetData();
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