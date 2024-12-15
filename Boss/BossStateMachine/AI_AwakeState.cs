using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AwakeState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;

    public AI_AwakeState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Awake State");

        BossManager.instance.bossAwake = true;

        anim.Play("Boss_Awake");
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Awake State");
    }

    public void OnUpdate()
    {
        // Awake 애니메이션 재생 완료 여부 확인
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Boss_Awake") && stateInfo.normalizedTime >= 1.0f)
        {
            // Idle 상태로 전환
            anim.Play("Boss_Idle");
            fsm.SwitchState(StateType.Idle);
        }
    }
}
