using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMap : MonoBehaviour
{
    GameObject mapSprite;

    private void OnEnable()
    {
        // 레이어 설정
        gameObject.layer = 14;

        // 부모 오브젝트의 첫번째 자식 오브젝트를 맵 스프라이트로 사용
        mapSprite = transform.parent.GetChild(0).gameObject;
        mapSprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            mapSprite.SetActive(true);
        }
    }
}
