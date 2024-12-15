using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    [Header("Door Settings")]
    public GameObject door;
    public string doorOpenTag = "Door_open";
    public string doorCloseTag = "Door_close";
    public string doorOpenLayer = "Door_open";
    public string doorCloseLayer = "Door_close";

    [Header("Timer Settings")]
    public float initialTime = 30f;
    private float time;
    private bool isSpawn;

    private Text timeText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Can't find enemy spawn point");
            return;
        }

        time = initialTime;
        timeText = UIController.instance.timeText; // UIController에서 타이머 UI 텍스트 가져오기

        // 초기 상태를 문이 열린 상태로 설정
        SetAllDoorChildState(isOpen: true);
    }

    void Update()
    {
        if (isSpawn)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        // 타이머 감소, 0 이하로 내려가지 않도록 처리
        time = Mathf.Max(time - Time.deltaTime, 0);
        UpdateTimeText();

        // 보스가 깨어나지 않았고 타이머가 끝났으며 적이 없을 경우 문 개방
        if (!BossManager.instance.bossAwake && time <= 0 && CurrentEnemyCount.instance.CurrentCount <= 0)
        {
            SetAllDoorChildState(isOpen: true);
        }
    }

    private void UpdateTimeText()
    {
        timeText.text = time.ToString("F1"); // UI 타이머 텍스트 갱신
    }

    public void SpawnEnemy()
    {
        if (isSpawn) return; // 이미 스폰 중이면 무시

        // 문을 닫고 적 스폰 시작
        SetAllDoorChildState(isOpen: false);
        isSpawn = true;

        // 3~5마리 적 무작위 수
        int count = Random.Range(3, 6);
        CurrentEnemyCount.instance.CurrentCount = count;
        CurrentEnemyCount.instance.enemymanage = this;

        for (int i = 0; i < count; i++)
        {
            // 무작위 스폰 지점과 적 종류 선택
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint.position, Quaternion.identity);
        }
    }

    public void SpawnNextWave()
    {
        if (time <= 0) return;
        isSpawn = false;
        SpawnEnemy();
    }

    public void UpdateBossDoorState()
    {
        if (BossManager.instance.bossAwake)
        {
            // 보스 각성 시 문 닫기
            SetAllDoorChildState(isOpen: false);
        }
        else if (BossManager.instance.bossDeath)
        {
            // 보스 사망 시 문 열기
            SetAllDoorChildState(isOpen: true);
        }
    }

    // 문의 모든 자식 오브젝트 상태(열림/닫힘) 설정
    private void SetAllDoorChildState(bool isOpen)
    {
        // 열림/닫힘 상태에 따른 태그와 레이어 선택
        string currentTag = isOpen ? doorOpenTag : doorCloseTag;
        string currentLayer = isOpen ? doorOpenLayer : doorCloseLayer;
        bool isTrigger = isOpen;
        bool showSprite = !isOpen;

        int currentChildCount = door.transform.childCount;

        for (int i = 0; i < currentChildCount; i++)
        {
            GameObject doorChild = door.transform.GetChild(i).gameObject;
            SetDoorState(doorChild, currentTag, currentLayer, isTrigger, showSprite);
        }
    }

    // 문 자식 오브젝트의 태그, 레이어, 콜라이더, 스프라이트 상태 설정
    private void SetDoorState(GameObject doorChild, string tagName, string layerName, bool isTrigger, bool showSprite)
    {
        var spriteRenderer = doorChild.GetComponent<SpriteRenderer>();
        var collider = doorChild.GetComponent<BoxCollider2D>();

        // 스프라이트 표시 상태 설정
        spriteRenderer.enabled = showSprite;
        // 콜라이더 트리거 설정
        collider.isTrigger = isTrigger;

        doorChild.tag = tagName;
        doorChild.layer = LayerMask.NameToLayer(layerName);
    }
}
