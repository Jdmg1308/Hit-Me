using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public enum EnemyStates {
            Idle,
            Chase,
            Attack
        }
    public EnemyState currentEnemyState { get; set; }

    public void Initialize(EnemyState startingState) {
        currentEnemyState = startingState;
        currentEnemyState.EnterState();
    }

    public void changeState(EnemyState newState) {
        currentEnemyState.ExitState();
        currentEnemyState = newState;
        currentEnemyState.EnterState();
    } 
}
