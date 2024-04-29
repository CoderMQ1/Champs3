using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;


    public static class DoTweenShortcutExtensions
    {
        public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup target, float startValue, float endValue, float duration)
        {
            target.alpha = startValue;
            return DOTween.To((DOGetter<float>)(() =>
                {
                    return target.alpha;
                }),
                (DOSetter<float>)(value =>
                {
                    target.alpha = value;
                }),
                endValue,
                duration);
        }
        
        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFade(
            this Image target,
            float startValue,
            float endValue,
            float duration)
        {
            var color = target.color;
            target.color = new Color(color.r, color.g, color.b, startValue);
            TweenerCore<Color, Color, ColorOptions> alpha = DOTween.ToAlpha((DOGetter<Color>) (() => target.color), (DOSetter<Color>) (x => target.color = x), endValue, duration);
            alpha.SetTarget<TweenerCore<Color, Color, ColorOptions>>((object) target);
            return alpha;
        }
        
        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFade(
        this Image target,
        float endValue,
        float duration)
        {
            TweenerCore<Color, Color, ColorOptions> alpha = DOTween.ToAlpha((DOGetter<Color>) (() => target.color), (DOSetter<Color>) (x => target.color = x), endValue, duration);
            alpha.SetTarget<TweenerCore<Color, Color, ColorOptions>>((object) target);
            return alpha;
        }
        
        
        public static TweenerCore<float, float, FloatOptions> DODolly(
            this CinemachineTrackedDolly target,
            float startValue, float endValue, float duration)
        {
            target.m_PathPosition = startValue;
            return DOTween.To((DOGetter<float>) (() => target.m_PathPosition), (DOSetter<float>) (x => target.m_PathPosition = x), endValue, duration);
            
        }

        
        
        /// <summary>Tweens a Material's alpha color to the given value
        /// (will have no effect unless your material supports transparency).
        /// Also stores the material as the tween's target so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param>
        /// <param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFade(
        this Material target,
        string colorName,
        float endValue,
        float duration)
        {
            TweenerCore<Color, Color, ColorOptions> alpha = DOTween.ToAlpha((DOGetter<Color>) (() => target.GetColor(colorName)), (DOSetter<Color>) (x => target.SetColor(colorName,x)), endValue, duration);
            alpha.SetTarget<TweenerCore<Color, Color, ColorOptions>>((object) target);
            return alpha;
        }
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMove(
        this RectTransform target,
        Vector3 endValue,
        float duration,
        bool snapping = false)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To((DOGetter<Vector3>) (() => target.anchoredPosition3D), (DOSetter<Vector3>) (x => target.anchoredPosition3D = x), endValue, duration);
            t.SetOptions(snapping).SetTarget<Tweener>((object) target);
            return t;
        }
    }
