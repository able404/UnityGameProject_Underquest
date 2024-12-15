using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    [Header("Shell Settings")]
    public float speed;
    public float stopTime = .5f;
    public float fadeSpeed = .01f;

    private Rigidbody2D theRB;
    private SpriteRenderer theSR;

    void Awake()
    {
        theRB = GetComponent<Rigidbody2D>();
        theSR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 탄피에 초기 랜덤 각도로 위 방향 속도를 부여하여 튕겨나가는 효과
        float angle = Random.Range(-30f, 30f); 
        theRB.velocity = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * speed;

        // 투명도 및 중력 값 리셋
        theSR.color = new Color(theSR.color.r, theSR.color.g, theSR.color.b, 1);
        theRB.gravityScale = 3;

        StartCoroutine(Stop()); // 일정 시간 후 탄피 정지
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(stopTime); // stopTime초 대기

        // 탄피 운동 정지 및 중력 해제
        theRB.velocity = Vector2.zero;       
        theRB.gravityScale = 0;

        // 알파를 줄여가며 페이드아웃
        while (theSR.color.a > 0)
        {
            float newAlpha = theSR.color.a - fadeSpeed;
            if (newAlpha < 0f) newAlpha = 0f;
            theSR.color = new Color(theSR.color.r, theSR.color.g, theSR.color.b, newAlpha);
            yield return new WaitForFixedUpdate();
        }

        // 오브젝트 풀로 반환
        ObjectPool.Instance.PushObject(gameObject);
    }
}
