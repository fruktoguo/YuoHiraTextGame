using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using YuoAnima;
using YuoTools;
using YuoTools.Main.Ecs;

namespace YuoAnima
{
    public class YuoAnima2DComponent : YuoAnimaBaseComponent
    {
        public void Init(YuoAnimator2D animator2D)
        {
            Animator = animator2D;
            animator2D.anima = this;
            Play(Animator.defaultAnima.name);
        }

        public SpriteRenderer SpriteRenderer;
        public YuoAnimator2D Animator;
        private YuoAnimaItem _nowAnima;

        public float Speed = 1;

        [ShowInInspector]
        public override YuoAnimaItem NowAnima
        {
            get => _nowAnima;
            protected set
            {
                if (value != _nowAnima)
                {
                    if (value != null) Entity.RunSystem<IAnimaExit>();
                    _nowAnima = value;
                    if (_nowAnima == null) return;
                    Animator.Play(value.AnimaName);
                    Entity.RunSystem<IAnimaEnter>();
                }
            }
        }

        /// <summary>
        /// 直接播放动画
        /// </summary>
        /// <param name="name"></param>
        /// <param name="force"></param>
        public override void Play(string name, bool force = false)
        {
            if (Animator.Animas.TryGetValue(name, out var clip))
            {
                NowAnima = clip.AnimaItem;
            }
            else
            {
                Debug.LogError($"{Entity.EntityName} Can't find clip {name}");
            }
        }

        /// <summary>
        /// 当前所有走线
        /// </summary>
        public Dictionary<string, Dictionary<string, YuoAnimaTransition>> Transitions = new();

        /// <summary>
        ///  添加一个走线
        /// </summary>
        public override YuoAnimaTransition AddTransition(string name, string from, string to,
            TransitionType transitionType = TransitionType.AutoTo,
            BoolAction<YuoAnimaTransition> condition = null)
        {
            if (Animator.Animas.TryGetValue(from, out var fromClip) &&
                Animator.Animas.TryGetValue(to, out var toClip))
            {
                var transition = new YuoAnimaTransition(fromClip.AnimaItem, toClip.AnimaItem)
                {
                    duration = 0,
                    transitionType = transitionType,
                    condition = condition,
                };
                if (Transitions.TryGetValue(from, out var transitions))
                {
                    transitions.Add(name, transition);
                }
                else
                {
                    Transitions.Add(from, new Dictionary<string, YuoAnimaTransition>
                    {
                        { name, transition }
                    });
                }

                GetAnimaItem(from).Transitions.Add(transition);
                return transition;
            }
            else
            {
                Debug.LogError($"{Entity.EntityName} Can't find clip {from} or {to}");
                return null;
            }
        }

        public override void SetSpeed(float speed)
        {
            Animator.Speed = speed;
        }

        public override void SetSpeed(string clipName, float speed)
        {
            if (Animator.Animas.TryGetValue(clipName, out var animaItem))
            {
                animaItem.Speed = speed;
            }
            else
            {
                Debug.LogError($"{Entity.EntityName} Can't find clip {clipName}");
            }
        }

        public YuoAnimaItem GetAnimaItem(string name)
        {
            if (Animator.Animas.TryGetValue(name, out var clip))
            {
                return clip.AnimaItem;
            }
            else
            {
                Debug.LogError($"{Entity.EntityName} Can't find clip {name}");
                return null;
            }
        }

        public override YuoAnimaEvent AddEvent(string clipName, float timeRatio, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop)
        {
            var clip = GetAnimaItem(clipName);
            var animaEvent = new YuoAnimaEvent
            {
                eventType = eventType,
                time = timeRatio.RClamp(0, 1),
                action = action
            };
            clip.Events.Add(animaEvent);
            return animaEvent;
        }
    }


    public class YuoAnima2DUpdateSystem : YuoSystem<YuoAnima2DComponent>, IUpdate
    {
        public override string Group => SystemGroupConst.Anima;
        protected override void Run(YuoAnima2DComponent anima)
        {
            anima.Animator.Update();
        }
    }

    public interface IAnimaEnter : ISystemTag
    {
    }

    public interface IAnimaExit : ISystemTag
    {
    }
}