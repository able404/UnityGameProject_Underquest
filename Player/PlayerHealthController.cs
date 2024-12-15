using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    [Header("Health Settings")]
    public int currentHealth;
    public int maxHealth;

    [Header("Invincibility Settings")]
    public float invincibleLength; // 무적상태
    private float invincibleCounter;

    [Header("Visual & Effects")]
    public GameObject deathEffect;
    private SpriteRenderer theSR;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        theSR = GetComponent<SpriteRenderer>();     
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        HandleInvincibility();
    }

    //무적상태 타이머 처리
    private void HandleInvincibility()
    {
        if (invincibleCounter > 0)
        {
            invincibleCounter -= Time.deltaTime;

            if (invincibleCounter <= 0)
            {
                EndInvincibility();
            }
        }
    }

    public void DealDamage(int damageAmount, Transform attackerTransform)
    {
        if (invincibleCounter <= 0)
        {
            currentHealth -= damageAmount;
            AudioManager.instance.PlaySFX(2);
            CameraShakeManager.instance.CameraShake(impulseSource);

            if (currentHealth <= 0)
            {
                PlayerDeath();
            }
            else
            {
                StartInvincibility(invincibleLength);
                SetPlayerAlpha(0.3f);
                PlayerController.instance.KnockBack(attackerTransform);
            }
        }

        UIController.instance.UpdateHealthDisplay();
    }

    // 플레이어 사망 처리
    private void PlayerDeath()
    {
        currentHealth = 0;
        Instantiate(deathEffect, transform.position, transform.rotation);
        AudioManager.instance.PlaySFX(3); // 사망 사운드

        CurrentEnemyCount.instance.isGameOver = true;
        LevelManager.instance.RespawnPlayer();
    }

    public void HealPlayer()
    {
        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
        UIController.instance.UpdateHealthDisplay();
    }

    public void StartInvincibility(float duration)
    {
        invincibleCounter = duration;
        SetPlayerAlpha(0.3f);
    }

    private void EndInvincibility()
    {
        invincibleCounter = 0;
        SetPlayerAlpha(1f);
    }

    // 플레이어 스프라이트 투명도 조절
    private void SetPlayerAlpha(float alpha)
    {
        Color c = theSR.color;
        c.a = alpha;
        theSR.color = c;
    }
}
