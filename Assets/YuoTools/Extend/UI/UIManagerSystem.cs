using ET;
using UnityEngine;
using YuoTools.Extend;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class UIManagerComponent
    {
        public async ETTask<UIComponent> Open(string winName, GameObject go = null)
        {
            UIComponent component = GetUIView(winName) ?? await AddWindow(winName, go);

            if (component == null) return null;
            if (!openItems.Contains(component))
            {
                openItems.Add(component);
                component.AddComponent<UIActiveComponent>();
            }

            if (!component.ModuleUI) TopView = component;
            RunSystemAndChild<IUIOpen>(component);

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                anima.RunSystem<IUIOpen>();
                await anima.Open();
            }

            return component;
        }

        public static void ResetWindowLayer(UIComponent component)
        {
            if (component.ModuleUI)
            {
                if (!Get.moduleUiItems.Contains(component))
                {
                    Get.moduleUiItems.Add(component);
                    component.AddComponent<TopViewComponent>();
                }

                component.rectTransform.SetAsLastSibling();
            }
            else
            {
                component.SetWindowLayer(Get.Transform.childCount - 1 - Get.moduleUiItems.Count);
            }
        }

        public async void OpenSync(string winName, GameObject go = null) => await Open(winName, go);

        public async ETTask<T> Open<T>() where T : UIComponent
        {
            if (uiItemsType.ContainsKey(typeof(T)))
                return await Open(uiItemsType[typeof(T)].ViewName) as T;
            else
            {
                Debug.LogError($"UIManagerComponent Open<T> error,not find {typeof(T)}");
                return null;
            }
        }

        public async void Open<T>(T component) where T : UIComponent
        {
            if (!openItems.Contains(component))
            {
                openItems.Add(component);
                component.AddComponent<UIActiveComponent>();
            }
            else
            {
                return;
            }

            if (component.AutoShow) component.rectTransform.gameObject.Show();

            if (component.ModuleUI)
            {
                if (!moduleUiItems.Contains(component))
                {
                    moduleUiItems.Add(component);
                    component.AddComponent<TopViewComponent>();
                }

                component.rectTransform.SetAsLastSibling();
            }
            else
            {
                component.SetWindowLayer(Transform.childCount - 1 - moduleUiItems.Count);
            }

            if (component.rectTransform.gameObject.activeSelf)
            {
                if (!component.ModuleUI) TopView = component;
                RunSystemAndChild<IUIOpen>(component);
            }

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                YuoWorld.RunSystem<IUIOpen>(anima);
                await anima.Open();
            }
        }

        public async ETTask<UIComponent> AddWindow(string winName, GameObject go = null)
        {
            var type = YuoWorld.Instance.GetComponentType($"View_{winName}Component");
            if (type == null)
            {
                Debug.LogError($"找不到对应类型---View_{winName}Component");
                return null;
            }

            //生成窗口
            var component =
                YuoWorld.Main.GetComponent<UIManagerComponent>().Entity
                    .AddChild(type, IDGenerate.GetID(winName)) as UIComponent;
            if (component == null) return null;

            component.AddComponent<UIAutoExitComponent>();

            if (go == null) go = await Create(winName);

            //初始化窗口
            component.rectTransform = go.transform as RectTransform;

            //如果有动画组件就挂载动画组件
            if (go.TryGetComponent<UISetting>(out var uiSetting))
            {
                component.Entity.AddComponent<UIAnimaComponent>().From(uiSetting);
                component.ModuleUI = uiSetting.ModuleUI;
                go.SetActive(uiSetting.Active);
                Object.Destroy(uiSetting);
            }

            component.ViewName = winName;

            uiItems.Add(winName, component);
            uiItemsType.Add(type, component);

            if (BaseIndex == -1) BaseIndex = go.transform.GetSiblingIndex();

            //调用这个窗口的初始化系统
            component.Entity.EntityName = "View_" + component.ViewName;

            YuoWorld.RunSystem<IUICreate>(component.Entity);

            return component;
        }

        public void Close<T>() where T : UIComponent
        {
            if (uiItemsType.ContainsKey(typeof(T)))
                Close(uiItemsType[typeof(T)].ViewName);
            else
            {
                Debug.LogError($"UIManagerComponent Close<T> error,not find {typeof(T)}");
            }
        }

        public async void Close(string winName)
        {
            UIComponent component = GetUIView(winName);
            if (component == null)
            {
                $"找不到窗口{winName}".LogError();
                return;
            }

            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIClose>(component);
            if (component.ModuleUI && moduleUiItems.Contains(component)) moduleUiItems.Remove(component);
            if (openItems.Contains(component))
            {
                openItems.Remove(component);
                component.GetComponent<UIActiveComponent>().Destroy();
            }

            if (!component.ModuleUI) TopView = openItems.Count > 0 ? openItems[^1] : null;

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                await anima.Close();
                YuoWorld.RunSystem<IUICloseAnima>(anima);
            }

            if (component.AutoHide) component.rectTransform.gameObject.Hide();
        }

        public void CloseAll()
        {
            var openItemsCopy = openItems.ToArray();
            foreach (var item in openItemsCopy)
            {
                Close(item.ViewName);
            }
        }

        public void Remove(string winName)
        {
            var win = GetUIView(winName);
            if (win != null)
            {
                uiItems.Remove(winName);
                uiItemsType.Remove(win.Type);
                win.Entity.Dispose();
            }
        }

        public bool IsOpen(string winName)
        {
            return openItems.Contains(GetUIView(winName));
        }

        public UIComponent GetUIView(string winName)
        {
            return uiItems.TryGetValue(winName, out var item) ? item : null;
        }

        public T GetUIView<T>() where T : UIComponent
        {
            return uiItemsType.ContainsKey(typeof(T)) ? uiItemsType[typeof(T)] as T : null;
        }

        private async ETTask<GameObject> Create(string winName)
        {
            return (await YuoWorld.Main.GetBaseComponent<AssetsLoadComponent>().LoadPrefabAsync(LoadPath + winName))
                .Instantiate(Transform);
        }

        private UIComponent _topView;

        public UIComponent TopView
        {
            get => _topView;
            private set
            {
                if (_topView != value)
                {
                    if (value != null)
                    {
                        _topView?.GetComponent<TopViewComponent>().Destroy();

                        value.Entity.AddComponent<TopViewComponent>();

                        RunSystemAndChild<IUIActive>(value);
                    }

                    _topView = value;
                }
            }
        }

        public int WindowCount => uiItems.Count + BaseIndex;

        public static Vector2 CanvasSize;

        void RunSystemAndChild<T>(UIComponent component) where T : ISystemTag
        {
            component.Entity.RunSystem<T>();
            YuoWorld.RunSystem<T>(component.Entity.GetAllChildren());
        }
    }

    public partial class UIComponent
    {
        public void SetWindowLayer(int layer)
        {
            rectTransform.SetSiblingIndex(layer);
        }

        public T AddChildAndInstantiate<T>(T template) where T : UIComponent, new()
        {
            var go = Object.Instantiate(template.rectTransform, template.rectTransform.parent);
            var child = Entity.AddChild<T>();
            child.rectTransform = go.transform as RectTransform;
            child.RunSystem<IUICreate>();
            go.Show();
            return child;
        }
    }

    public class UIManagerAwakeSystem : YuoSystem<UIManagerComponent>, IAwake
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIManagerComponent component)
        {
            if (component.Transform == null)
            {
                component.Destroy();
                return;
            }

            var uiSettings = component.Transform.GetComponentsInChildren<UISetting>(true);
            foreach (var uiSetting in uiSettings)
            {
                uiSetting.Init();
            }
        }
    }

    public class UIItemOpenSystem : YuoSystemOfBase<UIComponent, TopViewComponent>, IUIOpen
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIComponent component, TopViewComponent topViewComponent)
        {
            // component.SetWindowLayer(YuoWorld.Main.GetComponent<UIManagerComponent>().WindowCount);
            UIManagerComponent.ResetWindowLayer(component);
            if (component.AutoShow) component.rectTransform.Show();
        }
    }

    public class UIItemSelectSystem : YuoSystemOfBase<UIComponent>, IStart, IUICreate
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIComponent baseComponent)
        {
            if (baseComponent.rectTransform != null)
            {
                baseComponent.AddComponent<EntitySelectComponent>().SelectGameObject =
                    baseComponent.rectTransform.gameObject;
            }
            else
            {
                Debug.LogError($"UIItemSelectSystem error,not find rectTransform {baseComponent.ViewName}");
            }
        }
    }

    public class UICanvasSizeResetSystem : YuoSystem<UIManagerComponent>, IAwake, IUIAdaption
    {
        protected override void Run(UIManagerComponent component)
        {
            // var canvas = component.Transform.GetComponent<CanvasScaler>();
            // ReflexHelper.LogAll(canvas);
        }
    }

    public class UIAutoExitComponent : YuoComponent
    {
        public override string Name => "场景切换时自动销毁";
    }

    /// <summary>
    /// 切换场景时清理UI
    /// </summary>
    public class UIAutoExitSystem : YuoSystem<UIAutoExitComponent>, ISceneExit
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIAutoExitComponent component)
        {
            component.Entity.Destroy();
        }
    }
}