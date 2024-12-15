using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SleepState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;

    public AI_SleepState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Sleep State");
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Sleep State");
    }

    public void OnUpdate()
    {
        // 감지 범위 내 플레이어 있으면 보스 깨우기
        Vector2 bossPos = blackboard.owner.transform.position;
        Vector2 playerPos = PlayerController.instance.transform.position;

        Vector2 detectionCenter = bossPos;
        float halfWidth = blackboard.detectionWidth / 2f;
        float halfHeight = blackboard.detectionHeight / 2f;

        // 플레이어 범위 내 또는 보스 체력 감소 시 Awake
        if (playerPos.x >= detectionCenter.x - halfWidth &&
            playerPos.x <= detectionCenter.x + halfWidth &&
            playerPos.y >= detectionCenter.y - halfHeight &&
            playerPos.y <= detectionCenter.y + halfHeight)
        {
            fsm.SwitchState(StateType.Awake);
        }
        else if(BossHealthController.instance.currentHealth < BossHealthController.instance.maxHealth)
        {
            fsm.SwitchState(StateType.Awake);
        }
    }
}
