using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using YuoTools.Extend.Helper;
using YuoTools.Extend.YuoMathf;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_GameSceneComponent
    {
        public List<View_MapGridComponent> MapGridComponents = new();

        public void AddMapGridComponent(string name, YuoVector2 position, float size)
        {
            var mapItem = AddChildAndInstantiate(Child_MapGrid);
            mapItem.Entity.EntityName = name;
            mapItem.rectTransform.gameObject.name = name;
            mapItem.SetGridData(name, position, size);
        }
    }

    public class View_GameSceneCreateSystem : YuoSystem<View_GameSceneComponent>, IUICreate
    {
        public override string Group => "UI/GameScene";

        protected override void Run(View_GameSceneComponent view)
        {
            view.FindAll();
            for (int i = 0; i < 10; i++)
            {
                view.AddMapGridComponent($"测试场景{i}", Random.insideUnitCircle * 1000, Random.Range(1f, 5f));
            }
        }
    }
}