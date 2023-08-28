using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoAnima
{
    [Icon("Assets/YuoTools/Editor/Texture/Animation2DClip.png")]
    [HideMonoScript]
    public class YuoAnimation2DClip : SerializedScriptableObject
    {
        public int frameRate = 30;
        public List<Sprite> frames = new();

        public Sprite GetFrame(int index)
        {
            return frames[index % frames.Count];
        }

        [ShowInInspector] public float Length => frames.Count / (float)frameRate;

#if UNITY_EDITOR

        /// <summary>
        ///  创建clip,如果选中了Sprite，自动添加
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/YuoAnima2D/Animation2DClip")]
        public static void Create()
        {
            var clip = CreateInstance<YuoAnimation2DClip>();


            var objs = UnityEditor.Selection.objects;
            foreach (var obj in objs)
            {
                if (obj is Sprite sprite)
                {
                    clip.frames.Add(sprite);
                }
                //如果是Texture,则选择所有Sprite
                else if (obj is Texture2D texture2D)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets($"t:Sprite",
                        new[] { UnityEditor.AssetDatabase.GetAssetPath(texture2D) });
                    foreach (var guid in guids)
                    {
                        var path1 = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        var sprite1 = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path1);
                        clip.frames.Add(sprite1);
                    }
                }
            }


            //获取当前选中的文件夹
            var path = UnityEditor.Selection.activeObject == null
                ? "Assets"
                : UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject.GetInstanceID());
            //如果选中的是文件，获取文件的父文件夹
            if (System.IO.Path.GetExtension(path) != "") path = System.IO.Path.GetDirectoryName(path);
            var fileName = "New Animation2DClip";
            if (clip.frames.Count > 0)
                fileName = clip.frames[0].name;


            path = $"{path}/{fileName}.asset";
            //如果有同名文件，自动加上数字
            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);


            UnityEditor.AssetDatabase.CreateAsset(clip, path);
            UnityEditor.Selection.activeObject = clip;
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        [SerializeField] [HideLabel] [PreviewField(height: 300, ObjectFieldAlignment.Center)] [ReadOnly]
        private Sprite preview;

        [HorizontalGroup("Preview")]
        [Button]
        async void PreviewAnima(int loop = 1)
        {
            if (frames.Count == 0) return;

            for (int j = 0; j < loop; j++)
            {
                for (var i = 0; i < frames.Count; i++)
                {
                    preview = frames[i];
                    nowFrame = i;
                    await Task.Delay((int)(1f / frameRate * 1000));
                }
            }

            nowFrame = 0;
            preview = frames[0];
        }

        [HorizontalGroup("PreviewCon")] [SerializeField]
        private int nowFrame = 0;
        
        [ShowInInspector]
        private float NowProgress => nowFrame / (float)frames.Count;

        [Button]
        [HorizontalGroup("PreviewCon")]
        void NextFrame()
        {
            nowFrame++;
            if (nowFrame >= frames.Count)
                nowFrame = 0;

            if (frames.Count == 0) return;
            var frame = frames[nowFrame];
            if (frame != null)
            {
                preview = frames[nowFrame];
            }
        }

        [Button]
        [HorizontalGroup("PreviewCon")]
        void LastFrame()
        {
            nowFrame--;
            if (nowFrame < 0)
                nowFrame = frames.Count - 1;

            if (frames.Count == 0) return;

            var frame = frames[nowFrame];
            if (frame != null)
            {
                preview = frames[nowFrame];
            }
        }

#endif
    }
}