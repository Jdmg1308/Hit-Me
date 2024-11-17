using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : Enemy, HasBasicStates, HasRangedStates
{
    // transition bools
    [field: SerializeField, Header("Transition Bools")] public bool InAttackRange { get; set; }
    [field: SerializeField] public bool InRunAwayRange { get; set; }
    [field: SerializeField] public bool InRangedAttackRange { get; set;}
    [field: SerializeField] public bool InChaseRange { get; set; }

    // props for states
    [Header("Ranged Enemy Props")]
    public float TimeBetweenRangedAttack = 5f;
    public GameObject BulletPrefab;
    public bool AllowShyAttack;

    // states
    public EnemyAttackState AttackState;
    private void AttackTransitionDecision()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state
        if (!Anim.GetBool("isPunching"))
        {
            if (canAttack && InAttackRange)
                AttackState.EnterState();
            else if (InRunAwayRange)
                StateMachine.changeState(RunAwayState);
            else if (canAttack && InRangedAttackRange)
                StateMachine.changeState(RangedAttackState);
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else
                StateMachine.changeState(IdleState);
        }
    }

    public EnemyRunAwayState RunAwayState;
    private void RunAwayTransitionDecision()
    {
        if (IsGrounded)
        {
            if (canAttack && InAttackRange)
                StateMachine.changeState(AttackState);
            else if (InRunAwayRange)
                return;
            else if (canAttack && InRangedAttackRange)
                StateMachine.changeState(RangedAttackState);
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else
                StateMachine.changeState(IdleState);
        }
    }
    
    public EnemyRangedAttackState RangedAttackState;
    private void RangedAttackTransitionDecision()
    {
        if (!Anim.GetBool("isShooting"))
        {
            if (canAttack && InAttackRange)
                StateMachine.changeState(AttackState);
            else if (InRunAwayRange)
                StateMachine.changeState(RunAwayState);
            else if (InRangedAttackRange)
                RangedAttackState.EnterState();
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else
                StateMachine.changeState(IdleState);
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
            else if (InChaseRange)
                return;
            else
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
        RangedAttackState = new EnemyRangedAttackState(this, RangedAttackTransitionDecision, TimeBetweenRangedAttack, BulletPrefab);
        ChaseState = new EnemyChaseState(this, ChaseTransitionDecision);
        IdleState = new EnemyIdleState(this, IdleTransitionDecision);
    }

    protected override void Start()
    {
        // state machine
        StateMachine.Initialize(IdleState);

        base.Start();
    }

    public float reverseRunAwayTime = 2f;

    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.layer == LayerMask.NameToLayer("InvisibleWalls"))
        {
            if (StateMachine.currentEnemyState is EnemyRunAwayState runAway)
                runAway.reverseRunAway(reverseRunAwayTime, collision.transform.position);
        }
    }
}