using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, TransitionDecisionDelegate transitionDecision) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.Attack;
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case AnimationTriggerType.StartAttack:
                e.StartCoroutine(e.Punch());
                break;
            case AnimationTriggerType.EndAttack:
                e.EndPunch();
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
            e.Anim.SetBool("isPunching", true);
        }
    }
}
