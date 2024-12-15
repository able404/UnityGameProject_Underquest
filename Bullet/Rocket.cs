using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float lerp; // 로켓 회전 보간 값
    public float speed;
    public int damage;

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    private Rigidbody2D theRB;

    private Vector3 targetPos;
    private Vector3 direction;
    private bool arrived; // 목표점 도착 여부 표시

    private void Awake()
    {
        theRB = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Vector2 _target)
    {
        arrived = false;
        targetPos = _target;
    }

    private void FixedUpdate()
    {
        direction = (targetPos - transform.position).normalized;

        if(!arrived)
        {
            float distance = Vector2.Distance(transform.position, targetPos);
            if (distance > 0f)
            {
                // Slerp를 사용해 목표 방향으로 부드럽게 회전
                transform.right = Vector3.Slerp(transform.right, direction, lerp / distance);
                theRB.velocity = transform.right * speed;
            }
        }

        // 목표까지의 거리가 1f 미만이고 아직 도착하지 않았다면 도착 처리
        if (Vector2.Distance(transform.position, targetPos) < 1f && !arrived)
        {
            arrived = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Chest") || other.CompareTag("Machine") || other.CompareTag("Door_close") || other.CompareTag("Boss_Death"))
        {
            HandleHitObstacle();
        }
        else if (other.CompareTag("BulletMaxDistance"))
        {
            // 최대 사거리 초과 시 로켓 회수
            ObjectPool.Instance.PushObject(gameObject);
            StopSfx(11);
        }
        else if (other.CompareTag("Enemy"))
        {
            HandleHitEnemy(other);
        }
        else if (other.CompareTag("Boss"))
        {
            HandleHitBoss(other);
        }
    }

    // 로켓이 장애물과 충돌했을 때 로직 처리
    private void HandleHitObstacle() 
    {
        CreateExplosionEffect();
        StopSfx(11);
        PlaySfx(12);

        theRB.velocity = Vector2.zero;
        StartCoroutine(Push(gameObject, 0.1f));
    }

    // 处理火箭击中敌人的逻辑
    private void HandleHitEnemy(Collider2D enemyCollider)
    {
        EnemyHealthController enemyHealth = enemyCollider.GetComponent<EnemyHealthController>();
        EnemyController enemyController = enemyCollider.GetComponent<EnemyController>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        if (enemyController != null)
        {
            enemyController.OnHitByPlayer();
        }

        CreateExplosionEffect();
        StopSfx(11);
        PlaySfx(12);

        theRB.velocity = Vector2.zero;
        StartCoroutine(Push(gameObject, 0.1f));
    }

    // 处理火箭击中Boss的逻辑
    private void HandleHitBoss(Collider2D bossCollider)
    {
        BossHealthController bossHealth = bossCollider.GetComponent<BossHealthController>();

        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damage);
        }

        CreateExplosionEffect();
        StopSfx(11);
        PlaySfx(12);

        theRB.velocity = Vector2.zero;
        StartCoroutine(Push(gameObject, 0.1f));
    }

    private void CreateExplosionEffect()
    {
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        exp.transform.position = transform.position;
    }

    IEnumerator Push(GameObject _object, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.PushObject(_object);
    }

    // 특정 인덱스의 사운드 이펙트 재생 중지
    private void StopSfx(int index)
    {
        if (AudioManager.instance != null &&
            AudioManager.instance.soundEffects != null &&
            index < AudioManager.instance.soundEffects.Length &&
            AudioManager.instance.soundEffects[index].isPlaying)
        {
            AudioManager.instance.soundEffects[index].Stop();
        }
    }

    // 특정 인덱스의 사운드 이펙트 재생
    private void PlaySfx(int index)
    {
        if (AudioManager.instance != null &&
            AudioManager.instance.soundEffects != null &&
            index < AudioManager.instance.soundEffects.Length)
        {
            AudioManager.instance.soundEffects[index].Play();
        }
    }
}
