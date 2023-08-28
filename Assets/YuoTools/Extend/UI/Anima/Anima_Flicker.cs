using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ET;
using UnityEngine;

using YuoTools;

public class Anima_Flicker : MonoBehaviour
{
    public int FlickerNum = 5;
    public float FlickerDelay = 0.01666f;
    public GameObject flickerGO;
    public float StartDelay = 0;

    public static async Task Flicker(GameObject FlickerGO, int FlickerNum, float FlickerDelay, ETTask task = null)
    {
        for (int i = 0; i < FlickerNum * 2; i++)
        {
            await YuoWait.WaitTimeAsync(FlickerDelay);
            FlickerGO.SetActive(i % 2 == 1);
        }
        task?.SetResult();
    }

    public static async Task FlickerFrame(GameObject FlickerGO, int FlickerNum ,int FlickerDelay = 1, ETTask task = null)
    {
        for (int i = 0; i < FlickerNum * 2; i++)
        {
            await YuoWait.WaitFrameAsync(FlickerDelay);
            FlickerGO.SetActive(i % 2 == 1);
        }
        task?.SetResult();
    }

    private async void OnEnable()
    {
        flickerGO.SetActive(false);
        if (StartDelay > 0) await YuoWait.WaitTimeAsync(StartDelay);
        await Flicker(flickerGO, FlickerNum, FlickerDelay);
    }
}