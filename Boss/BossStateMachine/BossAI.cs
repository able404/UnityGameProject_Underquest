using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;

[Serializable]
public class BossBlackboard : Blackboard
{
    [Header("Movement Settings")]
    public float moveSpeed;

    [Header("Detection Range")]
    public float detectionWidth; // 감지 범위(가로)
    public float detectionHeight; // 감지 범위(세로)

    [Header("References")]
    public GameObject owner; // Boss本体的引用

    [Header("Projectile Settings")]
    public GameObject armProjectilePrefab;
    public Transform armProjectilePos;

    [Header("Laser Settings")]
    public GameObject laserPrefab;
    public Transform laserPos;
}

public class BossAI : MonoBehaviour
{
    private FSM fsm;
    public BossBlackboard blackboard;

    void Start()
    {
        // 상태 머신 초기화
        fsm = new FSM(blackboard);
        fsm.transform = this.transform;

        // 각종 상태 추가
        fsm.AddState(StateType.Sleep, new AI_SleepState(fsm));
        fsm.AddState(StateType.Awake, new AI_AwakeState(fsm));
        fsm.AddState(StateType.Idle, new AI_IdleState(fsm));
        fsm.AddState(StateType.Move, new AI_MoveState(fsm));
        fsm.AddState(StateType.Death, new AI_DeathState(fsm));
        fsm.AddState(StateType.Melee, new AI_MeleeState(fsm));
        fsm.AddState(StateType.Shoot, new AI_ShootState(fsm));
        fsm.AddState(StateType.LaserCast, new AI_LaserCastState(fsm));

        // 초기 상태: Sleep
        fsm.SwitchState(StateType.Sleep);
    }

    void Update()
    {
        fsm.OnUpdate();
    }

    public void EnterDeathState()
    {
        fsm.SwitchState(StateType.Death);
    }

    /* 在编辑器中可视化检测范围
    void OnDrawGizmos()
    {
        if (blackboard == null || blackboard.owner == null) return;

        // 获取检测范围的中心和大小
        Vector3 position = blackboard.owner.transform.position;
        Vector3 size = new Vector3(blackboard.detectionWidth, blackboard.detectionHeight, 0);

        // 设置 Gizmos 的颜色
        Gizmos.color = Color.yellow;

        // 绘制长方形范围
        Gizmos.DrawWireCube(position, size);
    }
    */
}
