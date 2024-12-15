using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [Header("Tracer Settings")]
    public float fadeSpeed;

    private LineRenderer line;
    private float alpha;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        alpha = line.endColor.a;
    }

    private void OnEnable()
    {
        line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, alpha);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        // 매 프레임 endColor의 알파를 감소시켜 0이 될 때까지
        while (line != null && line.endColor.a > 0)
        {
            float newAlpha = line.endColor.a - fadeSpeed;
            if (newAlpha < 0f) newAlpha = 0f;
            line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, newAlpha);
            yield return new WaitForFixedUpdate();
        }

        // 페이드 아웃 완료 후 오브젝트 풀로 반환
        ObjectPool.Instance.PushObject(gameObject);
    }
}
