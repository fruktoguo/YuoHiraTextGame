using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
namespace YuoTools
{
    public class SetMultiDisplay : MonoBehaviour
    {
        RenderTexture renderTexture;
        [SerializeField]
        List<RawImage> display = new List<RawImage>();
        public Camera mainCamera;
        void Awake()
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 1);
            mainCamera.targetTexture = renderTexture;
            foreach (var item in display)
            {
                item.texture = renderTexture;
            }
            if (display.Count > 1)
            {
                for (int i = 1; i < display.Count; i++)
                {
                    if (Display.displays.Length > i)
                    {
                        Display.displays[i].Activate();
                        Display.displays[i].SetRenderingResolution(Display.displays[i].systemWidth, Display.displays[i].systemHeight);
                    }
                }
            }
        }
    }
}