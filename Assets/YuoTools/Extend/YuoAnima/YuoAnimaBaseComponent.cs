using UnityEngine.Events;
using YuoTools;
using YuoTools.Main.Ecs;

namespace YuoAnima
{
    public abstract class YuoAnimaBaseComponent : YuoComponent
    {
        public abstract YuoAnimaItem NowAnima { get; protected set; }
        public abstract void Play(string clipName, bool force = false);

        public abstract YuoAnimaEvent AddEvent(string clipName, float timeRatio, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop);

        public abstract YuoAnimaTransition AddTransition(string name, string from, string to,
            TransitionType transitionType = TransitionType.AutoTo,
            BoolAction<YuoAnimaTransition> condition = null);

        public abstract void SetSpeed(float speed);

        public abstract void SetSpeed(string clipName, float speed);
    }
}