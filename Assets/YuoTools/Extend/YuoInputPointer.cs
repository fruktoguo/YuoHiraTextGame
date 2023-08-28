using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    public class YuoInputPointerSystem : YuoSystem<YuoInputPointerComponent>, IUpdate
    {
        public override string Group => SystemGroupConst.Input;
        protected override void Run(YuoInputPointerComponent component)
        {
            component.Update();
        }
    }

    [AutoAddToMain()]
    public partial class YuoInputPointerComponent : YuoComponentInstance<YuoInputPointerComponent>
    {
        readonly Dictionary<int, Touch> LastTouchs = new Dictionary<int, Touch>();
        [SerializeField] List<TouchItem> TouchItems = new List<TouchItem>();
    }

    public partial class YuoInputPointerComponent
    {
        public UnityEvent<TouchItem> OnTouchDown = new UnityEvent<TouchItem>();
        public UnityEvent<TouchItem> OnTouchUp = new UnityEvent<TouchItem>();
        public UnityEvent<TouchItem> OnTouchMove = new UnityEvent<TouchItem>();

        public void Update()
        {
#if UNITY_EDITOR || !UNITY_ANDROID

            if (Input.GetMouseButtonDown(0))
            {
                var item = GetTouch(0);
                item.touch = new Touch()
                {
                    position = Input.mousePosition,
                    fingerId = 0,
                    phase = TouchPhase.Began,
                };
                item.Down(item.touch);
                OnTouchDown?.Invoke(item);
            }
            else if (Input.GetMouseButton(0))
            {
                var item = GetTouch(0);
                item.touch = new Touch()
                {
                    position = Input.mousePosition,
                    fingerId = 0,
                    phase = TouchPhase.Moved,
                };
                item.Drag(item.touch);
                OnTouchMove?.Invoke(item);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var item = GetTouch(0);
                item.touch = new Touch()
                {
                    position = Input.mousePosition,
                    fingerId = 0,
                    phase = TouchPhase.Ended,
                };
                item.Up(item.touch);
                OnTouchUp?.Invoke(item);
            }
#else
            foreach (var touch in Input.touches)
            {
                var item = GetTouch(touch.fingerId);

                if (item.Dragging)
                {
                    item.touch = touch;
                    item.Drag(touch);
                    
                    OnTouchMove?.Invoke(item);
                }
                else
                {
                    item.touch = touch;
                    item.Down(touch);
                    
                    OnTouchDown?.Invoke(item);
                    
                    item.Dragging = true;
                }

                if (LastTouchs.ContainsKey(touch.fingerId))
                {
                    LastTouchs.Remove(touch.fingerId);
                }
            }

            foreach (var touch in LastTouchs.Values)
            {
                var item = GetTouch(touch.fingerId);

                if (item.Dragging)
                {
                    item.touch = touch;

                    item.Up(touch);
                    
                    OnTouchUp?.Invoke(item);
                    
                    item.Dragging = false;
                }
            }

            LastTouchs.Clear();

            foreach (var touch in Input.touches)
            {
                LastTouchs.Add(touch.fingerId, touch);
            }
#endif
        }

        public TouchItem GetTouch(int id)
        {
            if (TouchItems.Count <= id)
            {
                for (int i = TouchItems.Count; i <= id; i++)
                {
                    TouchItems.Add(new TouchItem()
                    {
                        //Pressed =x=> Log($"{x.fingerId} down {x.position}"),
                        //Released =x=> Log($"{x.fingerId} up {x.position}"),
                        //Dragged =x=> Log($"{x.fingerId} dragged {x.position}")
                    });
                }
            }

            return TouchItems[id];
        }

        public class TouchItem
        {
            public Touch touch;

            readonly Dictionary<string, Action<Touch>> _onDown = new Dictionary<string, Action<Touch>>();
            readonly Dictionary<string, Action<Touch>> _onUp = new Dictionary<string, Action<Touch>>();
            readonly Dictionary<string, Action<Touch>> _onDrag = new Dictionary<string, Action<Touch>>();

            public void Down(Touch _touch)
            {
                foreach (var action in _onDown.Values)
                {
                    action?.Invoke(_touch);
                }
            }

            public void Up(Touch _touch)
            {
                foreach (var action in _onUp.Values)
                {
                    action?.Invoke(_touch);
                }
            }

            public void Drag(Touch _touch)
            {
                foreach (var action in _onDrag.Values)
                {
                    action?.Invoke(_touch);
                }
            }

            public void AddOnDown(string key, Action<Touch> action)
            {
                if (!_onDown.ContainsKey(key))
                {
                    _onDown.Add(key, action);
                }
                else
                {
                    _onDown[key] = action;
                }
            }

            public void AddOnUp(string key, Action<Touch> action)
            {
                if (!_onUp.ContainsKey(key))
                {
                    _onUp.Add(key, action);
                }
                else
                {
                    _onUp[key] = action;
                }
            }

            public void AddOnDrag(string key, Action<Touch> action)
            {
                if (!_onDrag.ContainsKey(key))
                {
                    _onDrag.Add(key, action);
                }
                else
                {
                    _onDrag[key] = action;
                }
            }

            public void RemoveOnDown(string key)
            {
                if (_onDown.ContainsKey(key))
                {
                    _onDown.Remove(key);
                }
            }

            public void RemoveOnUp(string key)
            {
                if (_onUp.ContainsKey(key))
                {
                    _onUp.Remove(key);
                }
            }

            public void RemoveOnDrag(string key)
            {
                if (_onDrag.ContainsKey(key))
                {
                    _onDrag.Remove(key);
                }
            }

            public bool Dragging;
        }
    }
}