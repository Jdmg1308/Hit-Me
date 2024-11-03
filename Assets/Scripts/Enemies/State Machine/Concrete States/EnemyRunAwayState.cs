using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunAwayState : EnemyState
{
    public EnemyRunAwayState(Enemy enemy, TransitionDecisionDelegate transitionDecision) : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.RunAway;
    }
}
