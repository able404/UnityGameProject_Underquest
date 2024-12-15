using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public static VendingMachine instance;

    [Header("Configuration")]
    public GameObject[] merchandise;

    [Header("Purchase Settings")]
    public int initialPrice = 3; // 초기 가격
    private int currentPrice;    // 현재 가격

    private bool canBuy;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canBuy = false;
        currentPrice = initialPrice; // 현재 가격을 초기 가격으로 설정
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canBuy && !PauseMenu.instance.isPaused)
        {
            RandomItemShop();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canBuy = true;
            UIController.instance.InteractionPromptText.gameObject.SetActive(true);
            UIController.instance.CoinShortagePromptText.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canBuy = false;
            UIController.instance.InteractionPromptText.gameObject.SetActive(false);
        }
    }

    public void RandomItemShop()
    {
        if (LevelManager.instance.coinsCollected >= currentPrice)
        {
            LevelManager.instance.coinsCollected -= currentPrice;
            UIController.instance.UpdateCoinCount();

            currentPrice++;

            // 상품 배열이 비어있지 않은지 확인
            if (merchandise.Length > 0)
            {
                int merchandiseID = Random.Range(0, merchandise.Length);
                Instantiate(merchandise[merchandiseID], transform.position, Quaternion.identity);

                if (Pickup.instance != null)
                {
                    Pickup.instance.merchandiseID = merchandiseID;
                }
            }
            else
            {
                Debug.LogWarning("No merchandise available in the vending machine.");
            }
        }
        else
        {
            StartCoroutine(DisableCoinShortagePrompt());
        }
    }

    private IEnumerator DisableCoinShortagePrompt()
    {
        UIController.instance.CoinShortagePromptText.gameObject.SetActive(true);
        UIController.instance.InteractionPromptText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        UIController.instance.CoinShortagePromptText.gameObject.SetActive(false);
    }
}
