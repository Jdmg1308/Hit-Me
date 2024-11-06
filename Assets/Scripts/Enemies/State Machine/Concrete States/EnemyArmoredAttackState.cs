using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

public class EnemyArmoredAttackState : EnemyState
{
    private int attackDamage;
    private Vector2 attackForce;
    private float TotalGuardBreakTime;
    private Func<bool> getGuardBreak;
    private Action<bool> setGuardBreak;
    private float originalMass;
    private bool currentlyStunned;
    private float TotalChargeUpTime;

    private Coroutine hitStunImmunity;
    private Coroutine hitStunImmunityFlash;

    public EnemyArmoredAttackState(Enemy enemy, TransitionDecisionDelegate transitionDecision, 
        int damage, Vector2 force, float chargeUpTime, float guardBreakTime, Func<bool> getGuardBreakFunc, Action<bool> setGuardBreakAction, float baseMass) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.ArmoredAttack;
        attackDamage = damage;
        attackForce = force;
        TotalGuardBreakTime = guardBreakTime;
        getGuardBreak = getGuardBreakFunc;
        setGuardBreak = setGuardBreakAction;
        originalMass = baseMass;
        TotalChargeUpTime = chargeUpTime;
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case AnimationTriggerType.StartAttack:
                e.StartCoroutine(ArmoredAttack());
                break;
            case AnimationTriggerType.ArmorUp:
                ArmorUp();
                break;
            case AnimationTriggerType.ChargeUp:
                e.StartCoroutine(ChargeUp());
                break;
            case AnimationTriggerType.EndAttack:
                EndArmoredAttack();
                break;
            case AnimationTriggerType.EndAttackDamaging:
                e.EndShouldBeDamaging();
                break;
        }
    }

    public override void EnterState()
    {
        if (e.canAttack && e.IsGrounded && !e.MidJump)
        {
            float dirToPlayer = e.Player.transform.position.x - e.transform.position.x;
            e.FlipCharacter(dirToPlayer > 0);
            e.Anim.SetBool("isArmoredAttack", true);
        }
    }

    public override void FrameUpdate()
    {
        if (getGuardBreak() && !currentlyStunned)
            e.StartCoroutine(GuardBreakTimer());
        
        base.FrameUpdate();
    }

    // armored attack active frames
    public IEnumerator ArmoredAttack()
    {
        e.ShouldBeDamaging = true;
        while (e.ShouldBeDamaging && !e.InHitStun)
        {
            Collider2D player = Physics2D.OverlapCircle(e.DetectAttack.transform.position, e.AttackRadius, 1 << e.Player.layer);
            if (player != null)
            {
                Vector2 force = new Vector2((e.FacingRight ? 1 : -1) * Math.Abs(attackForce.x), attackForce.y);
                player.GetComponent<PlayerController>().TakeDamage(attackDamage, force);
            }
            yield return null; // wait a frame
        }
    }

    public void ArmorUp()
    {
        // start hit stun immunity
        if (!e.HitStunImmune)
        {
            hitStunImmunity = e.StartCoroutine(e.HitStunImmunity(1000));
            hitStunImmunityFlash = e.StartCoroutine(e.HitStunImmunityFlash(1000));
        }
    }

    // freeze while charging
    public IEnumerator ChargeUp()
    {
        float elapsedTime = 0;
        e.Anim.speed = 0;
        while (e.HitStunImmune && elapsedTime < TotalChargeUpTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        e.Anim.speed = 1;
    }

    private IEnumerator GuardBreakTimer()
    {
        // stopping all indications of hit stun immunity
        if (hitStunImmunity != null)
        {
            e.StopCoroutine(hitStunImmunity);
            hitStunImmunity = null;
        }
        if (hitStunImmunityFlash != null)
        {
            e.StopCoroutine(hitStunImmunityFlash);
            hitStunImmunityFlash = null;
        }

        // adjusting mass for combos
        e.baseMass = 1;
        e.RB.mass = e.baseMass;

        // visual indicator
        e.SetSpriteColor(0.5f);
        e.Anim.SetBool("isWalking", false);

        currentlyStunned = true;
        yield return new WaitForSeconds(TotalGuardBreakTime);

        // reset character
        e.baseMass = originalMass;
        e.RB.mass = e.baseMass;
        e.SetSpriteColor(1f);
        setGuardBreak(false);
        currentlyStunned = false;
    }

    // set end of animation
    public void EndArmoredAttack()
    {
        e.Anim.SetBool("isArmoredAttack", false);
        e.StartCoroutine(e.PauseAction(e.AttackWait));
        // stopping all indications of hit stun immunity
        e.HitStunImmune = false;
        if (hitStunImmunity != null)
        {
            e.StopCoroutine(hitStunImmunity);
            hitStunImmunity = null;
        }
        if (hitStunImmunityFlash != null)
        {
            e.StopCoroutine(hitStunImmunityFlash);
            hitStunImmunityFlash = null;
        }
    }
}
