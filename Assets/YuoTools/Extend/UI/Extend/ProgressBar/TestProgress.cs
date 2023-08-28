using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace YuoTools
{
    public class TestProgress : ProgressBar
    {
        public TMP_Text text;
        public FloatAction getProgress;

        public override float GetProgress()
        {
            if (getProgress != null)
            {
                return getProgress();
            }

            return timer;
        }

        private void Awake()
        {
            getProgress += () => timer;
            getProgress += () => timer * 2;
        }

        public override void OnProgressChanged(float p)
        {
            text.text = p.ToString("0.00");
        }

        private float timer;

        protected override void Update()
        {
            base.Update();
            timer += Time.deltaTime * 0.2f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            timer = 0;
        }

        public override void OnLoadEnd()
        {
            base.OnLoadEnd();
        }
    }
}