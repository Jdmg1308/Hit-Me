using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttackState : EnemyState
{
    public EnemyRangedAttackState(Enemy enemy, TransitionDecisionDelegate transitionDecision) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.RangedAttack;
    }
}
