using System;
using System.Collections.Generic;
using ET;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;
using YuoTools.UI;
using Object = UnityEngine.Object;

namespace YuoTools.UI
{
    public partial class UIComponent : YuoComponent
    {
        public RectTransform rectTransform;
        public string ViewName;
        [ReadOnly] [SerializeField] int Layer;

        [LabelText("模块UI")] [ReadOnly] [Tooltip("悬浮在其他界面之上的UI")]
        public bool ModuleUI = false;

        /// <summary>
        ///  在调用ShowView时是否直接显示gameObject
        /// </summary>
        [HideInInspector] public bool AutoShow = true;

        /// <summary>
        ///  在调用CloseView时是否直接隐藏gameObject
        /// </summary>
        [HideInInspector] public bool AutoHide = true;
    }

    public static class UIComponentEx
    {
        public static void CloseView(this UIComponent component)
        {
            UIManagerComponent.Get.Close(component.ViewName);
        }
    }

    [AutoAddToMain()]
    public partial class UIManagerComponent : YuoComponentInstance<UIManagerComponent>
    {
        [ReadOnly] [ShowInInspector] Dictionary<string, UIComponent> uiItems = new();
        [ReadOnly] [ShowInInspector] Dictionary<Type, UIComponent> uiItemsType = new();
        [ReadOnly] [ShowInInspector] List<UIComponent> moduleUiItems = new();

        public override string Name => "UI管理器";

        [ReadOnly] [SerializeField] List<UIComponent> openItems = new List<UIComponent>();

        public int BaseIndex = -1;

        public string LoadPath = "Prefabs/UI/";

        Transform _transform;

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GameObject.Find("Canvas")?.transform;
                }

                return _transform;
            }
        }
    }

    /// <summary>
    /// 处于顶部的UI
    /// </summary>
    public class TopViewComponent : YuoComponent
    {
        public override string Name => "当前UI处于顶部";
    }

    /// <summary>
    ///  处于激活状态的UI
    /// </summary>
    public class UIActiveComponent : YuoComponent
    {
        public override string Name => "当前UI处于激活状态";
    }

    #region 接口

    /// <summary>
    ///  当UI打开时调用一次
    /// </summary>
    public interface IUIOpen : ISystemTag
    {
    }

    public interface IUIOpenAnima : ISystemTag
    {
    }

    /// <summary>
    ///  当UI关闭时调用一次
    /// </summary>
    public interface IUIClose : ISystemTag
    {
    }

    public interface IUICloseAnima : ISystemTag
    {
    }

    /// <summary>
    /// 在UI创建时调用一次,在Awake之后
    /// </summary>
    public interface IUICreate : ISystemTag
    {
    }

    /// <summary>
    ///  当UI处于顶层时调用一次
    /// </summary>
    public interface IUIActive : ISystemTag
    {
    }

    /// <summary>
    ///  UI手动切换自适应时调用
    /// </summary>
    public interface IUIAdaption : ISystemTag
    {
    }

    #endregion
}

public static partial class SystemTagType
{
    public static readonly Type UIOpen = typeof(IUIOpen);
    public static readonly Type UIClose = typeof(IUIClose);
    public static readonly Type UICreate = typeof(IUICreate);
    public static readonly Type UIActive = typeof(IUIActive);
}