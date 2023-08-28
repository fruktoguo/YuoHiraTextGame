using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class YuoUIDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        private static YuoUIDrag _current;
        [HideInInspector]
        public MaskableGraphic graphic;
        public UnityEvent<YuoUIDrag> onDragEnd;
        public UnityEvent onDragStart;

        private void Awake()
        {
            graphic = GetComponent<MaskableGraphic>();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //print($"拖拽结束：{gameObject.name},拖拽到了 {(Current != null ? Current.name : null)}");
            if(YuoUIDragManager.Get.DragItem == this) YuoUIDragManager.Get.DragItem = null;
            onDragEnd?.Invoke(_current);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDragStart?.Invoke();
            YuoUIDragManager.Get.DragItem = this;
            print($"开始拖拽 {gameObject.name}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _current = this;
            //print(gameObject.name + " OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_current == this) _current = null;
        }
    }


}