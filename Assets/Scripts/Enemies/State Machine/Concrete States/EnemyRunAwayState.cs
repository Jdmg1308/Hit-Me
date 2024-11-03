using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunAwayState : EnemyState
{
    public EnemyRunAwayState(Enemy enemy, TransitionDecisionDelegate transitionDecision) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.RunAway;
    }

    public override void PhysicsUpdate()
    {
        e.StartCoroutine(enemyAI(!e.IsPaused));
    }

    private IEnumerator enemyAI(bool enabled)
    {
        if (enabled)
        {
            Vector3 oppositePosition = e.Player.transform.position - e.transform.position;
            oppositePosition.x = oppositePosition.x > 0 ? -1000 : 1000;
            oppositePosition.y = oppositePosition.y > 0 ? -1000 : 1000;

            // grounded and in control abilities
            if (e.IsGrounded)
            {
                e.WalkToTarget(oppositePosition); // run from player

                // looking to drop down away from player
                if ((e.PlayerAbove || !e.PlayerBelow) && e.CurrentOneWayPlatform != null)
                {
                    yield return e.PauseAction(e.JumpDelay);
                    e.StartCoroutine(e.DisableCollision());
                }

                if (e.PlayerBelow || !e.PlayerAbove) // look for landing target if player below or neither below or above (below you or same level)
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

                
            }

            if (e.MidJump && e.RB.velocity.y < 0) // if falling from a jump, regain control and try to get to landing positin
                e.WalkToTarget(e.LandingTarget);
        }
    }
}
