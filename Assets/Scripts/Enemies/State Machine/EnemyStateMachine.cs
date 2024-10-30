using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine<T> where T : Enemy
{
    public enum EnemyStates
    {
        Idle,
        Chase,
        Attack
    }
    public EnemyState<T> currentEnemyState { get; set; }

    public void Initialize(EnemyState<T> startingState)
    {
        currentEnemyState = startingState;
        currentEnemyState.EnterState();
    }

    public void changeState(EnemyState<T> newState)
    {
        currentEnemyState.ExitState();
        currentEnemyState = newState;
        currentEnemyState.EnterState();
    }
}
