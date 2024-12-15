using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Gun
{
    [Header("Rocket Launcher Settings")]
    public int rocketNum;
    public float rocketAngle; // 로켓 간 각도 편차

    protected override void Fire()
    {
        animator.SetTrigger("Shoot");
        AudioManager.instance.PlaySFX(11);
        StartCoroutine(DelayFire(.2f)); // 일정 시간 후 로켓 발사
    }

    IEnumerator DelayFire(float delay)
    {
        yield return new WaitForSeconds(delay);

        int median = rocketNum / 2; 
        for (int i = 0; i < rocketNum; i++)
        {
            // 오브젝트 풀에서 로켓 가져오기
            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab); 
            bullet.transform.position = muzzlePos.position;

            // 로켓 수와 각도 편차를 바탕으로 각 로켓의 발사 각도 계산
            if (rocketNum % 2 == 1)
            {
                bullet.transform.right = Quaternion.AngleAxis(rocketAngle * (i - median), Vector3.forward) * direction;
            }
            else
            {
                bullet.transform.right = Quaternion.AngleAxis(rocketAngle * (i - median) + rocketAngle / 2, Vector3.forward) * direction;
            }

            bullet.GetComponent<Rocket>().SetTarget(mousePos);
        }
    }
}
