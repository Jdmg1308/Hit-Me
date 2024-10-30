using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState<BasicEnemy>
{
    public EnemyAttackState(BasicEnemy enemy, EnemyStateMachine<BasicEnemy> enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        id = EnemyStateMachine<BasicEnemy>.EnemyStates.Attack;
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
        if (e.canAttack)
            e.Anim.SetBool("isPunching", true);
    }

    public override void FrameUpdate()
    {
        // must finish punch animation before considering next action
        // InImpact = taking collisions, ImpactBool = damage hit stun state?
        if (!e.IsPaused && !e.InImpact && !e.Anim.GetBool("ImpactBool") && !e.InHitStun && !e.InKnockup)
        {
            if (!e.Anim.GetBool("isPunching") && !e.InAttackRange)
            {
                if (!e.InChaseRange)
                    enemyStateMachine.changeState(e.IdleState);
                else
                    enemyStateMachine.changeState(e.ChaseState);
            }
            else
            { // repeatedely punch if in range
                EnterState();
            }
        }
    }
}
