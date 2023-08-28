using System;
using UnityEngine;
using System.Collections.Generic;
using ET;
using UnityEngine.Events;
using YuoTools.Main.Ecs;

namespace YuoTools
{
    [AutoAddToMain()]
    public partial class YuoInputComponent : YuoComponentInstance<YuoInputComponent>
    {
        [SerializeField] Dictionary<string, InputItem> _all = new();
        private Dictionary<string, ETTask> _allDown = new();
    }

    public class InputCheckBaseComponent : YuoComponent
    {
        public virtual bool GetDown(KeyCode key) => false;
        public virtual bool GetUp(KeyCode key) => false;
        public virtual bool GetKey(KeyCode key) => false;
    }

    [AutoAddToMain()]
    public class YuoInputCheckComponent : InputCheckBaseComponent
    {
        public override bool GetDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public override bool GetUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public override bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }
    }

    public partial class YuoInputComponent
    {
        /// <summary>
        /// 遍历所有的输入项
        /// </summary>
        /// <param name="item"></param>
        void UpdateItem(InputItem item, InputCheckBaseComponent check)
        {
            //如果按下了这些键,则这个按键不会被监听
            foreach (var before in item.Befor)
            {
                if (GetItem(before).Down)
                {
                    return;
                }
            }

            if (item.canInput != null)
                //判断是否能按下,不就就跳出
                if (!item.canInput())
                    return;

            //上一次持续按下的时间不为零
            if (item.LastOnTime > 0.0001f)
            {
                if (item.OnTime - item.LastOnTime < 0.001f)
                {
                    //说明up事件没有正确触发,手动触发一次up事件
                    Up(item);
                }
            }

            //按键task超时
            if (item.InputTcs.Count > 0 && Time.unscaledTime - item.LastClickTime > 60)
            {
                item.InputTcs.Clear();
            }

            //判断对应的按键是否按下
            if (check.GetDown(item.key))
            {
                item.OnDown?.Invoke();
                //更显点击时间
                item.LastClickTime = Time.unscaledTime;
                item.Down = true;

                foreach (var tcs in item.InputTcs)
                {
                    tcs.SetResult();
                }

                item.InputTcs.Clear();
            }

            if (check.GetKey(item.key))
            {
                //意外事件  没按下就开始触发持续按下事件
                if (item.UseMustDown && !item.Down) return;
                //按下持续的时间
                item.OnTime += Time.unscaledDeltaTime;
                item.OnHold?.Invoke();
            }

            if (check.GetUp(item.key))
            {
                if (item.UseMustDown && !item.Down) return;
                Up(item);
            }
        }

        public void Pause()
        {
            Stop = true;
        }

        public void Resume()
        {
            Stop = false;
        }

        void Up(InputItem item)
        {
            item.OnUp?.Invoke();
            item.OnTime = 0;
            item.LastOnTime = 0;
            item.Down = false;
        }

        public InputItem GetItem(string key)
        {
            if (!_all.ContainsKey(key))
            {
                _all.Add(key, new InputItem(key));
            }

            return _all[key];
        }

        public bool Stop;

        public void Update()
        {
            if (Stop) return;
            if (Entity.TryGetBaseComponent<InputCheckBaseComponent>(out var check))
            {
                foreach (var item in _all.Values)
                {
                    UpdateItem(item, check);
                }
            }
        }

        public void Add(InputItem item)
        {
            if (!_all.ContainsKey(item.KeyName))
            {
                _all.Add(item.KeyName, item);
            }
            else
            {
                Debug.LogWarning("已经存在相同的键名:" + item.KeyName);
                var input = _all[item.KeyName];

                input.key = item.key;
                input.OnDown += item.OnDown;
                input.OnUp += item.OnUp;
                input.canInput += item.canInput;
                input.OnHold += item.OnHold;
            }
        }

        public void AddDown(string key, KeyCode code, UnityAction down)
        {
            if (!_all.ContainsKey(key))
                _all.Add(key, new InputItem(key, code));
            _all[key].OnDown += down;
        }

        public void RemoveDown(string key, UnityAction down)
        {
            if (!_all.ContainsKey(key))
                _all.Add(key, new InputItem(key));
            _all[key].OnDown -= down;
        }

        public void Save()
        {
        }

        [System.Serializable]
        public class InputItem
        {
            public InputItem(string name)
            {
                KeyName = name;
            }

            public InputItem(string name, KeyCode key)
            {
                KeyName = name;
                this.key = key;
            }

            /// <summary>
            /// 按键名称
            /// </summary>
            public string KeyName;

            /// <summary>
            /// 按键键码
            /// </summary>
            public KeyCode key;

            public float OnTime;

            public float LastOnTime;

            public float LastClickTime;

            /// <summary>
            /// 按下时持续触发
            /// </summary>
            [NonSerialized] public UnityAction OnHold;

            /// <summary>
            /// 按下时触发一次
            /// </summary>
            [NonSerialized] public UnityAction OnDown;

            /// <summary>
            /// 抬起时触发
            /// </summary>
            [NonSerialized] public UnityAction OnUp;

            /// <summary>
            /// 判断是否可用
            /// </summary>
            [NonSerialized] public BoolAction canInput;

            public List<ETTask> InputTcs = new();

            public ETTask WaitDown()
            {
                var tcs = ETTask.Create();
                InputTcs.Add(tcs);
                return tcs;
            }

            /// <summary>
            /// 忽略按键名称,当目标按键按下时,这个按键不会触发
            /// </summary>
            public List<string> Befor = new List<string>();

            internal bool Down;
            internal bool UseMustDown;

            public void AddBefor(string key)
            {
                if (!Befor.Contains(key))
                {
                    Befor.Add(key);
                }
            }

            public InputItem SetCanInput(BoolAction canInput)
            {
                this.canInput = canInput;
                return this;
            }
        }
    }

    public class YuoInputSystem : YuoSystem<YuoInputComponent>, IUpdate
    {
        public override string Group => SystemGroupConst.Input;

        protected override void Run(YuoInputComponent component)
        {
            component.Update();
        }
    }
}