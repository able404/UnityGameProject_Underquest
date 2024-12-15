using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LaserCastState : IState
{
    private BossBlackboard blackboard;
    private FSM fsm;
    private Animator anim;
    private float laserDuration = 1.3f; // 레이저 지속 시간
    private float laserTimer;
    private GameObject laserInstance;
    private Vector2 targetPosition; // 플레이어 위치 기록

    private GameObject laserPrefab;
    private Transform laserPos;
    private Transform playerTransform;

    public AI_LaserCastState(FSM fsm)
    {
        this.fsm = fsm;
        this.blackboard = fsm.blackboard as BossBlackboard;
        this.anim = blackboard.owner.GetComponent<Animator>();
        this.laserTimer = 0f;

        this.laserPrefab = blackboard.laserPrefab;
        this.laserPos = blackboard.laserPos;
        this.playerTransform = PlayerController.instance.transform;
    }

    public void OnEnter()
    {
        Debug.Log("Boss Entering LaserCast State");
        
        laserTimer = 0f;
        anim.Play("Boss_LaserCast");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.speed = 1f;

        // 플레이어 위치 기록
        targetPosition = playerTransform.position;

        // 레이저 생성
        laserInstance = GameObject.Instantiate(
            laserPrefab,
            laserPos.position,
            Quaternion.identity
        );

        // 레이저를 플레이어 방향으로 회전
        Vector2 direction = targetPosition - (Vector2)laserPos.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laserInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 레이저 애니메이션 재생
        Animator laserAnimator = laserInstance.GetComponent<Animator>();
        if (laserAnimator != null)
        {
            laserAnimator.Play("Laser");
        }
    }

    public void OnExit()
    {
        Debug.Log("Boss Exiting LaserCast State");

        // 레이저 오브젝트 파괴
        if (laserInstance != null)
        {
            GameObject.Destroy(laserInstance);
        }
        anim.speed = 1f;
    }

    public void OnUpdate()
    {
        if (!PauseMenu.instance.isPaused)
        {
            laserTimer += Time.deltaTime;

            // 레이저 지속 시간 동안 사운드 재생
            if (laserTimer >= 0.35f && laserTimer < laserDuration)
            {
                if (AudioManager.instance.soundEffects[13].isPlaying == true) return;
                AudioManager.instance.soundEffects[13].Play();
            }
            else if (laserTimer >= laserDuration)
            {
                // 레이저 종료 후 Idle 상태 복귀
                fsm.SwitchState(StateType.Idle);
            }
        }
    }
}
