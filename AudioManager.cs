using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource[] soundEffects;
    public AudioSource[] bgm;

    [Header("Mixer & Parameters")]
    public AudioMixer mixer;
    [Tooltip("Exposed parameter name for Music volume in the AudioMixer.")]
    public string musicParameterName = "Music";
    [Tooltip("Exposed parameter name for SFX volume in the AudioMixer.")]
    public string sfxParameterName = "SFX";

    private float currentBGMVolume;
    private float currentSFXVolume;

    private void Awake()
    {
        if(instance == null) // 싱글톤 설정, 씬 전환 시 오브젝트 파괴 방지
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBGMVolume(float value)
    {
        if (mixer != null && !string.IsNullOrEmpty(musicParameterName))
        {
            mixer.SetFloat(musicParameterName, value);
            currentBGMVolume = value;
        }
        else
        {
            Debug.LogWarning("Mixer or Music parameter not set correctly.");
        }
    }

    public float GetBGMVolume()
    {
        return currentBGMVolume;
    }

    public void SetSFXVolume(float value)
    {
        if (mixer != null && !string.IsNullOrEmpty(sfxParameterName))
        {
            mixer.SetFloat(sfxParameterName, value);
            currentSFXVolume = value;
        }
        else
        {
            Debug.LogWarning("Mixer or SFX parameter not set correctly.");
        }
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    public void PlaySFX(int soundToPlay)
    {
        if (soundEffects == null || soundToPlay < 0 || soundToPlay >= soundEffects.Length) return;

        var sfx = soundEffects[soundToPlay];
        sfx.Stop();
        sfx.pitch = Random.Range(0.9f, 1.1f);
        sfx.Play();
    }

    public void StopSFX(int soundToStop)
    {
        if (soundEffects == null || soundToStop < 0 || soundToStop >= soundEffects.Length) return;
        soundEffects[soundToStop].Stop();
    }

    public void PauseAllSFX()
    {
        if (soundEffects == null) return;
        foreach (var sfx in soundEffects)
        {
            if (sfx.isPlaying)
            {
                sfx.Pause();
            }
        }
    }

    public void UnPauseAllSFX()
    {
        if (soundEffects == null) return;
        foreach (var sfx in soundEffects)
        {
            sfx.UnPause();
        }
    }

    public void PlayMenuBGM()
    {
        if (bgm == null || bgm.Length < 2) return;

        if (!bgm[1].isPlaying)
        {
            bgm[0].Stop();
            bgm[1].Play();
        }
    }

    public void PlayGameBGM()
    {
        if (bgm == null || bgm.Length < 2) return;

        if (!bgm[0].isPlaying)
        {
            bgm[1].Stop();
            bgm[0].Play();
        }
    }
}
