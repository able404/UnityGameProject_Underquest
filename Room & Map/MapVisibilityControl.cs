using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapVisibilityControl : MonoBehaviour
{
    [Header("Map Settings")]
    public RawImage mapImage;
    public float fadeSpeed; // 페이드 인아웃 속도

    private float targetAlpha = 0f; // 목표 알파값

    void Start()
    {
        SetMapAlpha(0f);
    }

    void Update()
    {
        // 스페이스 키를 누르고 있으면 지도 표시(알파=1), 아니면 숨김(알파=0)
        targetAlpha = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        // 현재 알파값을 목표 알파값으로 부드럽게 보간
        float currentAlpha = mapImage.color.a;
        float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        SetMapAlpha(newAlpha);
    }

    void SetMapAlpha(float alpha)
    {
        Color color = mapImage.color;
        color.a = alpha;
        mapImage.color = color;
    }
}
