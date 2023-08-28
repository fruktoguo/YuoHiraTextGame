using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;

namespace YuoTools.UI
{

	public static partial class ViewType
	{
		public const string GameScene = "GameScene";
	}
	public partial class View_GameSceneComponent : UIComponent 
	{

		private View_MapGridComponent mChild_MapGrid;

		public View_MapGridComponent Child_MapGrid
		{
			get
			{
				if (mChild_MapGrid == null)
				{
					mChild_MapGrid = Entity.AddChild<View_MapGridComponent>();
					mChild_MapGrid.Entity.EntityName = "MapGrid";
					mChild_MapGrid.rectTransform = rectTransform.Find("D_MapGrid") as RectTransform;
					mChild_MapGrid.RunSystem<IUICreate>();
				}
				return mChild_MapGrid;
			}
		}


		[FoldoutGroup("ALL")]

		public List<View_MapGridComponent> all_View_MapGridComponent = new();

		public void FindAll()
		{
				
			all_View_MapGridComponent.Add(Child_MapGrid);;


		}
	}}
