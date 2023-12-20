using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoAnima
{
    [CreateAssetMenu(menuName = "YuoAnima2D/Create YuoAnimator2D", fileName = "YuoAnimator2D", order = 0)]
    [Icon("Assets/YuoTools/Editor/Texture/Animator2D.png")]
    [HideMonoScript]
    public class YuoAnimator2DScriptableObject : SerializedScriptableObject
    {
        public YuoAnimation2DClip defaultClip;
        public List<YuoAnimation2DClip> clips = new();

        [HideInInspector]
        public Dictionary<int, YuoAnimation2DClip> clipDic = new();

        public void Init()
        {
            clipDic.Clear();
            foreach (var clip in clips)
            {
                clipDic.Add(clip.name.GetHashCode(), clip);
            }
        }

        public bool TryGetClip(string clipName, out YuoAnimation2DClip clip)
        {
            clipDic.TryGetValue(clipName.GetHashCode(), out clip);
            return clip != null;
        }

        public bool TryGetClip(int hashName, out YuoAnimation2DClip clip)
        {
            clipDic.TryGetValue(hashName, out clip);
            return clip != null;
        }

        public YuoAnimation2DClip GetClip(string clipName)
        {
            TryGetClip(clipName, out var clip);
            return clip;
        }

        public YuoAnimation2DClip GetClip(int hashName)
        {
            TryGetClip(hashName, out var clip);
            return clip;
        }

#if UNITY_EDITOR

        /// <summary>
        ///  创建clip,如果选中了Sprite，自动添加
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/YuoAnima2D/YuoAnimator2D")]
        public static void Create()
        {
            var animator = CreateInstance<YuoAnimator2DScriptableObject>();

            var objs = UnityEditor.Selection.objects;
            foreach (var obj in objs)
            {
                if (obj is YuoAnimation2DClip clip)
                {
                    animator.clips.Add(clip);
                }
            }

            if (animator.clips.Count > 0) animator.defaultClip = animator.clips[0];

            //获取当前选中的文件夹
            var path = UnityEditor.Selection.activeObject == null
                ? "Assets"
                : UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject.GetInstanceID());
            //如果选中的是文件，获取文件的父文件夹
            if (System.IO.Path.GetExtension(path) != "") path = System.IO.Path.GetDirectoryName(path);
            var fileName = "YuoAnimator2D";

            path = $"{path}/{fileName}.asset";
            //如果有同名文件，自动加上数字
            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);

            UnityEditor.AssetDatabase.CreateAsset(animator, path);
            UnityEditor.Selection.activeObject = animator;
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}