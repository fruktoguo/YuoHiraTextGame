using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;
namespace YuoTools.UI
{
	public class View_NetworkCreateSystem :YuoSystem<View_NetworkComponent>, IUICreate
	{
		public override string Group =>"UI/Network";

		protected override void Run(View_NetworkComponent view)
		{
			view.FindAll();
			//关闭窗口的事件注册,名字不同请自行更
			view.Button_Close.SetUIClose(view.ViewName);
 			view.Button_Mask.SetUIClose(view.ViewName);
		}
	}
	public class View_NetworkOpenSystem :YuoSystem<View_NetworkComponent,UIAnimaComponent>, IUIOpen
	{
		public override string Group =>"UI/Network";

		protected override void Run(View_NetworkComponent view, UIAnimaComponent anima)
		{
			view.Button_Mask.image.SetColorA(0);

			view.Button_Mask.image.DOFade(0.6f, anima.AnimaDuration);
		}
	}
	public class View_NetworkCloseSystem :YuoSystem<View_NetworkComponent,UIAnimaComponent>, IUIClose
	{
		public override string Group =>"UI/Network";

		protected override void Run(View_NetworkComponent view, UIAnimaComponent anima)
		{
			view.Button_Mask.image.DOFade(0f, anima.AnimaDuration);
		}
	}
}
