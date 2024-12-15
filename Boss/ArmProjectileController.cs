using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmProjectileController : MonoBehaviour
{
    public static ArmProjectileController instance;

    private Vector3 direction; // 투사체 비행 방향
    public float speed = 10f;  // 투사체 비행 속도

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 투사체 방향으로 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        StartCoroutine(DestroyAfterTime());
    }

    public void Initialize(Vector3 targetPos)
    {
        // 목표 방향 벡터 정규화
        direction = (targetPos - transform.position).normalized;

        // 투사체가 바라볼 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 투사체 회전 설정
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(1.3f);
        Destroy(gameObject);
    }
}
