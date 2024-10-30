using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState<T> where T : Enemy
{
    protected T e;
    protected EnemyStateMachine<T> enemyStateMachine;
    public EnemyStateMachine<T>.EnemyStates id;

    public EnemyState(T enemy, EnemyStateMachine<T> enemyStateMachine)
    {
        this.e = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }

    public enum AnimationTriggerType
    {
        StartPunch,
        EndPunch,
        EndPunchDamaging
    }
    public virtual void AnimationTriggerEvent(AnimationTriggerType triggerType) { }
}
