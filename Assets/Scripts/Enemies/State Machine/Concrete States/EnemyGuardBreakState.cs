using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardBreakState : EnemyState
{
    private float TotalGuardBreakTime;
    private Action<bool> setGuardBreak;

    public EnemyGuardBreakState(Enemy enemy, TransitionDecisionDelegate transitionDecision, float guardBreakTime, Action<bool> setGuardBreakAction) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.GuardBreak;
        TotalGuardBreakTime = guardBreakTime;
        setGuardBreak = setGuardBreakAction;
    }

    public override void EnterState()
    {
        e.RB.mass = 1; // lightest mass
        e.SetSpriteColor(0.5f);
        e.StartCoroutine(GuardBreakTimer());
    }

    private IEnumerator GuardBreakTimer()
    {
        yield return new WaitForSeconds(TotalGuardBreakTime);
        e.RB.mass = e.baseMass;
        e.SetSpriteColor(1f);
        e.Anim.SetBool("isWalking", false);
        setGuardBreak(false);
    }
}
