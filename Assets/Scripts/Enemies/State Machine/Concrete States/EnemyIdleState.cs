using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector2 _targetPos;
    private float _currentTime = 0f;

    private TransitionDecisionDelegate Timing(TransitionDecisionDelegate transitionDecision)
    {
        return () =>
        {
            transitionDecision?.Invoke();

            // regular logic
            if (_currentTime > e.IdleTimeBetweenMove)
            {
                _currentTime = 0f;
                SetTarget();
            }
            _currentTime += Time.deltaTime;
        };
    }

    public EnemyIdleState(Enemy enemy, TransitionDecisionDelegate transitionDecision) : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.Idle;
        this.transitionDecision = Timing(transitionDecision);
    }

    public override void EnterState()
    {
        _targetPos = e.transform.position;
    }

    public override void PhysicsUpdate()
    {
        e.WalkToTarget(_targetPos);
    }

    private void SetTarget()
    {
        float randomX = Random.Range(-e.IdleRange, e.IdleRange);
        _targetPos = new Vector2(e.transform.position.x + randomX, e.transform.position.y);
    }
}
