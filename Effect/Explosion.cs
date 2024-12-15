using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    private AnimatorStateInfo info;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 현재 애니메이터 상태 정보 가져오기
        info = animator.GetCurrentAnimatorStateInfo(0);
        // 애니메이션이 완료되었는지 확인
        if (info.normalizedTime >= 0.9)
        {
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
