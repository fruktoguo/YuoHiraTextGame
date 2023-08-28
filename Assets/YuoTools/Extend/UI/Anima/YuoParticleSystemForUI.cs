using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace YuoTools
{
    public class YuoParticleSystemForUI : MonoBehaviour
    {
        public float cycleTime = 1;
        public float lifeTime = 1;
        public float startDelay = 0;

        Image _image;

        [SerializeField] bool _colorOverLifetime = false;

        [ShowIf("_colorOverLifetime")] public Gradient colorOverLifetimeGradient;
        private Color _baseColor;

        [SerializeField] bool _sizeOverLifetime = false;
        [ShowIf("_sizeOverLifetime")] public AnimationCurve sizeOverLifetimeCurve;
        private Vector3 _baseScale;

        [SerializeField] bool _sizeDeltaOverLifetime = false;
        [ShowIf("_sizeDeltaOverLifetime")] public AnimationCurve sizeDeltaOverLifetimeCurve;
        Vector2 _baseSizeDelta;

        private float _timer;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _baseColor = _image.color;
            _baseScale = _image.transform.localScale;
            _baseSizeDelta = _image.rectTransform.sizeDelta;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < lifeTime)
            {
                OnLife(_timer / lifeTime);
            }
            else if (_timer > cycleTime)
            {
                _timer = 0;
            }
        }

        void OnLife(float life)
        {
            if (_colorOverLifetime)
                _image.color = _baseColor * colorOverLifetimeGradient.Evaluate(life);
            if (_sizeOverLifetime)
                transform.localScale = _baseScale * sizeOverLifetimeCurve.Evaluate(life);
            if (_sizeDeltaOverLifetime)
                _image.rectTransform.sizeDelta = _baseSizeDelta * sizeDeltaOverLifetimeCurve.Evaluate(life);
        }
    }
}