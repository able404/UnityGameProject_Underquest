using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Gun
{
    private GameObject effect;
    private LineRenderer laser; 
    private bool isShooting;

    [Header("Laser Damage Settings")]
    public float damagePerSecond; // 초당 피해량
    public float damageInterval; // 피해 체크 간격
    private float damageTimer = 0f;

    private float maxShootingTime = 4.0f; // 최대 연속 사격 시간
    private float shootingTimer = 0f; // 현재 연속 사격 시간 기록

    protected override void Start()
    {
        base.Start();
        isShooting = false;

        laser = muzzlePos.GetComponent<LineRenderer>();
        effect = transform.Find("Laser_Effect").gameObject;
        effect.SetActive(false);
    }

    public override float GetCooldownProgress()
    {
        // 쿨다운 중이면 쿨다운 진행도 반환
        if (timer > 0f)
        {
            if (CooldownInterval <= 0f)
                return 1f;
            return timer / inserval;
        }
        else // 쿨다운이 아니라면 현재 사격 시간 / 최대 사격 시간 반환
        {
            if (maxShootingTime <= 0f)
                return 1f;
            return shootingTimer / maxShootingTime;
        }
    }

    protected override void Shoot()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.isPaused) return;

        // 마우스 위치와 방향 계산
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스 위치에 따라 무기 방향 전환
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(flipY, -flipY, 1);
        else
            transform.localScale = new Vector3(flipY, flipY, 1);

        // 사격 방향 계산 후 무기를 그 방향으로 회전
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;

        if (timer > 0)
        {
            // 쿨다운 중
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                // 쿨다운 종료 시 사격 타이머 리셋
                shootingTimer = 0f;
            }

            // 사격 중이라면 사격 중지
            if (isShooting)
            {
                StopShooting();
            }
        }
        else
        {
            // 쿨다운 아님 -> 사격 가능
            if (Input.GetButton("Fire1"))
            {
                if (shootingTimer < maxShootingTime)
                {
                    // 사격 중이 아니라면 사격 시작
                    if (!isShooting)
                    {
                        StartShooting();
                    }

                    shootingTimer += Time.deltaTime;
                    Fire();
                }
                else
                {
                    // 최대 사격 시간 도달 시 쿨다운 시작
                    StopShooting();
                    timer = inserval;
                }
            }
            else
            {
                // 사격 버튼을 떼었고 사격 중이라면 중지
                if (isShooting)
                {
                    StopShooting();
                }
                // 사격 타이머는 유지, 리셋 없음
            }
        }
    }
    
    protected override void Fire()
    {
        if (laser == null || effect == null) return;

        RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direction, 30, ~(1 << 14 | 1 << 7 | 1 << 8 | 1 << 9 | 1<< 10 | 1 << 15)); 

        laser.SetPosition(0, muzzlePos.position); 
       
        if (hit2D.collider != null)
        {
            // 레이저 명중 시 끝점 = 히트 포인트
            laser.SetPosition(1, hit2D.point);
            effect.transform.position = hit2D.point;
            effect.transform.forward = -direction;

            // 적이나 보스 명중 시 damageInterval 주기로 피해 적용
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                if (hit2D.collider.CompareTag("Enemy"))
                {
                    DamageEnemy(hit2D.collider.gameObject);
                }
                else if (hit2D.collider.CompareTag("Boss"))
                {
                    DamageBoss(hit2D.collider.gameObject);
                }
                damageTimer = 0f;
            }
        }
        else
        {
            // 히트 대상 없음 -> 레이저 끝점 = 전방 30유닛
            Vector2 endPos = (Vector2)muzzlePos.position + direction.normalized * 30;
            laser.SetPosition(1, endPos);
            effect.transform.position = endPos;
            effect.transform.forward = -direction;
        }
    }

    private void DamageEnemy(GameObject enemy)
    {
        EnemyHealthController enemyHealth = enemy.GetComponent<EnemyHealthController>();
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyHealth != null)
        {
            float damageAmount = damagePerSecond * damageInterval;
            enemyHealth.TakeDamage(damageAmount);
        }

        if (enemyController != null)
        {
            enemyController.OnHitByPlayer();
        }
    }

    private void DamageBoss(GameObject boss)
    {
        BossHealthController bossHealth = boss.GetComponent<BossHealthController>();

        if (bossHealth != null)
        {
            float damageAmount = damagePerSecond * damageInterval;
            bossHealth.TakeDamage(damageAmount);
        }
    }

    private void StartShooting()
    {
        isShooting = true;
        if (laser != null) laser.enabled = true;
        if (effect != null) effect.SetActive(true);
        if (animator != null) animator.SetBool("Shoot", true);
        if (AudioManager.instance != null && AudioManager.instance.soundEffects.Length > 10)
        {
            AudioManager.instance.soundEffects[10].Play();
        }
    }

    private void StopShooting()
    {
        isShooting = false;
        if (laser != null) laser.enabled = false;
        if (effect != null) effect.SetActive(false);
        if (animator != null) animator.SetBool("Shoot", false);
        if (AudioManager.instance != null) AudioManager.instance.StopSFX(10);
    }
}
