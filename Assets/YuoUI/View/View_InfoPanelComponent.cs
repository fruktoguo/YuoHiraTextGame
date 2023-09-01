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
		public const string InfoPanel = "InfoPanel";
	}
	public partial class View_InfoPanelComponent : UIComponent 
	{

		public static View_InfoPanelComponent GetView() => UIManagerComponent.Get.GetUIView<View_InfoPanelComponent>();


		private Button mButton_Mask;

		public Button Button_Mask
		{
			get
			{
				if (mButton_Mask == null)
					mButton_Mask = rectTransform.Find("C_Mask").GetComponent<Button>();
				return mButton_Mask;
			}
		}


		private Button mButton_Close;

		public Button Button_Close
		{
			get
			{
				if (mButton_Close == null)
					mButton_Close = rectTransform.Find("Item/C_Close").GetComponent<Button>();
				return mButton_Close;
			}
		}


		private View_InfoItemComponent mChild_InfoItem;

		public View_InfoItemComponent Child_InfoItem
		{
			get
			{
				if (mChild_InfoItem == null)
				{
					mChild_InfoItem = Entity.AddChild<View_InfoItemComponent>();
					mChild_InfoItem.Entity.EntityName = "InfoItem";
					mChild_InfoItem.rectTransform = rectTransform.Find("Item/BackGround/D_InfoItem") as RectTransform;
					mChild_InfoItem.RunSystem<IUICreate>();
				}
				return mChild_InfoItem;
			}
		}


		[FoldoutGroup("ALL")]

		public List<Button> all_Button = new();

		[FoldoutGroup("ALL")]

		public List<View_InfoItemComponent> all_View_InfoItemComponent = new();

		public void FindAll()
		{
				
			all_Button.Add(Button_Mask);
			all_Button.Add(Button_Close);;
				
			all_View_InfoItemComponent.Add(Child_InfoItem);;


		}
	}}
