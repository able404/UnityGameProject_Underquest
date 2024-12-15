using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_DeathState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;

    public AI_DeathState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Death State");

        // 사망 애니메이션 재생
        anim.Play("Boss_Death");

        // 이동 정지
        if (blackboard.owner.TryGetComponent(out Rigidbody2D theRB))
        {
            theRB.velocity = Vector2.zero;
        }
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Death State");
    }

    public void OnUpdate()
    {
        // 사망 애니메이션 재생 완료 여부 확인
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Boss_Death") && stateInfo.normalizedTime >= 1.0f)
        {
            Transform deathCollider = blackboard.owner.transform.Find("Boss Death Collider");

            if (deathCollider != null)
            {
                deathCollider.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Boss Death Collider not found!");
            }
        }
    }
}
