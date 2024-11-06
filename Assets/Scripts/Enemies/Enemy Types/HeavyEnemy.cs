using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class HeavyEnemy : BasicEnemy
{
    // transition bools
    public bool inGuardBreak;

    EnemyState GuardBreakState;
    protected void GuardBreakTransitionDecision()
    {
        if (inGuardBreak)
            return; // do nothing
        else if (canAttack && InAttackRange)
            DecideChargeAttack();
        else if (InChaseRange)
            StateMachine.changeState(ChaseState);
    }

    protected override void AttackTransitionDecision()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state
        if (inGuardBreak)
            StateMachine.changeState(GuardBreakState);
        else if (!Anim.GetBool("isPunching") && !Anim.GetBool("isArmoredAttack"))
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
        if (inGuardBreak)
            StateMachine.changeState(GuardBreakState);
        else if (IsGrounded)
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
        if (inGuardBreak)
            StateMachine.changeState(GuardBreakState);
        else if (canAttack && InAttackRange)
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
    public float GuardBreakTime = 5f; // how long guard break stun lasts
    public float forceThreshold = 16.5f; // min force required to cause guard break

    protected override void Awake()
    {
        base.Awake();
        // setting up state machine
        ChaseState = new EnemyChaseState(this, ChaseTransitionDecision);
        AttackState = new EnemyAttackState(this, AttackTransitionDecision);
        IdleState = new EnemyIdleState(this, IdleTransitionDecision);
        EnemyArmoredAttackState = new EnemyArmoredAttackState(this, AttackTransitionDecision, ArmoredAttackDamage, ArmoredAttackForce);
        GuardBreakState = new EnemyGuardBreakState(this, GuardBreakTransitionDecision, GuardBreakTime, value => inGuardBreak = value);
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

    public override void TakeKick(int damage, Vector2 force)
    {
        // if hit with enough force during armored attack
        if (force.x >= forceThreshold && HitStunImmune)
        {
            inGuardBreak = true;
            Anim.SetBool("isArmoredAttack", false); // cancel armored attack
            HitStunImmune = false;
        }

        base.TakeKick(damage, force);
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