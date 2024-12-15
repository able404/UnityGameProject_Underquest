using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentEnemyCount : MonoBehaviour
{
    public static CurrentEnemyCount instance;

    [Header("Enemy Count Settings")]
    public int CurrentCount = 0;

    public EnemyManager enemymanage;

    public bool isSpawn ;

    public bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    public void EnemyDamage()
    {
        if (CurrentCount > 0)
        {
            CurrentCount--;
            if (CurrentCount == 0)
            {
                enemymanage.SpawnNextWave();
            }
        }
    }
}
