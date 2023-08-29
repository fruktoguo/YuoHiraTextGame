using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.Extend.Ai.AIGC;

public class Test : MonoBehaviour
{
    public RawImage Image;

    [Button]
    private async void Draw(string parameters)
    {
        Image.texture = await AIGC.Draw(parameters);
    }

    // Update is called once per frame
    void Update()
    {
    }
}