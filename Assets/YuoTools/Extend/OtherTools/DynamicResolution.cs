using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 根据帧率动态修改分辨率
/// </summary>
public class DynamicResolution : MonoBehaviour
{
    [Header("检测时间")]
    public float ShowTime = 1f;

    private int count = 0;
    private float deltaTime = 0f;

    private List<float> fpsList = new List<float>();
    private int[] resolutionWidths = new int[3];
    private int lowestFps = 50;

    private void Start()
    {
        InitSolutions();
        SetResolution(resolutionWidths[0]);
    }

    private void Update()
    {
        count++;
        deltaTime += Time.deltaTime;
        if (deltaTime >= ShowTime)
        {
            float fps = count / deltaTime;
            float milliSecond = deltaTime * 1000 / count;
            count = 0;
            deltaTime = 0f;

            SetResolutionReduce(fps);
        }
    }

    private void InitSolutions()
    {
        int currentWidth = Screen.currentResolution.width;
        for (int i = 0; i < resolutionWidths.Length; i++)
        {
            resolutionWidths[i] = currentWidth - (int)(currentWidth * (i + 1) * 0.1f);
        }
    }

    private void SetResolution(int width)
    {
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;
        int height = currentHeight * width / currentWidth;
        Screen.SetResolution(width, height, true);
        Debug.Log("强制降低分辨率 目标分辨率为：" + width + " " + height + "  当前分辨率：" + currentWidth + " " + currentHeight);
    }

    private void SetResolutionReduce(float fps)
    {
        if (fps < lowestFps)
        {
            fpsList.Add(fps);
        }
        else
        {
            fpsList.Clear();
        }
        if (fpsList.Count < 5) return;
        fpsList.RemoveRange(0, 3);

        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;
        if (currentWidth < resolutionWidths[resolutionWidths.Length - 1])
        {
            return;
        }

        foreach (int width in resolutionWidths)
        {
            if (width < currentWidth)
            {
                int height = currentHeight * width / currentWidth;
                Screen.SetResolution(width, height, true);
                Debug.Log("由于帧率太低，强制降低分辨率 目标分辨率为：" + width + " " + height + "  当前分辨率：" + currentWidth + " " + currentHeight);
                break;
            }
        }
    }
}