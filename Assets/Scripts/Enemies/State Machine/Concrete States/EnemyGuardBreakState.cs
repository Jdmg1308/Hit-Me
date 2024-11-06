using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardBreakState : EnemyState
{
    private float TotalGuardBreakTime;
    private Action<bool> setGuardBreak;

    public EnemyGuardBreakState(Enemy enemy, TransitionDecisionDelegate transitionDecision, 
        float guardBreakTime, Action<bool> setGuardBreakAction) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.GuardBreak;
        TotalGuardBreakTime = guardBreakTime;
        setGuardBreak = setGuardBreakAction;
    }

    public override void EnterState()
    {
        e.StartCoroutine(GuardBreakTimer());
    }

    private IEnumerator GuardBreakTimer()
    {
        float originalMass = e.baseMass;
        e.baseMass = 1; // lightest mass for combos
        e.RB.mass = e.baseMass;
        e.SetSpriteColor(0.5f);
        e.Anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(TotalGuardBreakTime);

        // reset character
        e.baseMass = originalMass;
        e.RB.mass = e.baseMass;
        e.SetSpriteColor(1f);
        setGuardBreak(false);
    }
}
