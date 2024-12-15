using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomarea : MonoBehaviour
{
    [Header("Managers")]
    public EnemyManager enemyManager;
    public RoomSetup roomSetup;

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어가 트리거 범위 내에 있을 때
        if (other.CompareTag("Player"))
        {
            // 부모 오브젝트 태그에 따라 행동 결정
            if (transform.parent.tag == "EnemyRoom")
            {
                enemyManager.SpawnEnemy();
            }
            else if (transform.parent.tag == "FinalRoom")
            {
                enemyManager.UpdateBossDoorState();
            }

            roomSetup.SetBulletMaxCollider(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomSetup.SetBulletMaxCollider(false);
        }
    }
}
