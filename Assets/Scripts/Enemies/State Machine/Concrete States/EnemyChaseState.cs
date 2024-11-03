using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy, TransitionDecisionDelegate transitionDecision) : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.Chase;
    }

    public override void PhysicsUpdate()
    {
        e.StartCoroutine(enemyAI(!e.IsPaused));
    }

    private IEnumerator enemyAI(bool enabled)
    {
        if (enabled)
        {
            // grounded and in control abilities
            if (e.IsGrounded)
            {
                e.WalkToTarget(e.Player.transform.position); // chase after player

                if (e.PlayerAbove) // look for landing target if player above
                {
                    e.DetectTargetFromPlatform();
                    e.ShouldJump = e.LandingTarget != Vector2.zero;
                }
                // looking to jump and valid landing target exists
                if (e.ShouldJump)
                {
                    Vector2 val = e.CalculateJumpForce(e.LandingTarget + (Vector2.up * e.LandingOffset));
                    e.FlipCharacter(val.x > 0);

                    yield return e.PauseAction(e.JumpDelay);

                    e.RB.velocity = new Vector2(val.x, 0f);
                    e.RB.AddForce(new Vector2(0f, val.y), ForceMode2D.Impulse);
                    e.ShouldJump = false;
                    e.MidJump = true;
                }

                // looking to drop down to player
                if (e.PlayerBelow && e.CurrentOneWayPlatform != null)
                {
                    yield return e.PauseAction(e.JumpDelay);
                    e.StartCoroutine(e.DisableCollision());
                }
            }

            if (e.MidJump && e.RB.velocity.y < 0) // if falling from a jump, regain control
                e.WalkToTarget(e.Player.transform.position);
        }
    }
}
