using UnityEngine;
using UnityEngine.EventSystems;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.MouseCallback
{
    public class MouseCallback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
        IPointerExitHandler, IPointerClickHandler
    {
        public MouseCallbackComponent callbackComponent;

        public PointerEventData EventData;

        public bool OpenDown = true;
        public bool OpenUp = true;
        public bool OpenEnter = true;
        public bool OpenExit = true;
        public bool OpenClick = true;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!OpenDown) return;
            if (callbackComponent.IsNull())
            {
                "callbackComponent is null".LogError();
                return;
            }

            EventData = eventData;
            YuoWorld.RunSystem(Input.GetMouseButtonDown(1) ? SystemTagType.MouseRightDown : SystemTagType.MouseDown,
                callbackComponent.Entity);
            EventData = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!OpenUp) return;
            if (callbackComponent.IsNull())
            {
                "callbackComponent is null".LogError();
                return;
            }

            EventData = eventData;
            YuoWorld.RunSystem(Input.GetMouseButtonDown(1) ? SystemTagType.MouseRightUp : SystemTagType.MouseUp,
                callbackComponent.Entity);
            EventData = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!OpenEnter) return;
            if (callbackComponent.IsNull())
            {
                "callbackComponent is null".LogError();
                return;
            }

            EventData = eventData;
            YuoWorld.RunSystem(SystemTagType.MouseEnter, callbackComponent.Entity);
            EventData = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!OpenExit) return;
            if (callbackComponent.IsNull())
            {
                "callbackComponent is null".LogError();
                return;
            }

            EventData = eventData;
            YuoWorld.RunSystem(SystemTagType.MouseExit, callbackComponent.Entity);
            EventData = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!OpenClick) return;
            if (callbackComponent.IsNull())
            {
                "callbackComponent is null".LogError();
                return;
            }

            EventData = eventData;
            YuoWorld.RunSystem(Input.GetMouseButtonUp(1) ? SystemTagType.MouseRightClick : SystemTagType.MouseClick,
                callbackComponent.Entity);
            EventData = null;
        }
    }
}