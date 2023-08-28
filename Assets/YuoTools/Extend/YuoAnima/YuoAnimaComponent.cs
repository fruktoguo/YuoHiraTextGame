using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using YuoTools;
using YuoTools.Main.Ecs;

namespace YuoAnima
{
    public class YuoAnimaComponent : YuoAnimaBaseComponent
    {
        public Animator animator { get; private set; }

        [ShowInInspector] [ReadOnly]
        private Dictionary<string, YuoAnimaItem> animas = new Dictionary<string, YuoAnimaItem>();

        [ShowInInspector] [ReadOnly] public override YuoAnimaItem NowAnima { get; protected set; }

        public YuoStateMachineBehaviour YuoStateMachineBehaviour { get; private set; }

        public void Init(Animator anima)
        {
            animator = anima;
            YuoStateMachineBehaviour = anima.GetBehaviour<YuoStateMachineBehaviour>();
            if (!YuoStateMachineBehaviour)
            {
                Debug.LogError($"请在 [AnimatorController] 的 [layer] 中添加行为 [YuoStateMachineBehaviour] ");
            }

            YuoStateMachineBehaviour.Init(OnStateEnter, OnStateExit, OnStateMove, OnStateUpdate);
        }

        public bool isUpdateMode = true;

        public void FixedUpdate()
        {
            if (!isUpdateMode)
                MUpdate(Time.fixedTime);
        }

        public void Update()
        {
            if (isUpdateMode)
            {
                MUpdate(Time.deltaTime);
            }
        }

        void MUpdate(float deltaTime)
        {
            if (NowAnima != null)
            {
                foreach (var item in NowAnima.Transitions)
                {
                    switch (item.transitionType)
                    {
                        case TransitionType.AutoTo:
                            if (item.duration >= 1)
                            {
                                Play(item.toClip.AnimaName);
                            }

                            break;
                        case TransitionType.WaitCondition:
                            if (item.condition != null && item.condition(item))
                                Play(item.toClip.AnimaName);

                            break;
                        case TransitionType.AutoToAndWaitCondition:
                            if (item.duration >= 1 && item.condition != null && item.condition(item))
                            {
                                Play(item.toClip.AnimaName);
                            }

                            break;
                    }
                }
            }
        }

        private float animaSpeed = 1;

        public override void SetSpeed(float speed)
        {
            animaSpeed = speed;
            animator.speed = speed;
        }

        public override void SetSpeed(string clipName, float speed)
        {
            var speedParam = $"{clipName}Speed";
            animator.SetFloat(speedParam, speed);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public override void Play(string animaName, bool force = false)
        {
            if (!force)
            {
                if (NowAnima == null || NowAnima.AnimaName != animaName)
                {
                    animator.Play(animaName);
                }
            }
            else
                animator.Play(animaName, 0, 0);
        }

        /// <summary>
        /// 根据Hash值获取对应动画名称
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string GetStateName(int hash)
        {
            if (HashCodeManager.Get.TryGetValue(hash, out var stateName))
            {
                return stateName;
            }

            return null;
        }

        public string GetStateName(AnimatorStateInfo state)
        {
            return GetStateName(state.shortNameHash);
        }

        YuoAnimaItem GetAnimaItem(string animaName)
        {
            if (animas.TryGetValue(animaName, out var item))
            {
                return item;
            }

            item = new YuoAnimaItem
            {
                AnimaName = animaName
            };

            animas.Add(animaName, item);

            return item;
        }

        private void OnStateEnter(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateEnter  {HashToName[stateInfo.shortNameHash]}");

            var clipName = GetStateName(stateInfo);

            NowAnima = GetAnimaItem(clipName);

            NowAnima.Length = stateInfo.length;

            foreach (var item in NowAnima.Events)
            {
                item.num = 0;
                if (item.eventType == AnimaEventType.Enter)
                {
                    item.num++;
                    item.action?.Invoke();
                }
            }

            foreach (var transition in NowAnima.Transitions)
            {
                transition.duration = 0;
            }

            Entity.RunSystem<IAnimaEnter>();
        }

        private void OnStateExit(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateExit  {HashToName[stateInfo.shortNameHash]}");

            if (NowAnima == null) return;
            var clipName = GetStateName(stateInfo);
            if (clipName == null || clipName != NowAnima.AnimaName) return;

            var animaItem = GetAnimaItem(clipName);

            UpdateAction(anima, stateInfo, layerIndex);

            ExitAction(animaItem, anima, stateInfo, layerIndex);

            Entity.RunSystem<IAnimaExit>();

            foreach (var transition in NowAnima.Transitions)
            {
                transition.duration = 0;
            }
        }

        void ExitAction(YuoAnimaItem animaItem, Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var item in animaItem.Events)
            {
                if (item.eventType == AnimaEventType.Exit)
                {
                    item.num++;
                    item.action?.Invoke();
                }
            }

            foreach (var transition in NowAnima.Transitions)
            {
                transition.duration = stateInfo.normalizedTime;
            }
        }

