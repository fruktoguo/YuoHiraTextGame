using UnityEngine.Events;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.Extend.Helper
{
    public static class UIHelper
    {
        public static void SetBtnClick(this Button btn, UnityAction action)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }

        public static void AddUIClose(this Button btn, string windowName)
        {
            btn.onClick.AddListener(() => UIManagerComponent.Get.Close(windowName));
        }

        public static void SetUIClose(this Button btn, string windowName)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => UIManagerComponent.Get.Close(windowName));
        }

        public static void AddUIOpen(this Button btn, string windowName)
        {
            btn.onClick.AddListener(Call);

            async void Call() => await UIManagerComponent.Get.Open(windowName);
        }

        public static void SetUIOpen(this Button btn, string windowName)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Call);

            async void Call() => await UIManagerComponent.Get.Open(windowName);
        }
    }
}