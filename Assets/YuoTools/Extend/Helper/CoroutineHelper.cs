using System;
using System.Collections;
using System.Threading.Tasks;
using ET;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace YuoTools.Extend.Helper
{
    public static class CoroutineHelper
    {
        public static async ETTask GetEtAwaiter(this AsyncOperation asyncOperation)
        {
            var task = ETTask.Create(true);
            asyncOperation.completed += _ => { task.SetResult(); };
            await task;
        }

        public static async Task GetAwaiter(this AsyncOperation asyncOperation)
        {
            var task = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => { task.SetResult(true); };
            await task.Task;
        }

        public static async ETTask<string> HttpGet(string link)
        {
            try
            {
                var req = UnityWebRequest.Get(link);
                await req.SendWebRequest().GetEtAwaiter();
                return req.downloadHandler.text;
            }
            catch (Exception e)
            {
                throw new Exception($"http request fail: {link.Substring(0, link.IndexOf('?'))}\n{e}");
            }
        }

        public static ETTask<T> GetEtAwaiter<T>(this ResourceRequest request) where T : Object
        {
            var task = ETTask<T>.Create(true);
            YuoAwait_Mono.Instance.StartCoroutine(LoadAsset(request, task));
            return task;
        }

        private static IEnumerator LoadAsset<T>(ResourceRequest request, ETTask<T> tcs) where T : Object
        {
            yield return request;
            var go = request.asset as T;
            tcs.SetResult(go);
        }

        private static ETTask<T> ToAwaiter<T>(T enumerator) where T : IEnumerator
        {
            var task = ETTask<T>.Create();
            YuoAwait_Mono.Instance.StartCoroutine(ToAwaiter(enumerator, task));
            return task;
        }

        private static IEnumerator ToAwaiter<T>(T enumerator, ETTask<T> task) where T : IEnumerator
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;

            task.SetResult(enumerator);
        }

        public delegate T YuoAction<out T>() where T : class;

        public static async ETTask WaitToNonempty<T>(YuoAction<T> obj) where T : class
        {
            int timeOut = 10000;
            while (obj?.Invoke() == null && timeOut-- > 0)
            {
                await YuoWait.WaitFrameAsync();
            }
        }

        #region Unity回调

        //onDisable
        public static ETTask WaitOnDisable(this GameObject go)
        {
            return AwaitOnDisable.Create(go);
        }

        private class AwaitOnDisable : MonoBehaviour
        {
            private ETTask _tcs;

            public static ETTask Create(GameObject go)
            {
                var tcs = ETTask.Create();
                go.AddComponent<AwaitOnDisable>()._tcs = tcs;
                return tcs;
            }

            private void OnDisable()
            {
                _tcs.SetResult();
                _tcs = null;
                Destroy(this);
            }
        }

        //onDestroy
        public static ETTask WaitOnDestroy(this GameObject go)
        {
            return AwaitOnDestroy.Create(go);
        }

        private class AwaitOnDestroy : MonoBehaviour
        {
            private ETTask _tcs;

            public static ETTask Create(GameObject go)
            {
                var tcs = ETTask.Create();
                go.AddComponent<AwaitOnDestroy>()._tcs = tcs;
                return tcs;
            }

            private void OnDestroy()
            {
                _tcs.SetResult();
                _tcs = null;
            }
        }

        //onEnable
        public static ETTask WaitOnEnable(this GameObject go)
        {
            return AwaitOnEnable.Create(go);
        }

        private class AwaitOnEnable : MonoBehaviour
        {
            private ETTask _tcs;

            public static ETTask Create(GameObject go)
            {
                var tcs = ETTask.Create();
                go.AddComponent<AwaitOnEnable>()._tcs = tcs;
                return tcs;
            }

            private void OnEnable()
            {
                _tcs.SetResult();
                _tcs = null;
                Destroy(this);
            }
        }

        #endregion

        #region Button

        public static ETTask WaitOnClick(this Button button)
        {
            var tcs = ETTask.Create();
            button.onClick.AddListener(OnClick);
            return tcs;

            void OnClick()
            {
                tcs.SetResult();
                button.onClick.RemoveListener(OnClick);
            }
        }

        #endregion
    }
}