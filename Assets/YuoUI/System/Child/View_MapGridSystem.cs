using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Extend.MathFunction;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_MapGridComponent : IDataView
    {
        [ShowInInspector] public MapGridData GridData { get; private set; }

        public void SetGridData(string name, YuoFloat2 position, float size)
        {
            GridData = AddComponent<MapGridData>();
            GridData.Name = name;
            GridData.Position = position;
            GridData.Size = size;             
            TextMeshProUGUI_GridText.text = name;
            rectTransform.sizeDelta = Vector2.one * size * 100;
            rectTransform.anchoredPosition = position;
            Button_BG.SetUIOpen(ViewType.InfoPanel);
            Button_BG.SetBtnClick(() => View_InfoPanelComponent.GetView().ShowInfo(this));
        }

        public List<string> GetData()
        {
            var data = new List<string>();
            data.AddRange(new List<string>()
            {
                $"名字:{GridData.Name}",
                $"大小:{GridData.Size * 100:F0}",
                $"位置:({GridData.Position.x:F2},{GridData.Position.y:F2})",
            });

            return data;
        }
    }

    public interface IDataView
    {
        public List<string> GetData();
    }

    public class MapGridData : YuoComponent
    {
        public string Name = "未知地点";
        public YuoFloat2 Position;
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