        private void OnStateMove(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateMove  {HashToName[stateInfo.shortNameHash]}");
        }

        private void OnStateUpdate(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //print($"OnStateUpdate  {HashToName[stateInfo.shortNameHash]}");

            if (NowAnima == null) return;

            if (GetStateName(stateInfo) != NowAnima.AnimaName) return;

            UpdateAction(anima, stateInfo, layerIndex);
        }

        void UpdateAction(Animator anima, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var item in NowAnima.Events)
            {
                switch (item.eventType)
                {
                    case AnimaEventType.Once:
                        if (item.num == 0 && stateInfo.normalizedTime >= item.time)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Loop:
                        if ((stateInfo.normalizedTime > item.num && stateInfo.normalizedTime % 1 >= item.time) ||
                            //防止动画速度过快的情况导致的事件不触发
                            stateInfo.normalizedTime > item.num + 1)
                        {
                            item.num++;
                            item.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Update:
                        item.action?.Invoke();
                        break;
                }
            }

            foreach (var transition in NowAnima.Transitions)
            {
                transition.duration = stateInfo.normalizedTime;
            }
        }

        public override YuoAnimaTransition AddTransition(string name, string from, string to,
            TransitionType transitionType = TransitionType.AutoTo, BoolAction<YuoAnimaTransition> condition = null)
        {
            var fromAnima = GetAnimaItem(from);
            var toAnima = GetAnimaItem(to);
            YuoAnimaTransition transition = new YuoAnimaTransition(fromAnima, toAnima);
            fromAnima.Transitions.Add(transition);
            transition.condition = condition;
            return transition;
        }

        public override YuoAnimaEvent AddEvent(string clip, float timeRatio, UnityAction action,
            AnimaEventType eventType = AnimaEventType.Loop)
        {
            YuoAnimaItem anima = GetAnimaItem(clip);

            YuoAnimaEvent animaEvent = new YuoAnimaEvent()
            {
                time = timeRatio.RClamp(1f),
                action = action,
                eventType = eventType
            };
            anima.Events.Add(animaEvent);
            return animaEvent;
        }
    }

    public class YuoAnimaUpdateSystem : YuoSystem<YuoAnimaComponent>, IFixedUpdate, IUpdate
    {
        public override string Group => SystemGroupConst.Anima;
        protected override void Run(YuoAnimaComponent component)
        {
            if (component.isUpdateMode)
                component.Update();
            else
                component.FixedUpdate();
        }
    }

    [Serializable]
    public class YuoAnimaTransition
    {
        public YuoAnimaItem fromClip;
        public YuoAnimaItem toClip;
        public float duration = 0;
        public TransitionType transitionType = TransitionType.AutoTo;
        public BoolAction<YuoAnimaTransition> condition;

        public YuoAnimaTransition(YuoAnimaItem fromClip, YuoAnimaItem toClip)
        {
            this.fromClip = fromClip;
            this.toClip = toClip;
        }
    }

    //[System.Serializable]
    public class YuoAnimaItem
    {
        public string AnimaName = "Null";

        public float Length;

        [ShowInInspector] [ReadOnly] public readonly List<YuoAnimaEvent> Events = new();

        public readonly List<YuoAnimaTransition> Transitions = new();
    }

    public enum AnimaEventType
    {
        Loop = 0,
        Once = 1,
        Update = 2,
        Exit = 3,
        Enter = 4,
    }

    public enum TransitionType
    {
        /// <summary>
        /// 等播放完自动切换
        /// </summary>
        AutoTo,

        /// <summary>
        /// 等待符合条件切换
        /// </summary>
        WaitCondition,

        /// <summary>
        /// 等待符合条件且播放完切换
        /// </summary>
        AutoToAndWaitCondition,
    }

    [Serializable]
    public class YuoAnimaEvent
    {
        public AnimaEventType eventType = AnimaEventType.Loop;
        public float time;
        public UnityAction action;
        public int num;

        public void Reset()
        {
            num = 0;
        }
    }
}