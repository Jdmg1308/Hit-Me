using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TransitionDecisionDelegate();

public class EnemyState
{
    protected Enemy e;
    public EnemyStateMachine.EnemyStates id;
    protected TransitionDecisionDelegate transitionDecision; // represents how the state will decide on how to transition to new states

    public EnemyState(Enemy enemy, TransitionDecisionDelegate transitionDecision = null)
    {
        this.e = enemy;
        this.transitionDecision = transitionDecision;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public void FrameUpdate() { 
        // default anti-decision making variables
        if (!e.IsPaused && !e.InImpact && !e.InHitStun && !e.InKnockup)
            transitionDecision?.Invoke();
    }
    public virtual void PhysicsUpdate() { }

    public enum AnimationTriggerType
    {
        StartPunch,
        EndPunch,
        EndPunchDamaging
    }
    public virtual void AnimationTriggerEvent(AnimationTriggerType triggerType) { }
}
