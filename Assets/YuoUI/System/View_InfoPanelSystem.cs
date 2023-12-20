using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine.Pool;
using YuoTools.Extend.Helper;
using YuoTools.Extend.OtherTools;
using YuoTools.Extend.MathFunction;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_InfoPanelComponent
    {
        public void ShowInfo(IDataView dataView)
        {
            UIManagerComponent.Get.OpenSync(ViewName);
            var data = dataView.GetData();
            for (var i = 0; i < data.Count; i++)
            {
                var info = data[i];
                var view = Pool.Get();
                view.TextMeshProUGUI_Text.text = info;
                float width = view.TextMeshProUGUI_Text.rectTransform.GetSize().x;
                view.rectTransform.SetSizeX(width + 20);
            }
        }

        public YuoObjectPool<View_InfoItemComponent> Pool;
    }

    public class View_InfoPanelCreateSystem : YuoSystem<View_InfoPanelComponent>, IUICreate
    {
        public override string Group => "UI/InfoPanel";

        protected override void Run(View_InfoPanelComponent view)
        {
            view.FindAll();
            view.Pool = new YuoObjectPool<View_InfoItemComponent>(
                () => view.AddChildAndInstantiate(view.Child_InfoItem),
                x =>
                {
                    x.rectTransform.Show();
                    x.rectTransform.SetAsLastSibling();
                },
                x => x.rectTransform.Hide());
            //关闭窗口的事件注册,名字不同请自行更
            view.Button_Close.SetUICloseWaitAnima(view.ViewName);
            view.Button_Mask.SetUICloseWaitAnima(view.ViewName);
        }
    }

    public class View_InfoPanelOpenSystem : YuoSystem<View_InfoPanelComponent, UIAnimaComponent>, IUIOpen
    {
        public override string Group => "UI/InfoPanel";

        protected override void Run(View_InfoPanelComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.SetColorA(0);
            view.Button_Mask.image.DOFade(0.6f, anima.AnimaDuration);
        }
    }

    public class View_InfoPanelCloseSystem : YuoSystem<View_InfoPanelComponent, UIAnimaComponent>, IUIClose
    {
        public override string Group => "UI/InfoPanel";

        protected override void Run(View_InfoPanelComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.DOFade(0f, anima.AnimaDuration);
        }
    }

    public class View_InfoPanelAnimaCloseSystem : YuoSystem<View_InfoPanelComponent, UIAnimaComponent>, IUICloseAnima
    {
        public override string Group => "UI/InfoPanel";

        protected override void Run(View_InfoPanelComponent view, UIAnimaComponent anima)
        {
            view.Pool.ReleaseAll();
        }
    }
}