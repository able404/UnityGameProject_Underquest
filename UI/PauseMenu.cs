using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [Header("UI Components")]
    public GameObject pauseScreen;

    [Header("Pause State")]
    public bool isPaused;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        if(isPaused)  // 일시정지 해제
        {
            isPaused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;

            AudioManager.instance.UnPauseAllSFX();
        }
        else          // 일시정지
        {
            isPaused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;

            AudioManager.instance.PauseAllSFX();
        }
    }

    public void MainMenu()
    {
        CurrentEnemyCount.instance.isGameOver = true;
        StartCoroutine(LoadMainMenu()); 
    }

    // 메인 메뉴 씬 비동기 로드용 코루틴
    private IEnumerator LoadMainMenu()
    {
        Time.timeScale = 1f;
        PlayerController.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        UIController.instance.FadeToBlack();
        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlayMenuBGM();
        SceneManager.LoadScene(0);
    }
}
