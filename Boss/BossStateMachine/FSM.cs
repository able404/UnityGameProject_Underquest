using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StateType
{
    Sleep,
    Awake,
    Idle,
    Move,
    Shoot,
    LaserCast,
    Melee,
    Death
}

public interface IState
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
}

[Serializable]
public class Blackboard
{
    // 여기서 공유 데이터나 외부에 노출할 설정 가능한 데이터 등을 저장
}

public class FSM
{
    public IState curState;
    public Dictionary<StateType, IState> states;
    public Blackboard blackboard;

    public Transform transform; // FSM과 연관된 캐릭터의 트랜스폼

    public FSM(Blackboard blackboard)
    {
        this.states = new Dictionary<StateType, IState>();
        this.blackboard = blackboard;
    }

    public void AddState(StateType stateType, IState state)
    {
        if(states.ContainsKey(stateType))
        {
            Debug.Log("[AddState] >>>>> map has contain key: " + stateType);
            return;
        }
        states.Add(stateType, state);
    }

    public void SwitchState(StateType stateType)
    {
        if(!states.ContainsKey(stateType))
        {
            Debug.Log("[SwitchState] >>>>> not contain key: " + stateType);
            return;
        }
        if(curState != null)
        {
            curState.OnExit();
        }
        curState = states[stateType];
        curState.OnEnter();
    }

    public void OnUpdate()
    {
        curState.OnUpdate();
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            Vector3 scale = transform.localScale;
            if (transform.position.x > target.position.x)
            {
                scale.x = Mathf.Abs(scale.x) * -1; // left
            }
            else if (transform.position.x < target.position.x)
            {
                scale.x = Mathf.Abs(scale.x); // right
            }
            transform.localScale = scale;
        }
    }
}
