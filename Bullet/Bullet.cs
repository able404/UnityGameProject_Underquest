using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed;
    public float damage;

    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // 폭발 효과 프리팹
    private Rigidbody2D theRB;

    void Awake()
    {
        theRB = GetComponent<Rigidbody2D>();
    }

    // 탄환 속도와 방향 설정
    public void SetSpeed(Vector2 direction)
    {
        theRB.velocity = direction * speed;
    }

    // 탄환이 다른 콜라이더와 접촉할 때 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Chest") || other.CompareTag("Machine") || other.CompareTag("Door_close") || other.CompareTag("Boss_Death"))
        {
            CreateExplosionEffect();
            ObjectPool.Instance.PushObject(gameObject);
        }
        else if(other.CompareTag("BulletMaxDistance"))
        {
            ObjectPool.Instance.PushObject(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyHealthController enemyHealth = other.GetComponent<EnemyHealthController>();
            EnemyController enemyController = other.GetComponent<EnemyController>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (enemyController != null)
            {
                enemyController.OnHitByPlayer();
            }

            CreateExplosionEffect();
            ObjectPool.Instance.PushObject(gameObject);
        }
        else if(other.CompareTag("Boss"))
        {
            BossHealthController bossHealth = other.GetComponent<BossHealthController>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            
            CreateExplosionEffect();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    private void CreateExplosionEffect()
    {
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        exp.transform.position = transform.position;
    }
}
