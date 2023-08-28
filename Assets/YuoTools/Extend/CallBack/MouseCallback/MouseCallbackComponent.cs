using System;
using UnityEngine;
using YuoTools.Extend.MouseCallback;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.MouseCallback
{
    /// <summary>
    ///  鼠标回调 
    ///  如果不是UI,则Camera需要挂载PhysicsRaycaster或者Physics2DRaycaster,且物体需要有Collider或者Collider2D
    /// <returns></returns>
    /// </summary>
    public class MouseCallbackComponent : YuoComponent
    {
        public Transform transform;
        public MouseCallback mouseCallback;
    }

    public class MouseCallbackComponentStartSystem : YuoSystem<MouseCallbackComponent>, IStart
    {
        public override string Group => SystemGroupConst.CallBack;
        protected override void Run(MouseCallbackComponent component)
        {
            //检查是否有鼠标回调组件
            if (component.transform.GetComponent<MouseCallback>() == null)
            {
                component.mouseCallback = component.transform.gameObject.AddComponent<MouseCallback>();
                component.mouseCallback.callbackComponent = component;
            }
        }
    }

    public class MouseCallbackComponentDestroySystem : YuoSystem<MouseCallbackComponent>, IDestroy
    {
        public override string Group => SystemGroupConst.CallBack;
        protected override void Run(MouseCallbackComponent component)
        {
            component.mouseCallback.TryDestroy();
        }
    }

    public interface IMouseEnter : ISystemTag
    {
    }

    public interface IMouseExit : ISystemTag
    {
    }

    public interface IMouseDown : ISystemTag
    {
    }

    public interface IMouseUp : ISystemTag
    {
    }

    public interface IMouseClick : ISystemTag
    {
    }

    // 没法用
    public interface IMouseRightClick : ISystemTag
    {
    }

    public interface IMouseRightDown : ISystemTag
    {
    }

    public interface IMouseRightUp : ISystemTag
    {
    }
}

public partial class SystemTagType
{
    public static readonly Type MouseEnter = typeof(IMouseEnter);
    public static readonly Type MouseExit = typeof(IMouseExit);
    public static readonly Type MouseDown = typeof(IMouseDown);
    public static readonly Type MouseUp = typeof(IMouseUp);
    public static readonly Type MouseClick = typeof(IMouseClick);
    public static readonly Type MouseRightClick = typeof(IMouseRightClick);
    public static readonly Type MouseRightDown = typeof(IMouseRightDown);
    public static readonly Type MouseRightUp = typeof(IMouseRightUp);
}
