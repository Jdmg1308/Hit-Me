using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunAwayState : EnemyState
{
    private Coroutine movementRoutine;

    public EnemyRunAwayState(Enemy enemy, TransitionDecisionDelegate transitionDecision) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.RunAway;
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

    private bool reverseRun;
    private Vector3 wallRunAwayFrom;
    private Coroutine reverseRunRoutine;

    public void reverseRunAway(float time, Vector3 wallPosition)
    {
        if (reverseRunRoutine != null)
            e.StopCoroutine(reverseRunRoutine);
        reverseRunRoutine = e.StartCoroutine(reverseRunAwayHelper(time, wallPosition));
    }

    // if touch wall, can change 'oppositePosition' to other wall to prevent sticking to a wall
    IEnumerator reverseRunAwayHelper(float time, Vector3 wallPosition)
    {   
        reverseRun = true;
        wallRunAwayFrom = wallPosition;
        yield return new WaitForSeconds(time);
        reverseRun = false;
    }

    private IEnumerator enemyAI()
    {
        try
        {
            while (true && !e.IsPaused)
            {
                Vector3 oppositePosition;
                if (!reverseRun)
                    oppositePosition = e.Player.transform.position - e.transform.position;
                else
                    oppositePosition = wallRunAwayFrom - e.transform.position;

                oppositePosition.x = oppositePosition.x > 0 ? -1000 : 1000;
                oppositePosition.y = oppositePosition.y > 0 ? -1000 : 1000;

                // grounded and in control abilities
                if (e.IsGrounded)
                {
                    e.WalkToTarget(oppositePosition); // run from player

                    if (e.PlayerBelow || !e.PlayerAbove && e.IsGrounded) // look for landing target if player below or neither below or above (below you or same level)
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

                    // looking to drop down away from player
                    if (e.PlayerAbove || !e.PlayerBelow && !e.InImpact && !e.InHitStun && !e.InKnockup && !e.Anim.GetBool("ImpactBool"))
                    {
                        e.CurrentOneWayPlatform = Physics2D.OverlapBox(e.groundCheck.transform.position, e.CheckGroundSize, 0f, e.GroundLayer)?.gameObject;
                        if (e.CurrentOneWayPlatform != null && e.CurrentOneWayPlatform.CompareTag("OneWayPlatform"))
                        {
                            yield return e.PauseAction(e.JumpDelay);
                            e.StartCoroutine(e.DisableCollision());
                        }
                    }
                }

                if (e.MidJump && e.RB.velocity.y < 0 && !e.InKnockup) // if falling from a jump, regain control and try to get to landing positin
                    e.WalkToTarget(oppositePosition);
                    
                yield return new WaitForFixedUpdate();
            }
        }
        finally
        {
            movementRoutine = null; // Clean up if coroutine exits
        }
    }
}
