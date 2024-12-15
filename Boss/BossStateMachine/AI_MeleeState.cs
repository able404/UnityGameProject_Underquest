using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_MeleeState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;
    private GameObject meleeArea;

    private float meleeDuration = 0.42f; // 애니메이션 총 재생 시간
    private float activateTime = 0.35f; // 콜라이더 활성화 시점
    private float timer;

    public AI_MeleeState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();
        this.meleeArea = blackboard.owner.transform.Find("Melee Area").gameObject;
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Melee State");
        anim.Play("Boss_Melee"); // 播放近战动画

        timer = 0;
        meleeArea.SetActive(false);
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Melee State");

        meleeArea.SetActive(false);
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        // 정 시간에 콜라이더 활성화
        if (timer >= activateTime && timer < meleeDuration)
        {
            meleeArea.SetActive(true);
        }

        // 애니메이션 종료 시 콜라이더 비활성화 및 Idle로 복귀
        if (timer >= meleeDuration)
        {
            meleeArea.SetActive(false);

            fsm.SwitchState(StateType.Idle); // 返回 Idle 状态
        }
    }
}

