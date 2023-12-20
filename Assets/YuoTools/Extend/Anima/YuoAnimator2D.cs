using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace YuoAnima
{
    public class YuoAnimator2D
    {
        public SpriteRenderer SpriteRenderer;

        public YuoAnima2DItem defaultAnima;

        public YuoAnima2DItem NowAnima;

        public Dictionary<string, YuoAnima2DItem> Animas = new();

        public YuoAnimator2DScriptableObject scriptableObject;

        public YuoAnimator2D(YuoAnimator2DScriptableObject scriptableObject, SpriteRenderer spriteRenderer)
        {
            SpriteRenderer = spriteRenderer;
            defaultAnima = new YuoAnima2DItem(scriptableObject.defaultClip);

            foreach (var yuoAnimation2DClip in scriptableObject.clips)
            {
                Animas.Add(yuoAnimation2DClip.name, new YuoAnima2DItem(yuoAnimation2DClip));
            }

            this.scriptableObject = scriptableObject;
            this.scriptableObject.Init();
        }

        public YuoAnima2DComponent anima;

        public void Play(string clipName)
        {
            if (scriptableObject.clipDic.TryGetValue(clipName.GetHashCode(), out var clip))
            {
                if (clip.frames.Count > 0)
                {
                    SpriteRenderer.sprite = clip.frames[0];
                }
                NowAnima?.Reset();
                NowAnima = Animas[clipName];
                NowAnima?.Reset();
            }
            else
            {
                Debug.LogError("没有这个动画");
            }
        }

        public void Update()
        {
            if (NowAnima == null) return;
            NowAnima.currentTime += Time.deltaTime * Speed * NowAnima.Speed;
            while (NowAnima.currentTime >= NowAnima.Length)
            {
                NowAnima.currentTime -= NowAnima.Length;
                NowAnima.LoopCount++;
            }

            NowAnima.currentFrame = (int)(NowAnima.currentTime / NowAnima.frameTime);

            NowAnima.currentProgress =
                NowAnima.LoopCount + NowAnima.currentTime / NowAnima.Clip.Length;

            SpriteRenderer.sprite = NowAnima.Clip.frames[NowAnima.currentFrame];

            //事件
            foreach (var animaEvent in NowAnima.AnimaItem.Events)
            {
                switch (animaEvent.eventType)
                {
                    case AnimaEventType.Once:
                        if (animaEvent.num == 0 && NowAnima.currentProgress >= animaEvent.time)
                        {
                            animaEvent.num++;
                            animaEvent.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Loop:
                        if ((NowAnima.currentProgress > animaEvent.num &&
                             NowAnima.currentProgress % 1 >= animaEvent.time) ||
                            //防止动画速度过快的情况导致的事件不触发
                            NowAnima.currentProgress > animaEvent.num + 1)
                        {
                            animaEvent.num++;
                            animaEvent.action?.Invoke();
                        }

                        break;

                    case AnimaEventType.Update:
                        animaEvent.action?.Invoke();
                        break;
                }
            }

            //动画播放完了,判断一下是否有走线
            foreach (var transition in NowAnima.AnimaItem.Transitions)
            {
                switch (transition.transitionType)
                {
                    case TransitionType.AutoTo:
                        if (NowAnima.currentProgress >= 1)
                        {
                            anima.Play(transition.toClip.AnimaName);
                        }

                        break;
                    case TransitionType.WaitCondition:
                        break;
                    case TransitionType.AutoToAndWaitCondition:
                        break;
                }
            }
        }

        public float Speed = 1;
    }

    public class YuoAnima2DItem
    {
        public YuoAnima2DItem(YuoAnimation2DClip clip)
        {
            FrameRate = clip.frameRate;
            name = clip.name;
            Clip = clip;
            Length = clip.Length;
            frameTime = 1f / FrameRate;
            AnimaItem = new YuoAnimaItem
            {
                AnimaName = clip.name,
                Length = clip.Length
            };
        }

        public YuoAnimaItem AnimaItem;

        public string name;

        public float FrameRate;

        public float Length;

        public YuoAnimation2DClip Clip;


        public float Speed = 1;

        public float frameTime;

        public float currentTime;
        public int currentFrame;
        public float currentProgress;
        public int LoopCount;

        public void Reset()
        {
            currentFrame = 0;
            currentTime = 0;
            currentProgress = 0;
            LoopCount = 0;
            foreach (var animaEvent in AnimaItem.Events)
            {
                animaEvent.Reset();
            }
        }
    }

    [Serializable]
    public class YuoAnima2DEvent
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