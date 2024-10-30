using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public bool InChaseRange;
    public bool InAttackRange;
    public EnemyChaseState ChaseState;
    public EnemyAttackState AttackState;
    public EnemyIdleState IdleState;

    public EnemyStateMachine<BasicEnemy>.EnemyStates enemyState;
    public EnemyStateMachine<BasicEnemy> StateMachine { get; set; }

    protected override void Awake()
    {
        base.Awake();
        // setting up state machine
        StateMachine = new EnemyStateMachine<BasicEnemy>();
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        IdleState = new EnemyIdleState(this, StateMachine);
    }

    protected override void Start()
    {
        // state machine
        StateMachine.Initialize(IdleState);

        base.Start();
    }

    protected override void Update()
    {
        StateMachine.currentEnemyState.FrameUpdate();
        enemyState = StateMachine.currentEnemyState.id; // for debug

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsPaused && !InImpact && !InKnockup)
            StateMachine.currentEnemyState.PhysicsUpdate();
    }

    #region Detection
    public void SetInChaseRange(bool inChaseRange)
    {
        InChaseRange = inChaseRange;
    }

    public void SetInAttackRange(bool inAttackRange)
    {
        InAttackRange = inAttackRange;
    }
    #endregion

    #region Animation Triggers
    public void AnimationTriggerEvent(EnemyState<BasicEnemy>.AnimationTriggerType triggerType)
    {
        StateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }
    #endregion
}