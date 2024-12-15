using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pickup : MonoBehaviour
{
    public static Pickup instance;

    [Header("Type Settings")]
    public bool isCoin;
    public bool isHeal;
    public bool isGun;

    [Header("Effects")]
    public GameObject pickupEffect;
    public float throwHeight; // 上抛高度
    public float throwDuration; // 上抛时间

    [Header("Pickup Settings")]
    public int      merchandiseID;
    public float    pickupDistance; // 拾取距离
    public float    moveSpeed;

    private bool isCollected;
    private bool canPickup = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ThrowItem();
    }

    private void Update()
    {
        MoveTowardsPlayerIfAllowed();
    }

    private void ThrowItem()
    {
        transform.DOJump(transform.position + new Vector3(Random.Range(-1f, 1f), -0.5f, 0), throwHeight, 1, throwDuration)
                .OnComplete(() =>
                {
                    canPickup = true;
                });
    }

    // 픽업 가능 시 플레이어에게 이동
    private void MoveTowardsPlayerIfAllowed()
    {
        if (canPickup && Vector2.Distance(transform.position, PlayerController.instance.transform.position) < pickupDistance)
        {
            Vector2 dir = (PlayerController.instance.transform.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected && canPickup)
        {
            if (isCoin)
            {
                CollectCoin();
            }
            else if (isHeal && PlayerHealthController.instance.currentHealth < PlayerHealthController.instance.maxHealth)
            {
                CollectHeal();
            }
            else if (isGun)
            {
                CollectGun();
            }
        }
    }

    // 코인 획득
    private void CollectCoin()
    {
        LevelManager.instance.coinsCollected++;
        isCollected = true;
        Destroy(gameObject);

        Instantiate(pickupEffect, transform.position, transform.rotation);
        UIController.instance.UpdateCoinCount();
        AudioManager.instance.PlaySFX(0);
    }

    // 힐 아이템 획득
    private void CollectHeal()
    {
        PlayerHealthController.instance.HealPlayer();
        isCollected = true;
        Destroy(gameObject);

        AudioManager.instance.PlaySFX(1);
    }

    // 총기 획득
    private void CollectGun()
    {
        PlayerController.instance.PickUpGun(merchandiseID);
        isCollected = true;
        Destroy(gameObject);

        AudioManager.instance.PlaySFX(8);
    }
}
