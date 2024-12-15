using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Health UI")]
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite heartFull;
    public Sprite heartEmpty;
    public Sprite heartHalf;

    [Header("Text UI")]
    public Text coinText;
    public Text timeText;
    public Text InteractionPromptText;
    public Text CoinShortagePromptText;

    [Header("Fade Screen")]
    public Image fadeScreen;
    public float fadeSpeed;
    private bool shouldFadeToBlack, shouldFadeFromBlack;

    [Header("Cooldown UI")]
    public Slider dashCooldown;
    public Slider gunsCooldown;

    [Header("Tutorial UI")]
    public GameObject gameTutorial;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // UI 초기화
        fadeScreen.color = new Color(0f, 0f, 0f, 1f);
        InteractionPromptText.gameObject.SetActive(false);
        CoinShortagePromptText.gameObject.SetActive(false);
        UpdateCoinCount();
        FadeFromBlack();

        dashCooldown.maxValue = PlayerController.instance.dashCooldown;
        dashCooldown.minValue = 0f;
        dashCooldown.value = 0f;

        // Initialize GunsCooldown slider with default values
        gunsCooldown.minValue = 0f;
        gunsCooldown.value = gunsCooldown.maxValue = 1f; // 시작 시 가득 찬 상태

        gameTutorial.SetActive(true);
    }

    void Update()
    {
        // 화면 페이드 처리
        if (shouldFadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));

            if (fadeScreen.color.a == 1f)
            {
                shouldFadeToBlack = false;
            }
        }

        if(shouldFadeFromBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

            if (fadeScreen.color.a == 0f)
            {
                shouldFadeFromBlack = false;
            }
        }
    }

    public void UpdateHealthDisplay()
    {
        // 血量ui
        switch (PlayerHealthController.instance.currentHealth)
        {
            case 6:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;

                break;

            case 5:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartHalf;

                break;

            case 4:
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartEmpty;

                break;

            case 3:
                heart1.sprite = heartFull;
                heart2.sprite = heartHalf;
                heart3.sprite = heartEmpty;

                break;

            case 2:
                heart1.sprite = heartFull;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;

                break;

            case 1:
                heart1.sprite = heartHalf;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;

                break;

            case 0:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;

                break;

            default:
                heart1.sprite = heartEmpty;
                heart2.sprite = heartEmpty;
                heart3.sprite = heartEmpty;

                break;
        }
    }

    public void UpdateCoinCount()
    {
        coinText.text = LevelManager.instance.coinsCollected.ToString();
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

    public void UpdateDashCooldown(float cooldownTime)
    {
        dashCooldown.value = cooldownTime;
    }

    public void UpdateGunCooldown(float progress)
    {
        gunsCooldown.value = progress;
    }

    public void CloseTutorial()
    {
        gameTutorial.SetActive(false);
    }

    public void ShowTutorial()
    {
        gameTutorial.SetActive(true);
    }
}
