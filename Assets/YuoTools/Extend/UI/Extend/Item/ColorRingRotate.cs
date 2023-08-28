using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YuoTools
{
    public class ColorRingRotate : MonoBehaviour
    {
        Image image;
        SpriteRenderer spriteRenderer;

        private float defFade;

        void Start()
        {
            image = GetComponent<Image>();
            if (image) defFade = image.color.a;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer) defFade = spriteRenderer.color.a;
        }

        float hue = 0;

        public float speed = 1;

        // Update is called once per frame
        void Update()
        {
            var color = Color.HSVToRGB(hue, 1, 1).RSetA(defFade);
            if (image)
            {
                image.color = color;
            }
            else
            {
                spriteRenderer.color = color;
            }

            hue += Time.deltaTime * speed * 0.1f;
            if (hue > 1)
            {
                hue = 0;
            }
        }
    }
}