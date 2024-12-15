using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float detectionRange; // 敌人的视野范围

    Rigidbody2D theRB;
    SpriteRenderer theSR;
    Animator anim;

    private Vector2 lastVelocity; // 이전 프레임의 이동 속도 저장

    private enum EnemyState
    {
        Idle,
        Chasing,
        Battling
    }
    private EnemyState currentState;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        theSR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentState = EnemyState.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                CheckForPlayer();
                break;

            case EnemyState.Chasing:
            case EnemyState.Battling:
                MoveTowardsPlayer();
                break;
        }

        // 在每一帧更新最后的速度
        lastVelocity = theRB.velocity;
    }

    void CheckForPlayer()
    {
        if (PlayerController.instance != null)
        {
            float distance = Vector2.Distance(transform.position, PlayerController.instance.transform.position);
            // 플레이어가 감지 범위 내라면 추적 시작
            if (distance <= detectionRange)
            {
                SetChasingState();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // 플레이어 방향 벡터 정규화
        Vector2 direction = (PlayerController.instance.transform.position - transform.position).normalized;
        theRB.velocity = direction * moveSpeed;

        // 이동 방향에 따라 스프라이트 플립
        theSR.flipX = theRB.velocity.x < 0;
    }

    public void OnHitByPlayer()
    {
        SetBattlingState();
    }

    private void SetChasingState()
    {
        currentState = EnemyState.Chasing;
        anim.SetBool("isMoving", true);
    }

    private void SetBattlingState()
    {
        currentState = EnemyState.Battling;
        anim.SetBool("isMoving", true);
    }

    public Vector2 GetLastVelocity()
    {
        return lastVelocity;
    }

    /* 在编辑器中可视化检测范围 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    */
}
