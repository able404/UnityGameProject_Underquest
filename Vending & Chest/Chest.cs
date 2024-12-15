using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    public GameObject[] reward;

    [Header("Reward Configuration")]
    public int minRewardCount = 6; // 최소 드롭 개수
    public int maxRewardCount = 11; // 최대 드롭 개수(상한 미포함)
    [Range(0f, 1f)]
    public float rareRewardChance = 0.05f; // 희귀 보상 드롭 확률

    private bool canOpen;
    private bool isOpened;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        isOpened = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canOpen && !isOpened && !PauseMenu.instance.isPaused)
        {
            OpenChest();
        }
    }

    // 보물상자를 여는 로직
    private void OpenChest()
    {
        anim.SetTrigger("isOpened");
        isOpened = true;

        // 보상 배열이 비어있다면 반환
        if (reward == null || reward.Length == 0)
        {
            Debug.LogWarning("No rewards assigned to the chest.");
            return;
        }

        // 랜덤한 개수의 주요 보상 생성
        int rewardNum = Random.Range(minRewardCount, maxRewardCount);
        for (int i = 0; i < rewardNum; i++)
        {
            if (reward.Length > 0 && reward[0] != null)
            {
                Instantiate(reward[0], transform.position, Quaternion.identity);
            }

            // 낮은 확률로 추가 보상 생성
            if (reward.Length > 1 && reward[1] != null && Random.value <= rareRewardChance)
            {
                Instantiate(reward[1], transform.position, Quaternion.identity);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canOpen = true;
            UIController.instance.InteractionPromptText.gameObject.SetActive(true);
            UIController.instance.CoinShortagePromptText.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canOpen = false;
            UIController.instance.InteractionPromptText.gameObject.SetActive(false);
        }
    }

    public void OnAnimationEnd() { return; }
}
