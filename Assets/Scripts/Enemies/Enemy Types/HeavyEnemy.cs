using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class HeavyEnemy : BasicEnemy
{
    private void ChaseTransitionDecision()
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

    private void AttackTransitionDecision()
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
    
    private void IdleTransitionDecision()
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