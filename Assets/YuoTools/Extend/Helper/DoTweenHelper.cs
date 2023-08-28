using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ET;

namespace YuoTools.Extend.UI.YuoLayout
{
    public static class DoTweenHelper
    {
        public static TweenerCore<float, float, FloatOptions> To(this float value, float to, float duration, Action<float> onValueChange)
        {
          return  DOTween.To(() => value, x => onValueChange(x), to, duration);
        }
    }
}