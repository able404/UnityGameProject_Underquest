using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_IdleState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;
    private float timer; 
    private System.Random random; // 랜덤 생성기

    public AI_IdleState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();
        this.timer = 0f;
        this.random = new System.Random();
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Idle State");
        anim.Play("Boss_Idle");
        timer = 0f;
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Idle State");
    }

    public void OnUpdate()
    {
        // 플레이어와 보스 거리 계산
        Vector2 bossPos = blackboard.owner.transform.position;
        Vector2 playerPos = PlayerController.instance.transform.position;
        float distance = Vector2.Distance(bossPos, playerPos);

        Vector2 direction = playerPos - bossPos;
        bool isPlayerLeftOrRight = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);

        fsm.FlipTo(PlayerController.instance.transform);

        // 거리 기반 상태 전환
        if (distance > 4.5f)
        {
            fsm.SwitchState(StateType.Move);
        }
        else if (distance <= 2.3f && isPlayerLeftOrRight)
        {
            fsm.SwitchState(StateType.Melee);
        }

        // 일정 시간마다 Shoot 상태로 전환 시도
        timer += Time.deltaTime;
        if (timer >= 0.6f)
        {
            timer = 0f; // 重置计时器
            int chance = random.Next(0, 100);
            if (chance < 60) // 60% 확률로 Shoot 상태
            {
                fsm.SwitchState(StateType.Shoot);
            }
        }
    }
}