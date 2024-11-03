using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : Enemy, HasBasicStates, HasRangedStates
{
    // transition bools
    [field: SerializeField] public bool InAttackRange { get; set; }
    [field: SerializeField] public bool InRunAwayRange { get; set; }
    [field: SerializeField] public bool InRangedAttackRange { get; set;}
    [field: SerializeField] public bool InChaseRange { get; set; }
    public bool AllowShyAttack;

    // states
    public EnemyAttackState AttackState;
    private void AttackTransitionDecision()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state
        if (!Anim.GetBool("ImpactBool")) // effectively not in hitstun
        {
            if (!Anim.GetBool("isPunching") && !InAttackRange)
            {
                if (InRunAwayRange)
                    StateMachine.changeState(RunAwayState);
                else if (canAttack && InRangedAttackRange)
                    StateMachine.changeState(RangedAttackState);
                else if (InChaseRange)
                    StateMachine.changeState(ChaseState);
                else
                    StateMachine.changeState(IdleState);
            }
            else
            { // repeatedely punch if in range
                AttackState.EnterState();
            }
        }
    }

    public EnemyRunAwayState RunAwayState;
    private void RunAwayTransitionDecision()
    {
        if (IsGrounded)
        {
            if (canAttack && InAttackRange)
                StateMachine.changeState(AttackState);
            else if (canAttack && InRangedAttackRange)
                StateMachine.changeState(RangedAttackState);
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else if (!InRunAwayRange)
                StateMachine.changeState(IdleState);
        }
    }
    
    public EnemyRangedAttackState RangedAttackState;
    private void RangedAttackTransitionDecision()
    {
        if (!Anim.GetBool("ImpactBool"))
        {
            if (!Anim.GetBool("isPunching")) // replace isPunching withr respective range anim
            {
                if (canAttack && InAttackRange)
                    StateMachine.changeState(AttackState);
                else if (InRunAwayRange)
                    StateMachine.changeState(RunAwayState);
                else if (InChaseRange && !InRangedAttackRange)
                    StateMachine.changeState(ChaseState);
                else if (!InRangedAttackRange)
                    StateMachine.changeState(IdleState);
            }
            else
            {
                RangedAttackState.EnterState();
            }
        }
    }
    
    public EnemyChaseState ChaseState;
    private void ChaseTransitionDecision()
    {
        if (IsGrounded)
        {
            if (canAttack && InAttackRange)
                StateMachine.changeState(AttackState);
            else if (InRunAwayRange)
                StateMachine.changeState(RunAwayState);
            else if (canAttack && InRangedAttackRange)
                StateMachine.changeState(RangedAttackState);
            else if (!InChaseRange)
                StateMachine.changeState(IdleState);
        }
    }

    public EnemyIdleState IdleState;
    private void IdleTransitionDecision()
    {
        // in priority of distance
        if (canAttack && InAttackRange)
            StateMachine.changeState(AttackState);
        else if (InRunAwayRange)
            StateMachine.changeState(RunAwayState);
        else if (canAttack && InRangedAttackRange)
            StateMachine.changeState(RangedAttackState);
        else if (InChaseRange)
            StateMachine.changeState(ChaseState);
    }

    protected override void Awake()
    {
        base.Awake();
        // setting up states
        AttackState = new EnemyAttackState(this, AttackTransitionDecision);
        RunAwayState = new EnemyRunAwayState(this, RunAwayTransitionDecision);
        RangedAttackState = new EnemyRangedAttackState(this, RangedAttackTransitionDecision);
        ChaseState = new EnemyChaseState(this, ChaseTransitionDecision);
        IdleState = new EnemyIdleState(this, IdleTransitionDecision);
    }

    protected override void Start()
    {
        // state machine
        StateMachine.Initialize(IdleState);

        base.Start();
    }
}