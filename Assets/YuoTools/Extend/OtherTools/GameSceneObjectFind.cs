using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public class GameSceneObjectFind : MonoBehaviour
    {
        public static GameSceneObjectFind Instance;
        public List<GameObject> GameObjects = new ();

        public Dictionary<string, GameObject> Dictionary = new();

        private void Awake()
        {
            Instance = this;
            foreach (var o in GameObjects)
            {
                Dictionary.TryAdd(o.name, o);
            }
        }

        public static GameObject Get(string name)
        {
            if (Instance != null) return Instance.Dictionary[name];
            return null;
        }
    }
}