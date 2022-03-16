using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sound.JFieldBGMList;
using Random = UnityEngine.Random;

public enum BGM
{
    Title,
    Select,
    Result,
    Extra,
    Intro,
    Credit
}

public enum SE
{
    TitleGate,
    SelectMain,
    SelectSub,
    Deside,
    Ready, // 3カウント
    Go,
    CountDown,
    Speedup,
    Miss,
    LifeAlert,
    Gameover,
    ShutterClose,
    ShutterOpen,
    DesideOD
}

public sealed class Sound : SingletonMonoBehaviour<Sound>
{
    [SerializeField, EnumIndex(typeof(BGM))] private List<BGMData> bgmList;
    [SerializeField, EnumIndex(typeof(SE))] private List<SEData> seList;

    private BGMAudio audioBGM = new BGMAudio();
    private BGMAudio audioBGMsub;
    private AudioSource audioSE;

    private int fieldBGMCount;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            audioBGM.audio = gameObject.AddComponent<AudioSource>();
            audioSE = gameObject.AddComponent<AudioSource>();

            List<FieldBGM> list = JsonUtility.FromJson<JFieldBGMList>(Resources.Load("System/FieldBGMList").ToString()).recordsList;
            fieldBGMCount = list.Count;
            foreach (FieldBGM f in list)
            {
                bgmList.Add(new BGMData(f));
            }
        }
    }

    public int FieldBGMCount
    {
        get => fieldBGMCount;
    }

    public void Play(BGM bgm)
    {
        Debug.Log($"BGM {bgm}");

        audioBGM.Play(bgmList[(int)bgm]);
    }

    public void Play(SE se)
    {
        Debug.Log($"SE {se}");

        float v = se == SE.CountDown ? SystemSettings.VolumeCountDown : SystemSettings.VolumeSE;
        audioSE.PlayOneShot(seList[(int)se].clip, seList[(int)se].volume * v / 100.0f);
    }

    public void PlayField(int index)
    {
        if (index == -1)
        {
            index = Enum.GetNames(typeof(BGM)).Length + //
                Random.Range(0, bgmList.Count - Enum.GetNames(typeof(BGM)).Length);
        }
        else
        {
            index = Enum.GetNames(typeof(BGM)).Length + index < bgmList.Count ? //
                Enum.GetNames(typeof(BGM)).Length + index : bgmList.Count - 1;
        }

        audioBGM.Play(bgmList[index]);
    }

    public void Stop()
    {
        audioBGM.FadeOut(90);
    }

    public void AdjustPitch(int speedlevel, int minspeedlevel)
    {
        float pitch = 0.05f * ((minspeedlevel == 1 ? speedlevel : speedlevel - minspeedlevel) / 10) + 1.0f;
        audioBGM.SetPitch(pitch);
    }

    public void RefreshVolume()
    {
        audioBGM.Refresh();
    }

    public string GetFieldBGMSong(int index)
    {
        index = Enum.GetNames(typeof(BGM)).Length + index < bgmList.Count ? //
            Enum.GetNames(typeof(BGM)).Length + index : bgmList.Count - 1;
        return $"{bgmList[index].song}";
    }
    public string GetFieldBGMArtist(int index)
    {
        index = Enum.GetNames(typeof(BGM)).Length + index < bgmList.Count ? //
            Enum.GetNames(typeof(BGM)).Length + index : bgmList.Count - 1;
        return $"{bgmList[index].artist}";
    }
    public int GetPitchRank()
    {
        return (int)((audioBGM.audio.pitch - 1.0f) / 0.05f);
    }

    [Serializable]
    private sealed class BGMData
    {
        [SerializeField] internal AudioClip clip;
        [SerializeField] internal string artist;
        [SerializeField] internal string song;
        [SerializeField, Range(0.0f, 1.0f)] internal float volume = 1.0f;
        [SerializeField] internal float loopStart;
        [SerializeField] internal float loopEnd;

        internal BGMData(FieldBGM f)
        {
            this.clip = Resources.Load<AudioClip>($"Audio/{f.artist} - {f.song.Replace("=", "_")}");
            this.artist = f.artist;
            this.song = f.song;
            this.volume = f.volume;
            this.loopStart = f.loop_start;
            this.loopEnd = f.loop_end;
        }
    }

    [Serializable]
    private sealed class SEData
    {
        [SerializeField] internal AudioClip clip;
        [SerializeField, Range(0.0f, 1.0f)] internal float volume = 1.0f;
    }

    private sealed class BGMAudio
    {
        internal AudioSource audio;
        private float bgmMaxVolume = 1.0f;
        private BGMData np;
        private bool isPlaying;

        internal void Refresh()
        {
            bgmMaxVolume = np.volume * SystemSettings.VolumeBGM / 100.0f;
            audio.volume = bgmMaxVolume;
        }

        internal void Play(BGMData data)
        {
            np = data;

            audio.volume = 0.0f;
            audio.clip = np.clip;
            bgmMaxVolume = np.volume * SystemSettings.VolumeBGM / 100.0f;
            audio.time = 0.0f;

            audio.Play();
            isPlaying = true;
            LoopCheck();
            FadeIn(30);
        }

        internal void SetPitch(float pitch)
        {
            audio.pitch = pitch;
        }

        internal async void FadeIn(int frame)
        {
            audio.pitch = 1.0f;
            for (int i = 1; i <= frame; i++)
            {
                audio.volume = bgmMaxVolume * i / frame;
                await UniTask.WaitForEndOfFrame();

            }
        }

        internal async void FadeOut(int frame)
        {
            for (int i = 1; i <= frame; i++)
            {
                audio.volume = bgmMaxVolume * (frame - i) / frame;
                await UniTask.WaitForEndOfFrame();
            }
            audio.Stop();
            isPlaying = false;
            audio.pitch = 1.0f;
        }

        private async void LoopCheck()
        {
            while (isPlaying)
            {
                //  Debug.Log($"{audio.time} {now.loopEnd}");
                if (audio.time >= np.loopEnd)
                {
                    audio.time = np.loopStart;
                    Debug.Log($"{audio.time} {np.loopEnd}");
                }
                await UniTask.Delay(10);
            }
        }
    }

    /** ******************************
     * フィールドBGM読み込み用クラス
     ** ******************************/
    [Serializable]
    public sealed class JFieldBGMList
    {
        public List<FieldBGM> recordsList = new List<FieldBGM>();

        [Serializable]
        public sealed class FieldBGM
        {
            public string artist;
            public string song;
            public float volume;
            public float loop_start;
            public float loop_end;
        }
    }
}

