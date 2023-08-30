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
		public const string MainMenu = "MainMenu";
	}
	public partial class View_MainMenuComponent : UIComponent 
	{

		public static View_MainMenuComponent GetView() => UIManagerComponent.Get.GetUIView<View_MainMenuComponent>();


		private Button mButton_开始游戏;

		public Button Button_开始游戏
		{
			get
			{
				if (mButton_开始游戏 == null)
					mButton_开始游戏 = rectTransform.Find("选项/C_开始游戏").GetComponent<Button>();
				return mButton_开始游戏;
			}
		}


		private Button mButton_继续游戏;

		public Button Button_继续游戏
		{
			get
			{
				if (mButton_继续游戏 == null)
					mButton_继续游戏 = rectTransform.Find("选项/C_继续游戏").GetComponent<Button>();
				return mButton_继续游戏;
			}
		}


		private Button mButton_设置;

		public Button Button_设置
		{
			get
			{
				if (mButton_设置 == null)
					mButton_设置 = rectTransform.Find("选项/C_设置").GetComponent<Button>();
				return mButton_设置;
			}
		}


		private Button mButton_退出;

		public Button Button_退出
		{
			get
			{
				if (mButton_退出 == null)
					mButton_退出 = rectTransform.Find("选项/C_退出").GetComponent<Button>();
				return mButton_退出;
			}
		}



		[FoldoutGroup("ALL")]

		public List<Button> all_Button = new();

		public void FindAll()
		{
				
			all_Button.Add(Button_开始游戏);
			all_Button.Add(Button_继续游戏);
			all_Button.Add(Button_设置);
			all_Button.Add(Button_退出);;


		}
	}}
