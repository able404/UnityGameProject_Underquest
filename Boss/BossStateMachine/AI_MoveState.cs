using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI_MoveState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private float timer; // 랜덤 상태 전환 타이머
    private System.Random random;

    public AI_MoveState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.timer = 0f;
        this.random = new System.Random();
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Move State");
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Move State");
        blackboard.owner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void OnUpdate()
    {
        Vector2 bossPos = blackboard.owner.transform.position;
        Vector2 playerPos = PlayerController.instance.transform.position;
        float distance = Vector2.Distance(bossPos, playerPos);

        fsm.FlipTo(PlayerController.instance.transform);

        // 거리 가깝다면 Idle로
        if (distance <= 4.5f)
        {
            fsm.SwitchState(StateType.Idle);
            return;
        }

        // 거리가 멀 때 일정 확률로 다른 상태 전환
        if (distance > 5.0f)
        {
            timer += Time.deltaTime;
            if (timer >= 0.6f)
            {
                timer = 0f;
                int chance = random.Next(0, 100);
                if (chance < 60)  // 60%
                {
                    fsm.SwitchState(StateType.LaserCast);
                    return;
                }
                else if (chance >= 60 && chance < 90)  // 30%
                {
                    fsm.SwitchState(StateType.Shoot);
                    return;
                }
            }
        }

        // 그렇지 않다면 계속 플레이어쪽으로 이동
        Vector2 direction = (playerPos - bossPos).normalized;
        blackboard.owner.transform.position += (Vector3)direction * blackboard.moveSpeed * Time.deltaTime;
    }
}
