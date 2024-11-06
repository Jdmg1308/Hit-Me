using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : Enemy, HasBasicStates
{
    // transition bools
    [field: SerializeField, Header("Transition Bools")] public bool InChaseRange { get; set; }
    [field: SerializeField] public bool InAttackRange { get; set; }

    // states
    public EnemyChaseState ChaseState;
    protected virtual void ChaseTransitionDecision()
    {
        // check state
        if (IsGrounded)
        { // can only change state if on ground and not paused
            if (canAttack && InAttackRange)
                StateMachine.changeState(AttackState);
            else if (InChaseRange)
                return;
            else
                StateMachine.changeState(IdleState);
        }
    }

    public EnemyAttackState AttackState;
    protected virtual void AttackTransitionDecision()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state
        if (!Anim.GetBool("isPunching"))
        {
            if (canAttack && InAttackRange) // repeatedely punch if in range
                AttackState.EnterState();
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else
                StateMachine.changeState(IdleState);
        }
    }
    
    public EnemyIdleState IdleState;
    protected virtual void IdleTransitionDecision()
    {
        if (canAttack && InAttackRange)
            StateMachine.changeState(AttackState);
        else if (InChaseRange)
            StateMachine.changeState(ChaseState);
    }

    protected override void Awake()
    {
        base.Awake();
        // setting up state machine
        ChaseState = new EnemyChaseState(this, ChaseTransitionDecision);
        AttackState = new EnemyAttackState(this, AttackTransitionDecision);
        IdleState = new EnemyIdleState(this, IdleTransitionDecision);
    }

    protected override void Start()
    {
        // state machine
        StateMachine.Initialize(IdleState);

        base.Start();
    }
}