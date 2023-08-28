using System.Collections.Generic;
using DG.Tweening;
using ET;
using UnityEngine;

namespace YuoTools.Main.Ecs
{
    public class SoundManger : SingletonMono<SoundManger>
    {
        [Header("正在播放的AudioSource")]
        public List<SoundData> Sounds = new List<SoundData>();

        [Header("休眠的AudioSource")]
        public List<SoundData> SoundPools = new List<SoundData>();

        [Header("背景音效")]
        public SoundData Bg;

        [System.Serializable]
        public class SoundData
        {
            public string name = "null";
            public float SoundVolume = 1;
            public AudioSource source;
        }

        [Header("最大缓存Source数量")]
        public int MaxSourceNum = 8;

        private float BgValue = 1;
        private float SoundValue = 1;
        private List<ETTask> AllTask = new ();

        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            Bg = new SoundData() { source = gameObject.AddComponent<AudioSource>() };
            Bg.source.playOnAwake = false;
        }

        public void SetBgVolume(float f)
        {
            f.Clamp(1);
            BgValue = f;
            Bg.source.volume = f * Bg.SoundVolume;
        }

        public void SetSoundVolume(float f)
        {
            f.Clamp(1);
            SoundValue = f;
            foreach (var item in Sounds)
            {
                item.source.volume = f * item.SoundVolume;
            }
        }

        public async void PlaySound(AudioClip clip, float value = 1)
        {
            if (clip == null) return;
            SoundData asTemp;
            if (SoundPools.Count > 0)
            {
                asTemp = SoundPools[0];
                SoundPools.Remove(asTemp);
            }
            else
            {
                asTemp = new SoundData() { source = gameObject.AddComponent<AudioSource>() };
            }
            asTemp.source.Stop();
            asTemp.source.clip = clip;
            asTemp.source.loop = false;
            asTemp.source.playOnAwake = false;
            asTemp.source.volume = SoundValue * value;
            asTemp.SoundVolume = value;
            asTemp.source.Play();
            Sounds.Add(asTemp);
            ETTask task = YuoWait.WaitTimeAsync(clip.length);
            AllTask.Add(task);
            await task;
            asTemp.source.Stop();
            Sounds.Remove(asTemp);
            SoundPools.Add(asTemp);
            AllTask.Remove(task);
        }

        public void StopAllSound()
        {
            for (int i = 0; i < Sounds.Count;)
            {
                Sounds[0].source.Stop();
                SoundPools.Add(Sounds[0]);
                Sounds.RemoveAt(0);
            }
            MaxSourceNum.Clamp();
            if (SoundPools.Count > MaxSourceNum)
            {
                for (int i = MaxSourceNum; i < SoundPools.Count;)
                {
                    Destroy(SoundPools[i].source);
                    SoundPools.RemoveAt(MaxSourceNum);
                }
            }
        }

        public void PlayBg(AudioClip clip, float value = 1, bool Loop = true)
        {
            if (Bg.source.clip != clip)
            {
                Bg.source.clip = clip;
                Bg.source.Play();
            }
            Bg.source.loop = Loop;
            Bg.SoundVolume = value;
            Bg.source.volume = BgValue * value;
        }

        public void PauseBg()
        {
            Bg.source.Pause();
        }

        public void StopBg()
        {
            Bg.source.Stop();
            Bg.source.clip = null;
        }

        public void ResumeBg()
        {
            Bg.source.Play();
        }

        public void PauseSound()
        {
            foreach (var item in Sounds)
            {
                item.source.Pause();
            }
        }

        public void ResumeSound()
        {
            foreach (var item in Sounds)
            {
                item.source.Play();
            }
        }
    }
}