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
            case AnimationTriggerType.StartPunch:
                e.StartCoroutine(e.Punch());
                break;
            case AnimationTriggerType.EndPunch:
                e.EndPunch();
                break;
            case AnimationTriggerType.EndPunchDamaging:
                e.EndShouldBeDamaging();
                break;
        }
    }

    public override void EnterState()
    {
        if (e.canAttack && e.IsGrounded)
            e.Anim.SetBool("isPunching", true);
    }
}
