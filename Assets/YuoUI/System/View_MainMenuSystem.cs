using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public class View_MainMenuCreateSystem : YuoSystem<View_MainMenuComponent>, IUICreate
    {
        public override string Group => "UI/MainMenu";

        protected override void Run(View_MainMenuComponent view)
        {
            view.FindAll();
        }
    }
}