using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_MapGridComponent : UIComponent 
	{

		private RectTransform mainRectTransform;

		public RectTransform MainRectTransform
		{
			get
			{
				if (mainRectTransform == null)
					mainRectTransform = rectTransform.GetComponent<RectTransform>();
				return mainRectTransform;
			}
		}

		private Button mButton_BG;

		public Button Button_BG
		{
			get
			{
				if (mButton_BG == null)
					mButton_BG = rectTransform.Find("C_BG").GetComponent<Button>();
				return mButton_BG;
			}
		}


		private TextMeshProUGUI mTextMeshProUGUI_GridText;

		public TextMeshProUGUI TextMeshProUGUI_GridText
		{
			get
			{
				if (mTextMeshProUGUI_GridText == null)
					mTextMeshProUGUI_GridText = rectTransform.Find("C_GridText").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_GridText;
			}
		}



		[FoldoutGroup("ALL")]

		public List<RectTransform> all_RectTransform = new();

		[FoldoutGroup("ALL")]

		public List<Button> all_Button = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_RectTransform.Add(MainRectTransform);;
				
			all_Button.Add(Button_BG);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_GridText);;


		}
	}}
