using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class SoundManager : MonoBehaviour {

    #region SINGETONE
    public static SoundManager instance;
    #endregion

    #region CLASS
    [System.Serializable]
    public class NAME_AUDIOCLIP
    {
        public Global.E_SOUND name;
        public AudioClip clip;
    }
    [System.Serializable]
    public class NAME_AUDIOCLIPS
    {
        public Global.E_SOUND name;
        public AudioClip[] clips;
    }
    #endregion

    #region VARIABLE
    [Range(0, 1f)]
    public float BGMVolume = 1f;
    [Range(0, 1f)]
    public float SFXVolume = 1f;
    public bool SFXMute = false;

    public int audioSourceCount = 10;
    [Header("Clips")]
    public NAME_AUDIOCLIP[] clips;
    //public NAME_AUDIOCLIP[] clips;
    public NAME_AUDIOCLIPS[] randomClips;

    private AudioSource[] audios;
    [HideInInspector]
    public AudioSource bgm;
    #endregion

    #region UNITY_METHOD
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audios = new AudioSource[audioSourceCount];
        //dic_randomClips = new Dictionary<Global.E_SOUND, AudioClip[]>();

        for(var i = 0; i < audioSourceCount; ++i)
        {
            audios[i] = gameObject.AddComponent<AudioSource>();
            audios[i].playOnAwake = false;
        }

        // for(var i = 0; i < randomClips.Length; ++i)
        // {
        //     dic_randomClips.Add(randomClips[i].name, randomClips[i].clips);
        // }
    }
    #endregion

    

    #region METHOD
    /// <summary>
    /// 클립 이름으로 사운드 재생
    /// </summary>
    /// <param name="clipName">클립 이름</param>
    /// <param name="isLoop">반복 재생 여부</param>
    /// <returns></returns>
    public AudioSource Play(Global.E_SOUND clipName, bool isLoop = false, float pitch = 1f, float volume = -1f)
    {
        if(!ObscuredPrefs.GetBool("SFX", true) || SFXMute)
        {
            return null;
        }

        var audio = GetNonPlayingAudioSource();

        if (audio != null)
        {
            for(var i = 0; i < clips.Length; i++)
            {
                if(clipName == clips[i].name)
                {
                    audio.volume = volume > 0 ? volume : SFXVolume;
                    audio.clip = clips[i].clip;
                    audio.loop = isLoop;
                    audio.pitch = pitch;
                    audio.Play();
                    break;
                }
            }
        }

        return audio;
    }

    public void Play(Global.E_SOUND clipName)
    {
        Play(clipName, false);
    }

    // public AudioSource PlayOnlyOnce(Global.E_SOUND clipName, bool isLoop = false)
    // {
    //     if(!ObscuredPrefs.GetBool("SFX", true) || SFXMute)
    //     {
    //         return null;
    //     }
    //
    //     var audio = GetNonPlayingAudioSource();
    //
    //     if (audio != null)
    //     {
    //         for(var i = 0; i < audioSourceCount; ++i)
    //         {
    //             if(audios[i].clip != null && audios[i].clip.name == clipName)
    //             {
    //                 if(!audios[i].isPlaying)
    //                     audios[i].Play();
    //                 return audios[i];
    //             }
    //         }
    //
    //         audio = Play(clipName, isLoop);
    //     }
    //
    //     return audio;
    // }

    public AudioSource PlayRandom(Global.E_SOUND randomClipName, bool isLoop = false)
    {
        if(!ObscuredPrefs.GetBool("SFX", true) || SFXMute)
        {
            return null;
        }

        var audio = GetNonPlayingAudioSource();

        if (audio != null)
        {
            for(var i = 0; i < randomClips.Length; i++)
            {
                if(randomClipName == randomClips[i].name)
                {
                    audio.volume = SFXVolume;
                    audio.clip = randomClips[i].clips[Random.Range(0, randomClips[i].clips.Length)];
                    audio.loop = isLoop;
                    audio.Play();
                    break;
                }
            }
        }

        return audio;
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="clipName">클립 이름</param>
    /// <param name="isLoop">반복 재생 여부</param>
    public void PlayBGM(Global.E_SOUND clipName)
    {
        if (bgm == null)
            bgm = GetNonPlayingAudioSource();

        if (bgm.isPlaying)
            bgm.Stop();

        for(var i = 0; i < clips.Length; i++)
        {
            if(clipName == clips[i].name)
            {
                bgm.volume = BGMVolume;
                bgm.clip = clips[i].clip;
                bgm.loop = true;
                bgm.Play();
                break;
            }
        }
    }

    public void PlayBGM(Global.E_SOUND clipName, float time = 0)
    {
        if (bgm == null)
            bgm = GetNonPlayingAudioSource();

        if (bgm.isPlaying)
            bgm.Stop();

        for(var i = 0; i < clips.Length; i++)
        {
            if(clipName == clips[i].name)
            {
                bgm.volume = BGMVolume;
                bgm.clip = clips[i].clip;
                bgm.loop = true;
                bgm.time = time;
                bgm.Play();
                break;
            }
        }
    }

    /// <summary>
    /// 배경음악 중지
    /// </summary>
    public void StopBGM()
    {
        if (bgm != null)
            bgm.Stop();
    }

    public void PauseBGM()
    {
        if(bgm != null)
        {
            bgm.Pause();
        }
    }

    public void ResumeBGM()
    {
        if(bgm != null)
        {
            bgm.Play();
        }
    }

    public void MuteBGM(bool mute)
    {
        if(bgm != null)
        {
            bgm.mute = mute;
        }
    }

    /// <summary>
    /// AudioSource 풀에서 현재 재생중이 아닌 AudioSource 가져오기
    /// </summary>
    /// <returns></returns>
    AudioSource GetNonPlayingAudioSource()
    {
        if (audios == null)
            return null;

        for (int i = 0; i < audioSourceCount; ++i)
        {
            if (!audios[i].isPlaying && bgm != audios[i])
            {
                return audios[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 클립 재생중지
    /// </summary>
    /// <param name="clipName"></param>
    public void Stop(string clipName)
    {
        for(var i = 0; i < audioSourceCount; ++i)
        {
            if(audios[i].clip.name == clipName)
            {
                audios[i].Stop();
                audios[i].clip = null;
                return;
            }
        }
    }

    /// <summary>
    /// AudioSource 중지
    /// </summary>
    /// <param name="audio"></param>
    public void Stop(AudioSource audio)
    {
        audio.Stop();
        audio.clip = null;
    }

    public void ChangeBGM(Global.E_SOUND clipName)
    {
        StartCoroutine(ChangeBGMCoroutine(clipName));
    }

    IEnumerator ChangeBGMCoroutine(Global.E_SOUND clipName)
    {
        float t = 0;
        while(t < 1f)
        {
            bgm.volume = Mathf.Lerp(BGMVolume, 0, t);
            t += Time.deltaTime;
            yield return null;
        }
        PlayBGM(clipName);
        t = 0;
        while(t < 1f)
        {
            bgm.volume = Mathf.Lerp(0, BGMVolume, t);
            t += Time.deltaTime;
            yield return null;
        }
        bgm.volume = BGMVolume;
    }
    #endregion
}
