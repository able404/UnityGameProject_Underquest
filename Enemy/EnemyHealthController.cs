using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public static EnemyHealthController instance;

    [Header("Health Settings")]
    public float currentHealth, maxHealth;

    [Header("Death Settings")]
    public GameObject deathEffect;
    private bool isDead = false;

    [Header("Drop Settings")]
    public GameObject collectible; 
    public float chanceToDrop; // 아이템 드롭 확률

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;

            // 적 사망 후 오브젝트 비활성화
            gameObject.SetActive(false);

            Instantiate(deathEffect, transform.position, transform.rotation);

            float dropSelect = Random.Range(0, 10f);
            if(dropSelect <= chanceToDrop)
            {
                Instantiate(collectible, transform.position, transform.rotation);
            }

            AudioManager.instance.PlaySFX(4);
        }
    }

    private void OnDisable()
    {
        if(!CurrentEnemyCount.instance.isGameOver)
            CurrentEnemyCount.instance.EnemyDamage();

        Destroy(gameObject);
    }
}
