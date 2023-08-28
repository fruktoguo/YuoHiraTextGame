using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YuoTools
{
    public class LoadSceneAsync : MonoBehaviour
    {
        public static LoadSceneAsync instance;

        //显示进度的文本
        private Text progress;

        private AsyncOperation async = null;

        private void Awake()
        {
            instance = this;
            gameObject.SetActive(false);
        }

        private void Start()
        {
            progress = GetComponent<Text>();
        }

        public void LoadScene(string sceneName)
        {
            gameObject.SetActive(true);
            StartCoroutine(ILoadScene(sceneName));
        }

        private IEnumerator ILoadScene(string sceneName)
        {
            async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = true;
            while (!async.isDone)
            {
                if (async.progress < 0.9f)
                    progress.text = $"加载场景中…… {(int)async.progress}%";
                else
                {
                    progress.text = $"加载场景中…… 100%";
                    yield break;
                }
                yield return null;
            }
        }
    }
}