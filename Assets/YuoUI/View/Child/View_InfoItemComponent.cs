using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using Sirenix.OdinInspector;
using TMPro;

namespace YuoTools.UI
{

	public partial class View_InfoItemComponent : UIComponent 
	{

		private Image mainImage;

		public Image MainImage
		{
			get
			{
				if (mainImage == null)
					mainImage = rectTransform.GetComponent<Image>();
				return mainImage;
			}
		}

		private TextMeshProUGUI mTextMeshProUGUI_Text;

		public TextMeshProUGUI TextMeshProUGUI_Text
		{
			get
			{
				if (mTextMeshProUGUI_Text == null)
					mTextMeshProUGUI_Text = rectTransform.Find("C_Text").GetComponent<TextMeshProUGUI>();
				return mTextMeshProUGUI_Text;
			}
		}



		[FoldoutGroup("ALL")]

		public List<Image> all_Image = new();

		[FoldoutGroup("ALL")]

		public List<TextMeshProUGUI> all_TextMeshProUGUI = new();

		public void FindAll()
		{
				
			all_Image.Add(MainImage);;
				
			all_TextMeshProUGUI.Add(TextMeshProUGUI_Text);;


		}
	}}
