using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, IPuncher
{
    #region Variables
    [Header("Game Object Dependencies")]
    public GameEnemyManager GameEnemyManager;
    public GameObject Player;
    public Animator Anim { get; set; }

    [Header("State Variables")]
    public bool InImpact = false; // record knockback path + allows collision dmg + anim lock
    public bool InKnockup = false; // anim lock

    // IMoveable State Variables
    [field: SerializeField] public bool IsPaused { get; set; }
    [field: SerializeField] public bool FacingRight { get; set; } = false;
    [field: SerializeField] public bool IsGrounded { get; set; }
    [field: SerializeField] public bool ShouldJump { get; set; }
    [field: SerializeField] public bool MidJump { get; set; }
    [field: SerializeField] public bool PlayerAbove { get; set; }
    [field: SerializeField] public bool PlayerBelow { get; set; }

    // IDamageable Variables
    [field: SerializeField] public bool InHitStun { get; set; } = false;
    [field: SerializeField] public bool inAirStun { get; set; }
    [field: SerializeField] public bool inDownSlam { get; set; }

    // Collision Tuning
    [Header("Collision Tuning")]
    public float maxVelocity = 60; // don't want enemies to break game speed

    [Tooltip("Force threshold needed to cause damage and impact state, if not met then return to normal control")] public float collisionForceThreshold = 10;
    [Tooltip("Determines how much collision force is factored into impact damage"), Range(0, 1)] public float collisionDamageMultiplier = 0.5f;
    [Tooltip("Determines how much impact force is factored into bounce"), Range(0, 1)] public float collisionForceMultiplier = 0.5f;
    public float baseMass = 1;
    [Tooltip("Affects enemy floatiness during Impact State (for easier juggles)"), Range(0, 1)] public float postImpactMassScale = 0.9f;
    
    // IDamageable Variables
    [field: SerializeField, Header("Health/Death")] public int MaxHealth { get; set; } = 50;
    [field: SerializeField] public int CurrentHealth { get; set; } = 50;
    public object DeathLock { get; set; } = new object();
    [field: SerializeField] public bool IsDead { get; set; } = false;
    [field: SerializeField] public float HitStunTime { get; set; } = 0.3f;
    public int HitStunLimit = 100; // amount of hits can receive before getting hit stun immunity
    public int CurrentHitStunAmount = 0;
    public float HitStunImmunityTime = 3; // how long hit stun immunity lasts
    public bool HitStunImmune; // set during immunity
    public float outOfCombatDuration = 5f; // Duration for the timer in seconds
    private float outOfCombatTimer = 0f;   // Current timer value
    public Vector2 enemyCollisionForce = new Vector2(0.5f, 1.5f); // force applied when colliding with another enemy
    private GameObject downSlamExplosionTrigger;

    // IMoveable Variables
    // components
    public Rigidbody2D RB { get; set; }

    // state
    public Vector3 TopEnemyTransform { get; set; }
    public Vector3 BottomEnemyTransform { get; set; }
    private GameObject groundCheck;
    public float BodyGravity { get; set; }
    public float MaxJumpHeight { get; set; }
    public float MaxJumpDistance { get; set; }
    public Vector2 PlatformDetectionOrigin { get; set; }
    public Vector2 LandingTarget { get; set; }
    public GameObject CurrentOneWayPlatform { get; set; }
    public BoxCollider2D Collider { get; set; }

    // editor properties
    [field: SerializeField, Header("Movement")] public float ChaseSpeed { get; set; } = 3f;
    [field: SerializeField] public float MaxYJumpForce { get; set; } = 15f;
    [field: SerializeField] public float MaxXJumpForce { get; set; } = 5f;
    [field: SerializeField] public LayerMask PlatformDetectionMask { get; set; }
    [field: SerializeField] public float FallthroughTime { get; set; } = 1f;
    [field: SerializeField] public float LandingOffset { get; set; } = 1.5f;
    [field: SerializeField] public float JumpDelay { get; set; } = 0.15f;
    [field: SerializeField] public LayerMask GroundLayer { get; set; }
    public Vector2 CheckGroundSize;

    // IdleState Variables
    [Header("Idle Variables")]
    public float IdleRange = 5f;
    public float IdleTimeBetweenMove = 1.5f;

    // Punching Variables
    [Header("Attacking")] 
    public bool canAttack = true;
    public GameObject DetectAttack { get; set; }
    [field: SerializeField] public float AttackRadius { get; set; } = 0.8f;
    [field: SerializeField] public int PunchDamage { get; set; } = 5;
    [field: SerializeField] public Vector2 PunchForce { get; set; } = new Vector2(45, 7);
    public bool ShouldBeDamaging { get; set; } = false;
    [field: SerializeField] public float AttackWait { get; set; } = 0.75f;

    [Header("Knockback Path Tracer")]
    public float PointSpacing = 0.5f;  // Minimum distance between recorded points
    private LineRenderer _lineRenderer;
    private Vector3 _lastRecordedPosition;  // Last recorded position to avoid redundant points
    #endregion

    [Header("VFX")]
    // hurt fx
    public GameObject weakPowPrefab;
    public GameObject strongPowPrefab;
    public float fxRadius = 1.5f;
    public float strongFXThreshold = 10f;
    public float damageToSizeScaling = 0.01f; // dmg = size of vfx

    // hit stun immunity fx
    private SpriteRenderer spriteRenderer;
    private float flashDuration; // total duration of flashing, will be set to hit stun immunity time
    public float flashInterval = 0.15f; // time between flashes

    public float initialSpawnDelay = 1.5f;

    // State Machine Variables
    public EnemyStateMachine.EnemyStates enemyState; // for debug
    public EnemyStateMachine StateMachine { get; set; }

    #region Universal Functions
    // called before start when script is loaded
    protected virtual void Awake()
    {
        // accessing components
        Player = GameObject.FindGameObjectWithTag("Player");
        GameEnemyManager = GameObject.Find("GameEnemyManager").GetComponent<GameEnemyManager>();
        Anim = transform.Find("Sprite").GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        Collider = gameObject.GetComponent<BoxCollider2D>();
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        spriteRenderer = gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        downSlamExplosionTrigger = transform.Find("ExplosionDetection").gameObject;
        DetectAttack = gameObject.transform.Find("DetectAttack").gameObject;
        groundCheck = transform.Find("GroundCheck").gameObject;

        // setting up state machine
        StateMachine = new EnemyStateMachine();
    }

    // called before first frame after all scripts loaded
    protected virtual void Start()
    {
        // setting properties

        // IMoveable
        CurrentHealth = MaxHealth;
        BodyGravity = Mathf.Abs(Physics2D.gravity.y) * RB.gravityScale;
        TopEnemyTransform = transform.position + (Vector3.up * Collider.bounds.extents.y);
        BottomEnemyTransform = transform.position + (Vector3.down * Collider.bounds.extents.y);
        MaxJumpHeight = (Mathf.Pow(MaxYJumpForce, 2) / (2 * BodyGravity)) - Math.Abs(TopEnemyTransform.y - BottomEnemyTransform.y); // Calculate max height AI can jump
        float timeToApex = MaxYJumpForce / BodyGravity;
        MaxJumpDistance = MaxXJumpForce * timeToApex; // Calculate the max horizontal distance AI can jump
        LandingTarget = Vector2.zero;

        // knockback path tracer
        _lastRecordedPosition = transform.position; // Initialize the last recorded position

        // fx
        flashDuration = HitStunImmunityTime;

        // spawned with initial pause delay
        StartCoroutine(PauseAction(initialSpawnDelay));
    }

    protected virtual void Update()
    {
        // updating state
        StateMachine.currentEnemyState.FrameUpdate();
        enemyState = StateMachine.currentEnemyState.id; // for debug

        // updating transforms
        TopEnemyTransform = transform.position + (Vector3.up * Collider.bounds.extents.y);
        BottomEnemyTransform = transform.position + (Vector3.down * Collider.bounds.extents.y);
        PlatformDetectionOrigin = TopEnemyTransform + (Vector3.up * (MaxJumpHeight / 2)) + ((FacingRight ? Vector3.right : Vector3.left) * (MaxJumpDistance / 2));

        // record path during impact
        if (InImpact)
        {
            RB.mass = baseMass * postImpactMassScale;
            if (Vector3.Distance(transform.position, _lastRecordedPosition) > PointSpacing)
                AddPointToPath(transform.position);
        }
        else
        {
            RB.mass = baseMass;
        }

        // If the current hit stun amount is greater than 0, run the out of combat timer
        if (CurrentHitStunAmount > 0)
        {
            outOfCombatTimer -= Time.deltaTime;

            // If the timer reaches zero, reset the hit stun amount
            if (outOfCombatTimer <= 0)
                CurrentHitStunAmount = 0;
        }
    }

    protected virtual void FixedUpdate()
    {
        // exit jump state when landing
        if (RB.velocity.y == 0) MidJump = false;

        // detection checks
        PlayerAbove = (Player.transform.position.y > TopEnemyTransform.y) ? true : false; // check for need to jump to player level
        PlayerBelow = (Player.transform.position.y < BottomEnemyTransform.y) ? true : false;

        if (IsGrounded)
        {
            InKnockup = false;
            if (inDownSlam) StartCoroutine(downSlamExplosionActive());
        }

        IsGrounded = Physics2D.OverlapBox(groundCheck.transform.position, CheckGroundSize, 0f, GroundLayer) && !MidJump;

        // if low velocity, then no longer InImpact
        if (RB.velocity.magnitude < collisionForceThreshold)
        {
            InImpact = false;
            ClearPath();

            // if in hit stun or knockup, don't disable impact bool (will auto be disabled in future frames where hit stun or knockup as ended)
            if (!InHitStun && !InKnockup) Anim.SetBool("ImpactBool", false);
        }

        RB.velocity = Vector2.ClampMagnitude(RB.velocity, maxVelocity); // prob can set clamp in property

        RB.gravityScale = 2.5f;
        if (inAirStun)
        {
            RB.velocity = RB.velocity * 0;
            RB.gravityScale = 0;
        }

        if (!IsPaused && !InImpact && !InKnockup)
            StateMachine.currentEnemyState.PhysicsUpdate();
    }

    private IEnumerator downSlamExplosionActive()
    {
        downSlamExplosionTrigger.SetActive(true);
        yield return null;
        downSlamExplosionTrigger.SetActive(false);
        inDownSlam = false;
    }

    // receiving impact reaction
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
            CurrentOneWayPlatform = collision.gameObject;

        if (InImpact)
        {
            float impactForce = collision.relativeVelocity.magnitude;
            if (impactForce > collisionForceThreshold)
            { // if force > threshold, then deal dmg, otherwise no longer in InImpact state
                int collisionDamage = Mathf.RoundToInt(impactForce * collisionDamageMultiplier); // note: consider log max for extreme cases
                // bounce off surfaces, not enemies
                if (!collision.gameObject.CompareTag("enemy"))
                { 
                    Vector2 bounceDirection = collision.contacts[0].normal;
                    // if not one way platform or if hitting one way platform from above (the only allowed bounce, otherwise just go through it)
                    if (!collision.gameObject.CompareTag("OneWayPlatform") || (collision.gameObject.CompareTag("OneWayPlatform") && bounceDirection == Vector2.up))
                    {
                        Damage(collisionDamage, HitStunTime);

                        if (Math.Abs(bounceDirection.x) > 0) FlipCharacter(bounceDirection.x < 0);
                        RB.AddForce(bounceDirection * (impactForce * collisionForceMultiplier), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
    #endregion
    #region Movement
    public void FlipCharacter(bool right)
    {
        // storing whether object is already facingRight to avoid double flipping
        if (right != FacingRight)
        {
            FacingRight = !FacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    // Helper function to encapsulate the pause logic
    public IEnumerator PauseAction(float delay)
    {
        if (!IsPaused)
        {
            IsPaused = true;
            Anim.SetBool("isWalking", false);
            yield return new WaitForSeconds(delay);
            IsPaused = false;
        }
    }

    // walk in x direction toward target
    public void WalkToTarget(Vector2 target)
    {
        float stoppingDistance = 0.1f; // to prevent enemy glitching out on same spot
        Vector2 dist = target - (Vector2)transform.position;
        int xDirection = dist.x == 0 ? 0 : (dist.x > 0 ? 1 : -1);
        bool isOutOfReach = Math.Abs(dist.x) > stoppingDistance;

        RB.velocity = new Vector2(isOutOfReach ? xDirection * ChaseSpeed : 0, RB.velocity.y);
        FlipCharacter(isOutOfReach ? xDirection > 0 : FacingRight); // Maintain direction if idle
        Anim.SetBool("isWalking", isOutOfReach);
    }

    // find x and y jump force needed to reach landingPosition
    public Vector2 CalculateJumpForce(Vector2 landingPosition)
    {
        // Horizontal and vertical distances
        float deltaX = landingPosition.x - transform.position.x;
        float deltaY = Math.Abs(landingPosition.y - BottomEnemyTransform.y);

        // Calculate the vertical velocity needed to reach the platform height
        float verticalVelocity = Math.Min(Mathf.Sqrt(2 * BodyGravity * deltaY), MaxYJumpForce);
        // Time to reach the apex (top of the jump) at platform height
        float timeToApex = verticalVelocity / BodyGravity;
        // Calculate the horizontal velocity needed to reach the platform during that time
        float horizontalVelocity = (deltaX > 0 ? 1 : -1) * Math.Min(MaxXJumpForce, Math.Abs(deltaX) / timeToApex);

        // Return the calculated initial velocity as a 2D vector (x, y)
        return new Vector2(horizontalVelocity, verticalVelocity);
    }

    // look for platforms within detection box (max jump dist/height) and find furthest landing target within max jump
    public void DetectTargetFromPlatform()
    {
        Vector2 boxSize = new Vector2(MaxJumpDistance, MaxJumpHeight);
        Collider2D[] hits = Physics2D.OverlapBoxAll(PlatformDetectionOrigin, boxSize, 0f, PlatformDetectionMask);
        Debug.Log(PlatformDetectionOrigin);
        LandingTarget = Vector2.zero; // special val
        if (hits.Length == 0)
        { // No platforms detected, exit early
            return;
        }
        else
        { // look for platform furthest from enemy
            float maxDistance = float.MinValue;
            Collider2D furthestPlatform = null;
            foreach (var hit in hits)
            {
                // Check if the hit object has the "OneWayPlatform" tag
                if (hit.CompareTag("OneWayPlatform"))
                {
                    Debug.Log(hit.gameObject.name);
                    Debug.Log(hit.gameObject.transform.position);
                    // Calculate the distance between the enemy and the hit platform
                    float distance = Vector2.Distance(BottomEnemyTransform, hit.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        furthestPlatform = hit;
                    }
                }
            }

            // Get the collider bounds of the furthest platform
            if (furthestPlatform != null)
            {
                Bounds platformBounds = furthestPlatform.bounds;
                float platformXDist = (FacingRight ? platformBounds.max.x : platformBounds.min.x) - RB.transform.position.x;
                float jumpX = (platformXDist > 0 ? 1 : -1) * Math.Min(Math.Abs(platformXDist), MaxJumpDistance);
                LandingTarget = new Vector2(
                    RB.transform.position.x + jumpX,
                    platformBounds.max.y
                );
            }
        }
    }

    // for falling through platforms
    public IEnumerator DisableCollision()
    {
        Collider2D platformCollider = CurrentOneWayPlatform.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(Collider, platformCollider);
        yield return new WaitForSeconds(FallthroughTime);
        Physics2D.IgnoreCollision(Collider, platformCollider, false);
    }
    #endregion

    #region Health/Die Functions
    // refers to GM bc to apply card effects
    // if not player attack, shouldn't affect hit stun effects
    public void Damage(int damage, float hitStunTime)
    {
        GameEnemyManager.Damage(this, damage, hitStunTime);
    }

    private Coroutine hitStunCoroutine;
    // actual damage function that GM will reference
    public void DamageHelper(int damage, float hitStunTime)
    {
        CurrentHealth -= damage;

        SpawnDamageVFX(damage);

        if (CurrentHealth <= 0 && !gameObject.IsDestroyed())
            Die();

        // hit stun limit, if over limit, then become immune to fx of hit stun (not being able to attack) and player knockback for a bit
        // NOTE: still able to take dmg
        // CurrentHitStunAmount += hitstun;
        // if (CurrentHitStunAmount > HitStunLimit && !HitStunImmune)
        // {
        //     HitStunImmune = true;
        //     StartCoroutine(HitStunImmunity(HitStunImmunityTime));
        //     StartCoroutine(HitStunImmunityFlash(flashDuration));
        // }

        if (!HitStunImmune)
        {
            // anim set to impact to cause anim lock
            Anim.SetTrigger("ImpactTrigger");
            Anim.SetBool("ImpactBool", true);
            if (hitStunCoroutine != null)
                StopCoroutine(hitStunCoroutine);
            hitStunCoroutine = StartCoroutine(HitStun(hitStunTime));
        }

        // outOfCombatTimer = outOfCombatDuration;
    }

    // specific hit state (for punches), if in this state then temp can't attack
    private IEnumerator HitStun(float time)
    {
        InHitStun = true;
        yield return new WaitForSeconds(time);
        InHitStun = false;
        hitStunCoroutine = null;
    }

    private IEnumerator HitStunImmunity(float time)
    {
        yield return new WaitForSeconds(time);
        HitStunImmune = false;
        CurrentHitStunAmount = 0;
    }

    public void Die()
    {
        lock (DeathLock) // Ensure only one thread executes this block at a time
        {
            if (!IsDead)
            {
                IsDead = true;
                GameEnemyManager.Death(gameObject); // call this one cause list with enemies needs to be updated, this one calls Destroy too
            }
        }
    }

    public void TakeKick(int damage, Vector2 force)
    {
        if (force.x < 0)
            FlipCharacter(true);
        else if (force.x > 0)
            FlipCharacter(false);

        // no force if in immunity
        if (!HitStunImmune)
        {
            InImpact = true;

            if (IsGrounded)
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f); // slight upward translate to enable bounce from ground

            RB.velocity = Vector2.zero; // so previous velocity doesn't interfere
            RB.AddForce(force, ForceMode2D.Impulse);
        }

        Damage(damage, HitStunTime);
    }

    private Coroutine airStunCoroutine;

    public void TakePunch(int damage, float airStunTime)
    {
        Damage(damage, HitStunTime);
        if (airStunCoroutine != null)
            StopCoroutine(airStunCoroutine);
        if (airStunTime > 0)
            airStunCoroutine = StartCoroutine(AirStun(airStunTime));
        
    }

    // for air combos, stops them mid air
    private IEnumerator AirStun(float airStunTime) 
    {
        inAirStun = true;
        yield return new WaitForSeconds(airStunTime);
        inAirStun = false;
        airStunCoroutine = null;
    }

    public void TakeUppercut(int damage, Vector2 force)
    {
        if (force.x < 0)
            FlipCharacter(true);
        else if (force.x > 0)
            FlipCharacter(false);

        // no knockup if in immunity
        if (!HitStunImmune)
        {
            InKnockup = true;

            if (IsGrounded) 
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f); // slight upward translate to prevent isGrounded autotriggering
            IsGrounded = false;

            RB.velocity = Vector2.zero; // so previous velocity doesn't interfere
            RB.AddForce(force, ForceMode2D.Impulse);
        }

        Damage(damage, HitStunTime);
    }

    public void StopAttack()
    {
        Anim.SetBool("isPunching", false);
        EndShouldBeDamaging();
    }
    #endregion

    #region Knockback Path Tracing
    // Method to add a point to the LineRenderer
    private void AddPointToPath(Vector3 newPoint)
    {
        // Update the number of points in the LineRenderer
        _lineRenderer.positionCount += 1;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newPoint);

        // Update the last recorded position
        _lastRecordedPosition = newPoint;
    }

    private void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }
    #endregion

    #region Animation Triggers
    public void AnimationTriggerEvent(EnemyState.AnimationTriggerType triggerType)
    {
        StateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(groundCheck.transform.position, CheckGroundSize); // isGrounded
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireCube(PlatformDetectionOrigin, new Vector2(MaxJumpDistance, MaxJumpHeight)); // jump detection box
        Gizmos.DrawLine(BottomEnemyTransform, LandingTarget); // visual for where jumping landing target is

        // hitbox
        if (DetectAttack)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(DetectAttack.transform.position, AttackRadius);
        }

        if (downSlamExplosionTrigger != null && downSlamExplosionTrigger.activeSelf)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(downSlamExplosionTrigger.transform.position, downSlamExplosionTrigger.GetComponent<CircleCollider2D>().radius);
        }
    }
    #endregion

    #region Punching
    // punch active frames
    public IEnumerator Punch()
    {
        ShouldBeDamaging = true;
        while (ShouldBeDamaging && !InHitStun)
        {
            Collider2D player = Physics2D.OverlapCircle(DetectAttack.transform.position, AttackRadius, 1 << Player.layer);
            if (player != null)
            {
                Vector2 force = new Vector2((FacingRight ? 1 : -1) * Math.Abs(PunchForce.x), PunchForce.y);
                player.GetComponent<PlayerController>().TakeDamage(PunchDamage, force);
            }
            yield return null; // wait a frame
        }
    }

    // end punch active frames
    public void EndShouldBeDamaging()
    {
        ShouldBeDamaging = false;
    }

    // set end of animation
    public void EndPunch()
    {
        Anim.SetBool("isPunching", false);
        StartCoroutine(PauseAction(AttackWait));
    }
    #endregion

    #region VFX
    // flashing effect for hit immunity
    private IEnumerator HitStunImmunityFlash(float flashDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            // Toggle the sprite's visibility
            SetSpriteColor(spriteRenderer.color.a == 1f ? 0.5f : 1f);

            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }
        // reset vfx
        SetSpriteColor(1f);
    }

    // Helper method to set the alpha value of the sprite
    private void SetSpriteColor(float val)
    {
        Color dullColor = new Color(val, val, val, val);
        spriteRenderer.color = dullColor;
    }

    // spawning damage vfx
    private void SpawnDamageVFX(int damage)
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * fxRadius;
        Vector3 spawnPosition = new Vector3(randomPoint.x, randomPoint.y, 0) + transform.position;
        GameObject newVFX = Instantiate(damage >= strongFXThreshold ? strongPowPrefab : weakPowPrefab, spawnPosition, Quaternion.identity);
        // make vfx 'weaker' if enemy was hit while hit stun immune
        if (HitStunImmune)
            newVFX.GetComponent<SpriteRenderer>().color = newVFX.GetComponent<SpriteRenderer>().color * 0.5f;
        Vector3 newSize = newVFX.transform.localScale;
        newSize.x += damage * damageToSizeScaling;
        newSize.y += damage * damageToSizeScaling;
        newVFX.transform.localScale = newSize;
    }
    #endregion
}
