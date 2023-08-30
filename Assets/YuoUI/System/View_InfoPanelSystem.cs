using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;
namespace YuoTools.UI
{
	public partial class View_InfoPanelComponent
	{
		public void ShowInfo(IDataView dataView)
		{
			var data = dataView.GetData();
		}
	}
	public class View_InfoPanelCreateSystem :YuoSystem<View_InfoPanelComponent>, IUICreate
	{
		public override string Group =>"UI/InfoPanel";

		protected override void Run(View_InfoPanelComponent view)
		{
			view.FindAll();
			//关闭窗口的事件注册,名字不同请自行更
			view.Button_Close.SetUICloseWaitAnima(view.ViewName);
 			view.Button_Mask.SetUICloseWaitAnima(view.ViewName);
		}
	}
	public class View_InfoPanelOpenSystem :YuoSystem<View_InfoPanelComponent,UIAnimaComponent>, IUIOpen
	{
		public override string Group =>"UI/InfoPanel";

		protected override void Run(View_InfoPanelComponent view, UIAnimaComponent anima)
		{
			view.Button_Mask.image.SetColorA(0);
			view.Button_Mask.image.DOFade(0.6f, 0.2f);
		}
	}
	public class View_InfoPanelCloseSystem :YuoSystem<View_InfoPanelComponent,UIAnimaComponent>, IUIClose
	{
		public override string Group =>"UI/InfoPanel";

		protected override void Run(View_InfoPanelComponent view, UIAnimaComponent anima)
		{
			view.Button_Mask.image.DOFade(0f, 0.2f);
		}
	}
}
