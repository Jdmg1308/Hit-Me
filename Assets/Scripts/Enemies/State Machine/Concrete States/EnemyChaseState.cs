using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private Coroutine movementRoutine;

    public EnemyChaseState(Enemy enemy, TransitionDecisionDelegate transitionDecision) : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.Chase;
    }

    public override void EnterState()
    {
        movementRoutine = e.StartCoroutine(enemyAI());
    }

    public override void ExitState()
    {
        if (movementRoutine != null)
            e.StopCoroutine(movementRoutine);
    }

    private IEnumerator enemyAI()
    {
        try
        {
            while (true)
            {
                // grounded and in control abilities
                if (e.IsGrounded)
                {
                    e.WalkToTarget(e.Player.transform.position); // chase after player

                    if (e.PlayerAbove && e.IsGrounded) // look for landing target if player above
                    {
                        e.DetectTargetFromPlatform();
                        e.ShouldJump = e.LandingTarget != Vector2.zero;
                    }
                    // looking to jump and valid landing target exists
                    if (e.ShouldJump)
                    {
                        Vector2 val = e.CalculateJumpForce(e.LandingTarget + (Vector2.up * e.LandingOffset));
                        e.FlipCharacter(val.x > 0);
                        e.RB.velocity = Vector2.zero;

                        yield return e.PauseAction(e.JumpDelay);

                        // getting hit cancels jump
                        if (!e.InImpact && !e.InHitStun && !e.InKnockup && !e.Anim.GetBool("ImpactBool"))
                        {
                            e.RB.velocity = new Vector2(val.x, 0f);
                            e.RB.AddForce(new Vector2(0f, val.y), ForceMode2D.Impulse);
                            e.ShouldJump = false;
                            e.MidJump = true;
                        }
                    }

                    // looking to drop down to player
                    if (e.PlayerBelow)
                    {
                        e.CurrentOneWayPlatform = Physics2D.OverlapBox(e.groundCheck.transform.position, e.CheckGroundSize, 0f, e.GroundLayer).gameObject;
                        if (e.CurrentOneWayPlatform != null && e.CurrentOneWayPlatform.CompareTag("OneWayPlatform"))
                        {
                            yield return e.PauseAction(e.JumpDelay);
                            e.StartCoroutine(e.DisableCollision());
                        }
                    }
                }

                if (e.MidJump && e.RB.velocity.y < 0) // if falling from a jump, regain control
                    e.WalkToTarget(e.Player.transform.position);
                
                yield return new WaitForFixedUpdate();
            }
        }
        finally
        {
            movementRoutine = null; // Clean up if coroutine exits
        }
    }
}
