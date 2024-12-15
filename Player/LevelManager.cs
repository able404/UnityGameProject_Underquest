using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Respawn Settings")]
    public float waitToRespawn; // 대기 후 리스폰 시간

    public int coinsCollected; // 획득한 코인 수

    private void Awake()
    {
        instance = this;
    }

    public void RespawnPlayer()
    {
        PlayerController.instance.gameObject.SetActive(false);
        StartCoroutine(RespawnCo());
    }

    public void NextLevel()
    {
        StartCoroutine(RespawnCo());
    }

    private IEnumerator RespawnCo()
    {
        // 플레이어 사망 후 FadeOut까지 대기
        yield return new WaitForSeconds(waitToRespawn - (1f / UIController.instance.fadeSpeed));
        UIController.instance.FadeToBlack();

        // 페이드아웃 후 약간 대기 후 페이드인
        yield return new WaitForSeconds((1f / UIController.instance.fadeSpeed) + .2f);
        UIController.instance.FadeFromBlack();

        ObjectPool.Instance.SavePool();
        AudioManager.instance.PlayGameBGM();
        SceneManager.LoadScene(1);
    }
}
