using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmoredAttackState : EnemyState
{
    private int attackDamage;
    private Vector2 attackForce;

    public EnemyArmoredAttackState(Enemy enemy, TransitionDecisionDelegate transitionDecision, int damage, Vector2 force) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.ArmoredAttack;
        attackDamage = damage;
        attackForce = force;
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case AnimationTriggerType.StartAttack:
                e.StartCoroutine(ArmoredAttack());
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

    // hit stun immunity for 'armored' effect
    public IEnumerator ChargeUp()
    {
        // start hit stun immunity
        if (!e.HitStunImmune)
        {
            e.HitStunImmune = true;
            e.StartCoroutine(e.HitStunImmunity(e.HitStunImmunityTime));
            e.StartCoroutine(e.HitStunImmunityFlash(e.HitStunImmunityTime));
        }

        // waiting out hit stun immunity
        e.Anim.speed = 0;
        while (e.HitStunImmune)
        {
            yield return null;
        }
        e.Anim.speed = 1;
    }

    // set end of animation
    public void EndArmoredAttack()
    {
        e.Anim.SetBool("isArmoredAttack", false);
        e.StartCoroutine(e.PauseAction(e.AttackWait));
    }
}
