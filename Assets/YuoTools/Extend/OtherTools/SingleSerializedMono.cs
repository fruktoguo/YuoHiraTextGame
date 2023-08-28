using UnityEngine;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;

#endif

namespace YuoTools
{
#if ODIN_INSPECTOR

    public class SingletonSerializedMono<T> : SerializedMonoBehaviour where T : SingletonSerializedMono<T>
#else
    public class SingletonSerializedMono<T> : MonoBehaviour where T : SingletonSerializedMono<T>
#endif
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return instance;
            }
            protected set => instance = value;
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}