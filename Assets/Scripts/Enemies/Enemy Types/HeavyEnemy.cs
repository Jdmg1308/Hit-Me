using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class HeavyEnemy : BasicEnemy
{
    protected override void AttackTransitionDecision()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state
        if (!Anim.GetBool("isPunching") && !Anim.GetBool("isArmoredAttack"))
        {
            if (canAttack && InAttackRange) // repeatedly punch if in range
                DecideChargeAttack();
            else if (InChaseRange)
                StateMachine.changeState(ChaseState);
            else
                StateMachine.changeState(IdleState);
        }
    }

    protected override void ChaseTransitionDecision()
    {
        // check state
        if (IsGrounded)
        { // can only change state if on ground and not paused
            if (canAttack && InAttackRange)
                DecideChargeAttack();
            else if (InChaseRange)
                return;
            else
                StateMachine.changeState(IdleState);
        }
    }

    protected override void IdleTransitionDecision()
    {
        if (canAttack && InAttackRange)
            DecideChargeAttack();
        else if (InChaseRange)
            StateMachine.changeState(ChaseState);
    }

    public EnemyState EnemyArmoredAttackState;

    [Header("Armored Attack Props")]
    public float baseChanceForChargedAttack = 0.1f;
    public float currentChanceForChargedAttack;
    public float perMissChanceIncrease = 0.1f; // increase per missed attempt for heavy attack
    public float perHitChanceIncrease = 0.01f; // increase per tick of damage taken
    public int ArmoredAttackDamage = 18;
    public Vector2 ArmoredAttackForce;
    public float ArmoredTime; // sets how long attack 'charges' for

    protected override void Awake()
    {
        base.Awake();
        // setting up state machine
        ChaseState = new EnemyChaseState(this, ChaseTransitionDecision);
        AttackState = new EnemyAttackState(this, AttackTransitionDecision);
        IdleState = new EnemyIdleState(this, IdleTransitionDecision);
        EnemyArmoredAttackState = new EnemyArmoredAttackState(this, AttackTransitionDecision, ArmoredAttackDamage, ArmoredAttackForce);
    }

    protected override void Start()
    {
        // state machine
        StateMachine.Initialize(IdleState);
        currentChanceForChargedAttack = baseChanceForChargedAttack;
        HitStunImmunityTime = ArmoredTime;

        base.Start();
    }

    // decorated damage function that incremets chance for charged attack on hit
    public override void Damage(int damage, float hitStunTime)
    {
        currentChanceForChargedAttack += perHitChanceIncrease;
        base.Damage(damage, hitStunTime);
    }

    private void DecideChargeAttack()
    {
        // doesn't do attack
        if (UnityEngine.Random.value >= currentChanceForChargedAttack)
        {
            StateMachine.changeState(AttackState);
            currentChanceForChargedAttack += perMissChanceIncrease;
        }
        else // perform armored attack instead of regular
        {
            StateMachine.changeState(EnemyArmoredAttackState);
            currentChanceForChargedAttack = baseChanceForChargedAttack;
        }
    }
}