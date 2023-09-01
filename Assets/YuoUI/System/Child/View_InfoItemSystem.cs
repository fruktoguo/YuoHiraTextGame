using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;
namespace YuoTools.UI
{
	public class View_InfoItemCreateSystem :YuoSystem<View_InfoItemComponent>, IUICreate
	{
		public override string Group =>"UI/InfoItem";

		protected override void Run(View_InfoItemComponent view)
		{
			view.FindAll();
		}
	}
}
