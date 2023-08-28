using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace YuoTools
{
    public abstract class ProgressBar : MonoBehaviour
    {
        [Range(0, 1f)]
        public float Space = 0;

        private bool IsEnd = false;

        /// <summary>
        /// 返回当前进度值
        /// </summary>
        public abstract float GetProgress();

        public virtual void OnLoadEnd()
        {
            gameObject.Hide();
        }

        public virtual void OnProgressChanged(float p)
        {
        }

        protected float LastProgeress = 0;

        protected virtual void OnEnable()
        {
            IsEnd = false;
            OnProgressChanged(0);
        }

        protected virtual void Update()
        {
            if (!IsEnd)
            {
                float p = GetProgress();
                if (p - LastProgeress >= Space)
                {
                    LastProgeress = p;
                    OnProgressChanged(p);
                }
                if (p >= 1)
                {
                    IsEnd = true;
                    OnLoadEnd();
                }
            }
        }
    }
}