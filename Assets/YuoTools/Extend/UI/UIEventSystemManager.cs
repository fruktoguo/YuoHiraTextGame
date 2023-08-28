using ET;
using UnityEngine;
using UnityEngine.EventSystems;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.UI
{
    [AutoAddToMain()]
    public class UIEventSystemManager : YuoComponentGet<UIEventSystemManager>
    {
        private EventSystem _eventSystem;

        public EventSystem EventSystem
        {
            get
            {
                if (_eventSystem == null)
                {
                    _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                }

                return _eventSystem;
            }
        }

        public async void Shield(float time)
        {
            EventSystem.enabled = false;
            await YuoWait.WaitUnscaledTimeAsync(time);
            EventSystem.enabled = true;
        }

        public async void Shield(ETTask tcs)
        {
            EventSystem.enabled = false;
            await tcs;
            EventSystem.enabled = true;
        }
    }
}