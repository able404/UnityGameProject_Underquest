using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{
    [Header("Rifle Settings")]
    private GameObject effect;
    private float effectDuration = .1f;
    public float damage;

    protected override void Start()
    {
        base.Start();

        effect = transform.Find("Rifle_Effect").gameObject;
        effect.SetActive(false);
    }

    protected override void Fire()
    {
        animator.SetTrigger("Shoot");

        // 광선 검출: 총구 위치에서 사격 방향으로 길이 30의 레이 캐스트
        RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direction, 30, ~(1 << 14 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 15));

        AudioManager.instance.PlaySFX(7);

        // 오브젝트 풀에서 탄환 궤적(LineRenderer) 가져오기
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        if (bullet != null)
        {
            LineRenderer tracer = bullet.GetComponent<LineRenderer>();
            if (tracer != null)
            {
                tracer.SetPosition(0, muzzlePos.position);
                // 레이캐스트가 물체에 명중하면 충돌 지점, 아니면 최대 사거리 방향의 점을 설정
                Vector3 endPoint = hit2D.collider != null ? (Vector3)hit2D.point : muzzlePos.position + (Vector3)direction * 30f;
                tracer.SetPosition(1, endPoint);
            }
        }

        // 오브젝트 풀에서 탄피 가져오기
        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
        if (shell != null && shellPos != null)
        {
            shell.transform.position = shellPos.position;
            shell.transform.rotation = shellPos.rotation;
        }

        if (hit2D.collider != null)
        {
            effect.transform.position = hit2D.point;
            StartCoroutine(ShowHitEffect());

            if (hit2D.collider.CompareTag("Enemy"))
            {
                EnemyHealthController enemyHealth = hit2D.collider.GetComponent<EnemyHealthController>();
                EnemyController enemyController = hit2D.collider.GetComponent<EnemyController>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }

                if (enemyController != null)
                {
                    enemyController.OnHitByPlayer();
                }
            }
            else if (hit2D.collider.CompareTag("Boss"))
            {
                BossHealthController bossHealth = hit2D.collider.GetComponent<BossHealthController>();

                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(damage);
                }
            }
        }
    }

    private IEnumerator ShowHitEffect()
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        effect.SetActive(false);
    }
}
