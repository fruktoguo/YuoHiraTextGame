using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YuoTools.Extend.UI
{
    public class UISetting : MonoBehaviour
    {
        [HideInInspector] [HorizontalGroup] [Header("默认显示状态")]
        public bool Active = true;

        [HorizontalGroup("2")] [LabelText("模块UI")]
        public bool ModuleUI = false;

#if UNITY_EDITOR

        [HorizontalGroup("2")]
        [Button("生成UI代码")]
        public void SpawnCode()
        {
            SpawnUICode.SpawnCode(gameObject);
        }

        [HorizontalGroup("2")]
        [Button("选中System脚本")]
        public void OpenSystemScript()
        {
            var result = AssetDatabase.FindAssets($"View_{gameObject.name}System");
            if (result.Length > 0)
            {
                Selection.activeObject =
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0]));
            }
        }
#endif

        public async void Init()
        {
            var o = gameObject;
            Active = o.activeSelf;
            if (Active)
                await UIManagerComponent.Get.Open(o.name, o);
            else
            {
                var uiComponent = await UIManagerComponent.Get.AddWindow(o.name, gameObject);
            }
        }

        private Animator animator;

        public Animator Animator
        {
            get
            {
                if (!animator) animator = GetComponent<Animator>();
                return animator;
            }
        }

        [ShowInInspector] public bool OpenTools { get; set; } = false;


        [ShowIf("OpenTools", true)] [FoldoutGroup("Raycast")]
        public List<MaskableGraphic> maskableGraphics = new List<MaskableGraphic>();

        [ShowIf("OpenTools", true)]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "获取所有开启了Raycast的物体")]
        public void FindAllRaycast()
        {
            maskableGraphics.Clear();
            foreach (var item in transform.GetComponentsInChildren<MaskableGraphic>())
            {
                if (item.raycastTarget)
                {
                    maskableGraphics.Add(item);
                }
            }
        }

        [ShowIf("OpenTools", true)]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "清除剩余Raycast")]
        public void CloseRaycast()
        {
            foreach (var item in maskableGraphics)
            {
                item.raycastTarget = false;
            }
        }

        public enum UISate
        {
            Hide = 0,
            Show = 1,
            ShowAnima = 2,
            HideAnima = 3,
        }
    }
}