using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthController : MonoBehaviour
{
    public static BossHealthController instance;

    [Header("Health Settings")]
    public float currentHealth;
    public float maxHealth;

    private bool isDead = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;

            BossAI bossAI = GetComponent<BossAI>();
            if (bossAI != null)
            {
                BossManager.instance.bossDeath = true;
                BossManager.instance.bossAwake = false;

                // 사망 상태 전환
                bossAI.EnterDeathState();
            }

            LevelManager.instance.NextLevel();
        }
    }
}
