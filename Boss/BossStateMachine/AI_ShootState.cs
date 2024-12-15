using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_ShootState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;
    private bool hasShot = false;
    private float shootTime = 0.48f; // 투사체 발사 타이밍
    private GameObject armProjectilePrefab;
    private Transform armProjectilePos;
    private Transform playerTransform;

    public AI_ShootState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();

        this.armProjectilePrefab = blackboard.armProjectilePrefab;
        this.armProjectilePos = blackboard.armProjectilePos;
        this.playerTransform = PlayerController.instance.transform;
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering Shoot State");
        anim.Play("Boss_Shoot");
        hasShot = false; // 重置发射标志
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting Shoot State");
    }

    public void OnUpdate()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Boss_Shoot"))
        {
            float normalizedTime = stateInfo.normalizedTime;

            // 발사 타이밍에 투사체 발사
            if (!hasShot && normalizedTime >= shootTime / 0.55f)
            {
                hasShot = true;

                Vector3 targetPosition = playerTransform.position;

                GameObject projectile = GameObject.Instantiate(
                    armProjectilePrefab,
                    armProjectilePos.position,
                    Quaternion.identity
                );

                ArmProjectileController.instance.Initialize(targetPosition);
            }

            // 애니메이션 종료 시 Idle로 복귀
            if (normalizedTime >= 1.0f)
            {
                AudioManager.instance.PlaySFX(9);

                fsm.SwitchState(StateType.Idle);
            }
        }
    }
}
