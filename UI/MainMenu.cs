using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Animator chestAnim;
    public Image fadeScreen;
    public GameObject settings; // 설정 메뉴 패널
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Fade Settings")]
    public float fadeSpeed;
    private bool shouldFadeToBlack;
    private bool shouldFadeFromBlack;

    void Start()
    {
        if (fadeScreen == null)
        {
            Debug.LogWarning("fadeScreen is not assigned in the inspector.");
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        else
        {
            Debug.LogWarning("bgmSlider is not assigned in the inspector.");
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        else
        {
            Debug.LogWarning("sfxSlider is not assigned in the inspector.");
        }

        InitializeVolumeSettings();
        InitializeFade();
    }

    void Update()
    {
        HandleFade();
    }

    // 볼륨 설정 초기화
    private void InitializeVolumeSettings()
    {
        if (AudioManager.instance != null)
        {
            float lastBGMVolume = AudioManager.instance.GetBGMVolume();
            float lastSFXVolume = AudioManager.instance.GetSFXVolume();

            if (bgmSlider != null) bgmSlider.value = lastBGMVolume;
            if (sfxSlider != null) sfxSlider.value = lastSFXVolume;
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found, cannot initialize volumes.");
        }
    }

    // 페이드 스크린 초기화
    private void InitializeFade()
    {
        if (fadeScreen != null)
        {
            fadeScreen.color = new Color(0f, 0f, 0f, 1f);
            FadeFromBlack();
        }
    }

    // 매 프레임 페이드 업데이트
    private void HandleFade()
    {
        if (fadeScreen == null) return;

        if (shouldFadeToBlack)
        {
            float newAlpha = Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, newAlpha);

            if (Mathf.Approximately(fadeScreen.color.a, 1f))
            {
                shouldFadeToBlack = false;
            }
        }
        else if (shouldFadeFromBlack)
        {
            float newAlpha = Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, newAlpha);

            if (Mathf.Approximately(fadeScreen.color.a, 0f))
            {
                shouldFadeFromBlack = false;
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine(LoadGame());

        // 보물상자 오픈 애니메이션
        chestAnim.SetTrigger("isOpened"); // -> OnAnimationEnd()
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.1f);
        FadeToBlack();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
    }

    public void OnBGMVolumeChanged(float value)
    {
        AudioManager.instance.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    public void OnAnimationEnd()
    {
        ObjectPool.Instance.SavePool();
        AudioManager.instance.PlayGameBGM();
        SceneManager.LoadScene(1); // 게임 씬으로 전환
    }

    public void FadeToBlack()
    {
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack()
    {
        shouldFadeToBlack = false;
        shouldFadeFromBlack = true;
    }
}
