using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damageAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 적의 위치에 따라 넉백 방향 계산
            if (CompareTag("Boss") || CompareTag("Enemy"))
            {
                PlayerHealthController.instance.DealDamage(damageAmount, transform);
            }
        }
    }
}
