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

    // Timers for move and pause intervals
    private float moveTimer = 0f;
    private float pauseTimer = 0f;

    // Random range for movement and pause times
    private float minMoveTime = 1.2f;
    private float maxMoveTime = 1.5f;
    private float minPauseTime = 0.2f;
    private float maxPauseTime = 0.5f;

    // Decorated method to control movement and pause intervals
    private void WalkToTargetWithIntervals(Vector2 target)
    {
        // Enemy is in a pause state
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            e.RB.velocity = new Vector2(0, e.RB.velocity.y); // Stop horizontal movement
            e.Anim.SetBool("isWalking", false);             // Set animation to idle
            return;
        }

        // Enemy is in a movement state
        if (moveTimer > 0f)
        {
            moveTimer -= Time.deltaTime;
            e.WalkToTarget(target);
        }
        else
        {
            // Reset move and pause timers after each interval
            moveTimer = Random.Range(minMoveTime, maxMoveTime);
            pauseTimer = Random.Range(minPauseTime, maxPauseTime);
        }
    }

    private float chanceToJumpDrop = 0.15f;
    private bool Randomize()
    {
        return Random.value < chanceToJumpDrop;
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
                    WalkToTargetWithIntervals(e.Player.transform.position); // chase after player

                    if (e.PlayerAbove && e.IsGrounded && Randomize()) // look for landing target if player above
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
                    if (e.PlayerBelow && Randomize() && !e.InImpact && !e.InHitStun && !e.InKnockup && !e.Anim.GetBool("ImpactBool"))
                    {
                        e.CurrentOneWayPlatform = Physics2D.OverlapBox(e.groundCheck.transform.position, e.CheckGroundSize, 0f, e.GroundLayer)?.gameObject;
                        if (e.CurrentOneWayPlatform != null && e.CurrentOneWayPlatform.CompareTag("OneWayPlatform"))
                        {
                            yield return e.PauseAction(e.JumpDelay);
                            e.StartCoroutine(e.DisableCollision());
                        }
                    }
                }

                if (e.MidJump && e.RB.velocity.y < 0 && !e.InKnockup) // if falling from a jump, regain control
                    WalkToTargetWithIntervals(e.Player.transform.position);
                
                yield return new WaitForFixedUpdate();
            }
        }
        finally
        {
            movementRoutine = null; // Clean up if coroutine exits
        }
    }
}
