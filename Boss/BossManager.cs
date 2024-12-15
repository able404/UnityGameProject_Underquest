using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager instance;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform spawnPoint;

    [Header("Boss State")]
    public bool bossAwake = false;
    public bool bossDeath = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SpawnBoss();

        bossAwake = false;
        bossDeath = false;
    }

    public void SpawnBoss()
    {
        if (transform.parent.tag == "FinalRoom")
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
