using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun Settings")]
    public int bulletNum; // 동시에 발사할 탄환 수
    public float bulletAngle; // 동시에 발사할 탄환 수

    protected override void Fire()
    {
        animator.SetTrigger("Shoot");

        int median = bulletNum / 2; // 탄환을 중앙 기준으로 양옆으로 퍼뜨리기 위한 중간 인덱스 계산
        for (int i = 0; i < bulletNum; i++)
        {
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab); // 오브젝트 풀에서 탄환 가져오기
            bullet.transform.position = muzzlePos.position;

            // 탄환 순서에 따라 발사 각도 계산
            Vector2 shotDirection;
            if (bulletNum % 2 == 1)
            {
                // 탄환 수가 홀수이면 중간 탄환을 기준으로 양옆 대칭
                shotDirection = Quaternion.AngleAxis(bulletAngle * (i - median), Vector3.forward) * direction;
            }
            else
            {
                // 탄환 수가 짝수이면 반 각도를 offset으로 주어 대칭 맞추기
                shotDirection = Quaternion.AngleAxis(bulletAngle * (i - median) + bulletAngle / 2, Vector3.forward) * direction;
            }

            // 탄환 속도 및 방향 설정
            bullet.GetComponent<Bullet>().SetSpeed(shotDirection);
        }

        AudioManager.instance.PlaySFX(6);

        // 오브젝트 풀에서 탄피 가져오기
        if (shellPos != null)
        {
            GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
            if (shell != null)
            {
                shell.transform.position = shellPos.position;
                shell.transform.rotation = shellPos.rotation;
            }
        }
    }
}
