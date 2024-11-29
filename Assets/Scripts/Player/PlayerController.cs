using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Player p;
    public HashSet<IDamageable> iDamageableSet = new HashSet<IDamageable>();
    public bool shouldBeDamaging { get; private set; } = false;
    AudioManager audioManager;

    private void Awake()
    {
        p.GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        if (p.GM == null)
            p.GM = GameManager.instance;
        p.spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        p.playerChargeMeter = GameObject.Find("Player Charge Meter");
        p.playerExtendedChargeMeter = GameObject.Find("Player Extended Charge Meter");
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!(SceneManager.GetActiveScene().name == "TUTORIAL" || SceneManager.GetActiveScene().name == "SHOP"))
        {
            // p.GM.Difficulty(); // probably delete this whole if eventually, not sure yet tho
        }

        // setting defaults
        p.GM.healthCurrent = p.GM.healthMax; // Set health to max at start
        p.GM.healthBar.maxValue = p.GM.healthMax;
        p.GM.healthBar.value = p.GM.healthCurrent;

        // charge kick values
        p.playerChargeMeter.GetComponent<Slider>().value = 0;
        p.playerChargeMeter.SetActive(false); // hide
        p.playerChargeMeter.GetComponent<Slider>().maxValue = 1;

        p.playerExtendedChargeMeter.GetComponent<Slider>().value = 0; // reset value
        p.playerExtendedChargeMeter.SetActive(false); // hide
        p.forceIncrease = p.maxKickForce - p.heavyKickForce;
        // below is setting seondary bar to be proportionally larger than original bar based on extended max force
        float extendedPercent = (p.extendedMaxKickForce - p.heavyKickForce) / p.forceIncrease; // 1 + percent increase
        p.playerExtendedChargeMeter.GetComponent<Slider>().maxValue = extendedPercent;
        p.playerExtendedChargeMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(
            p.playerChargeMeter.GetComponent<RectTransform>().sizeDelta.x * extendedPercent,
            p.playerExtendedChargeMeter.GetComponent<RectTransform>().sizeDelta.y);
        p.kickChargeRate = 1f / p.maxChargeTime; // calc charge rate needed to reach desired time

        // accessing components
        p.rb = GetComponent<Rigidbody2D>();
        p.anim = GetComponent<Animator>();


        if (p.GM.iOSPanel && p.GM.iOSPanel.activeSelf)
        {
            p.MovementJoystickScript = p.GM.iOSPanel.GetComponent<MovementJoystick>();
            p.ButtonsAndClickScript = p.GM.iOSPanel.GetComponent<ButtonsAndClick>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (p.ControlsEnabled)
        {
            if (p.GM.mobile)
            {
                ProcessInputMobile();
                p.anim.SetBool("midJump", p.midJump);
                DirectionPlayerFacesMobile();
            }
            else
            {
                ProcessInput();
                // if not grappling then follow this rule, otherwise follow grappling rule
                if (!p.grapplingGun.isGrappling)
                    p.anim.SetBool("midJump", p.midJump);
                // if (p.grapplingGun.isGrappling)
                //     p.anim.SetBool("midJump", true);
                DirectionPlayerFaces();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) p.GM.Pause();
        p.playerChargeMeter.GetComponent<Slider>().value = p.kickCharge; // just in case
        p.playerExtendedChargeMeter.GetComponent<Slider>().value = p.kickCharge;
    }

    private void FixedUpdate()
    {
        if (p.rb.velocity.y == 0) p.midJump = false;

        // if is Grounded and just ended midJump, then play landing animation
        p.isGrounded = Physics2D.OverlapBox(p.groundCheck.position, p.checkGroundSize, 0f, p.groundObjects) && !p.midJump;
        Move();

        if (p.charging)
        {
            if (p.grapplingGun.isGrappling)
                p.GM.ChecklistScript.UpdateChecklistItem("SCK", true);

            p.kickCharge += (p.kickChargeRate * Time.deltaTime) + (p.rb.velocity.magnitude * p.movementChargeRateMultiplier); // charging goes up naturally + extra for moving fast
            // if grappling, use extended max, else regular max
            float max = p.grapplingGun.isGrappling ? p.playerExtendedChargeMeter.GetComponent<Slider>().maxValue : 1f;
            p.kickCharge = Mathf.Clamp(p.kickCharge, 0, max);
        }
    }

    //helper method called in p.GM, resets players damage to base levels 
    //and also player speed i guess LOL
    public void resetPlayerDamage()
    {
        p.kickDamage = p.baseKickDamage;
        p.punchDamage = p.basePunchDamage;
        p.uppercutDamage = p.baseUppercutDamage;
        p.moveSpeed = p.baseMoveSpeed;
    }

    #region Movement
    private void Move()
    {
        // if grapping, lower grav
        // otherwise, if falling set higher grav for snappiness
        p.rb.gravityScale = (p.rb.velocity.y < 0) ? p.fallingGravity : p.defaultGravity;
        if (p.grapplingGun.isGrappling) p.rb.gravityScale = 1f;

        // if !isGrounded and !isGrappling
        float adjustAirControl = 1;
        if (!p.isGrounded) adjustAirControl = !p.grapplingGun.isGrappling ? p.airControl : p.grappleAirControl;

        // for slight momentum during kick charge        
        p.friction = p.anim.GetBool("isKicking") ? 0.9f : p.baseFriction;

        // if in air/grappling, maintain prev x for momentum (else add ground friction), add directed movement with air control restrictions
        float xVelocity = (p.rb.velocity.x * (!p.isGrounded || p.grapplingGun.isGrappling ? 1 : p.friction))
            + (p.moveDirection * p.moveSpeed * adjustAirControl);
        p.rb.velocity = new Vector2(Mathf.Clamp(xVelocity, -p.XMaxSpeed, p.XMaxSpeed),
            Mathf.Min(p.rb.velocity.y, p.YMaxSpeed));

        if (p.isGrounded)
        {
            if (Mathf.Abs(p.rb.velocity.x) > 0.1f)
                FindObjectOfType<AudioManager>().PlayFootsteps(FindObjectOfType<AudioManager>().footsteps);
            else
                FindObjectOfType<AudioManager>().StopFootsteps();
        }
        else
        {
            FindObjectOfType<AudioManager>().StopFootsteps();
        }

        p.anim.SetBool("isWalking", Mathf.Abs(p.rb.velocity.x) > 0.1f);
        if (p.isJumping)
        {
            EndShouldBeDamaging();
            p.rb.velocity = new Vector2(p.rb.velocity.x, 0f);
            p.rb.AddForce(new Vector2(0f, p.jumpForce));
            p.midJump = true;
        }
        p.isJumping = false;

        // if punching in air, then freeze
        if (p.anim.GetBool("inAirCombo"))
        {
            p.rb.velocity = p.rb.velocity * 0;
            p.rb.gravityScale = 0;
        }
    }

    public void FlipCharacter(bool right)
    {
        // storing whether object is already facingRight to avoid double flipping
        if (right != p.facingRight)
        {
            p.facingRight = !p.facingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void DirectionPlayerFaces()
    {
        Vector2 mousePos = p.grapplingGun.m_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mousePos - (Vector2)p.grapplingGun.firePoint.position).normalized;

        // when kicking, face player towards cursor to make attack easier
        if (p.anim.GetBool("isKicking") || p.anim.GetBool("isPunching"))
            FlipCharacter(aimDirection.x > 0);
        // Handle character flipping only based on movement when moving
        else if (p.moveDirection != 0)
            FlipCharacter(p.moveDirection > 0);
        // If not moving, flip character based on aim direction
        else
            FlipCharacter(aimDirection.x > 0);
    }

    private void DirectionPlayerFacesMobile()
    {
        // Handle character flipping only based on movement when moving
        if (p.moveDirection != 0) FlipCharacter(p.moveDirection > 0);
    }
    #endregion

    #region Input
    private void ProcessInput()
    {
        // Normal Movement Input
        // scale of -1 -> 1
        if (Input.GetKeyDown(KeyCode.Space) && p.isGrounded)
        {
            audioManager.PlaySFX(audioManager.jump);
            p.isJumping = true;
            p.anim.SetTrigger("PressJump");
        }

        if (p.anim.GetBool("canMove") || !p.isGrounded)
        {
            p.moveDirection = Input.GetAxis("Horizontal");
            
            if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && p.currentOneWayPlatform != null)
                StartCoroutine(DisableCollision());
        }
        else
        {
            p.moveDirection = 0;
        }

        HandleAttackInput();

        HandleGrappleInput();

        //card drawing - TODO: ADD COOLDOWN (in battle manager maybe?)
        if (Input.GetKeyDown(KeyCode.F)) p.GM.useCard();
        //if (Input.GetKeyDown(KeyCode.Escape)) p.GM.Pause();
    }

    private void HandleAttackInput()
    {
        // punching
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.K) && !p.isHit)
        {
            p.anim.SetTrigger("punch");
            p.anim.SetBool("isPunching", true);
            p.anim.SetBool("isKicking", false);
            p.anim.ResetTrigger("PressJump");

            if (p.grapplingGun.isGrappling)
            {
                // do super upper cut instead
                p.anim.SetBool("midJump", false);
                p.anim.SetTrigger("SuperUppercut");
            }
        }

        // kick
        if (Input.GetKeyDown(KeyCode.Q) && !p.anim.GetBool("isKicking") && !p.isHit)
        {
            p.anim.SetBool("isKicking", true);
            p.anim.ResetTrigger("PressJump");
        }

        // while not holding down button to charge, set back to normal
        if (Input.GetKey(KeyCode.Q) && p.anim.GetBool("isKicking") && !p.isHit && p.charging)
        {
            p.playerChargeMeter.SetActive(true);
            p.anim.speed = 0; // pause anim
        }
        else
        {
            p.charging = false;
            p.anim.speed = 1; // unpause anim
            p.playerChargeMeter.SetActive(false);
            p.friction = p.baseFriction;
        }
    }

    private void HandleGrappleInput()
    {
        // start grappling
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.J)) p.grapplingGun.SetGrapplePoint();
        // pull yourself towrds object
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.J))
        {
            p.grapplingGun.pull();
            if (p.charging) p.playerExtendedChargeMeter.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.J))
        {
            p.grapplingGun.stopGrappling();
        }
        else
        {
            p.playerExtendedChargeMeter.SetActive(false);
        }
        // Pull enemies towaards you (need to work on)
        if (Input.GetKeyDown(KeyCode.E)) p.grapplingGun.PullEnemy();
        if (Input.GetKeyUp(KeyCode.E)) p.grapplingGun.StopPullingEnemy();
    }

    private void ProcessInputMobile()
    {
        // Normal Movement Input
        // scale of -1 -> 1
        p.moveDirection = p.MovementJoystickScript.joystickVec.x;

        if (p.ButtonsAndClickScript.isJumping && p.isGrounded)
        {
            p.isJumping = true;
            p.ButtonsAndClickScript.isJumping = false; // so that jumping is not spammed
        }
        if (p.MovementJoystickScript.joystickVec.normalized.y < -0.90 && p.currentOneWayPlatform != null)
            StartCoroutine(DisableCollision());

        // attacks
        if (p.ButtonsAndClickScript.isKicking)
        {
            p.anim.SetBool("isKicking", true);
            p.ButtonsAndClickScript.isKicking = false;
        }
        // Grappling hook Input
        //if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.J)) p.grapplingGun.SetGrapplePoint();
        if (p.ButtonsAndClickScript.pulling) p.grapplingGun.pull();
        //else if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.J)) p.grapplingGun.stopGrappling();

        if (p.ButtonsAndClickScript.drawCard)
        {
            p.GM.useCard();
            p.ButtonsAndClickScript.drawCard = false;
        }
        if (p.ButtonsAndClickScript.pause)
        {
            p.GM.Pause();
            p.ButtonsAndClickScript.pause = false;
        }
    }
    #endregion

    #region Kick Anim Calls
    public void StartCharging()
    {
        p.charging = true;
        p.anim.SetBool("inAirCombo", false);
    }

    public enum TypeOfKick
    {
        FastKick,
        HeavyKick
    }
    // kick active frames
    public IEnumerator Kick(TypeOfKick type)
    {

        shouldBeDamaging = true;

        // calculate kick force properties
        int dir = p.facingRight ? 1 : -1;
        float chargeIncrease = p.kickCharge * p.forceIncrease;
        float baseKickForce = type == TypeOfKick.FastKick ? p.fastKickForce : p.heavyKickForce;
        float weightedXForce = dir * (baseKickForce + chargeIncrease);
        Vector2 force = new Vector2(weightedXForce, 0);
        float weightedYForce = p.kickUpForce + (chargeIncrease * p.chargeUpForceMultiplier);

        // if grounded or moving up, kick upward, else downward
        force.y = ((p.isGrounded || p.rb.velocity.y >= 0) ? 1 : -1) * weightedYForce;

        // modding if downward force to replicate feeling that upward force grants
        if (force.y < 0)
        {
            float magnitude = force.magnitude;
            float adjustedX = dir * ((float)Math.Sqrt(Math.Pow(magnitude, 2) - Math.Pow(force.y * 0.5, 2)));
            force = new Vector2(adjustedX, force.y * 0.5f);
        }

        float kickRadius = p.kickRadius;
        // if (p.kickCharge > 1f) { // during grapple
        //     float t = Mathf.Clamp((p.kickCharge - 1f) / (p.playerExtendedChargeMeter.GetComponent<Slider>().maxValue - 1f), 0f, 1f);
        //     kickRadius = Mathf.Lerp(p.kickRadius, p.extendedChargeRadius, t);
        // }

        // stop grappling if attacking
        p.grapplingGun.stopGrappling();

        if (type == TypeOfKick.FastKick)
            p.GM.ChecklistScript.UpdateChecklistItem("PKC", true);
        if (type == TypeOfKick.HeavyKick)
        {
            if (Math.Abs(weightedXForce) < p.maxKickForce)
            {
                p.GM.ChecklistScript.UpdateChecklistItem("TK", true);
            }
            if (Math.Abs(weightedXForce) == p.maxKickForce)
                p.GM.ChecklistScript.UpdateChecklistItem("FCK", true);
        }

        while (shouldBeDamaging)
        {
            Collider2D[] enemyList = Physics2D.OverlapCircleAll(p.kickPoint.transform.position, kickRadius, p.enemyLayer);

            foreach (Collider2D enemyObject in enemyList)
            {
                // calculate direction of force and factor in player velocity to overall power
                // Vector2 dir = enemyObject.transform.position - transform.position;
                // dir.Normalize();
                // float weightedForce = p.kickForce + (p.rb.velocity.magnitude + enemyObject.GetComponent<Rigidbody2D>().velocity.magnitude) * p.movementForceMultiplier;
                // float weightedUpForce = p.kickUpForce + (p.rb.velocity.magnitude + enemyObject.GetComponent<Rigidbody2D>().velocity.magnitude) * p.movementUpForceMultiplier;

                // apply damage + force to enemy 
                IDamageable iDamageable = enemyObject.GetComponent<IDamageable>();
                if (iDamageable != null && !iDamageableSet.Contains(iDamageable))
                {
                    int extraDamage = (int)(p.kickCharge * p.kickChargeMaxDamage);
                    iDamageable.TakeKick(p.kickDamage + extraDamage, force);
                    iDamageable.StopAttack(); // cancel enemy attack
                    iDamageableSet.Add(iDamageable);
                }
            }
            yield return null; // wait a frame
        }

        // post active-frame processing
        if (iDamageableSet.Count == 0)
        {
            p.GM.audioSource.clip = p.GM.MissAudio;
            p.GM.audioSource.Play();
        }
        else
        {
            p.GM.audioSource.clip = p.GM.KickAudio;
            p.GM.audioSource.Play();

            if (force.magnitude > p.hitStopForceThreshold)
            {
                StartCoroutine(p.GM.HitStop(force.magnitude * p.hitStopScaling));
                StartCoroutine(p.GM.ScreenShake(force.magnitude * p.hitStopScaling, force.magnitude * p.screenShakeScaling));
            }
        }
        iDamageableSet.Clear();
    }

    // set end of kick/punch active frames
    public void EndShouldBeDamaging()
    {
        shouldBeDamaging = false;
        p.kickCharge = 0; // reset charge
    }

    // set end of animation
    public void EndKick()
    {
        p.anim.SetBool("isKicking", false);
        p.anim.SetBool("isPunching", false);
        p.anim.ResetTrigger("punch");
    }
    #endregion

    #region Punch Anim Calls
    public enum TypeOfPunch
    {
        Jab,
        Uppercut,
        SuperUppercut,
        DownSlam
    }

    public IEnumerator PunchCombo(TypeOfPunch partOfCombo)
    {
        if (partOfCombo == TypeOfPunch.SuperUppercut)
            p.GM.ChecklistScript.UpdateChecklistItem("GP", true);
        if (partOfCombo == TypeOfPunch.Uppercut)
            p.GM.ChecklistScript.UpdateChecklistItem("PC", true);
        if (partOfCombo == TypeOfPunch.DownSlam)
            p.GM.ChecklistScript.UpdateChecklistItem("PAC", true);


        shouldBeDamaging = true;
        int dir = p.facingRight ? 1 : -1;

        // use special larger radius if doing anything but jabs
        float radius = partOfCombo != TypeOfPunch.Jab ? p.uppercutRadius : p.punchRadius;
        if (p.isGrounded)
        {
            Vector2 force = p.forwardPunchMovement;
            force.x = Mathf.Abs(force.x) * (p.facingRight ? 1 : -1);
            p.rb.AddForce(force);
        }

        // stop grappling if attacking
        p.grapplingGun.stopGrappling();

        while (shouldBeDamaging)
        {
            Collider2D[] enemyList = Physics2D.OverlapCircleAll(p.punchPoint.transform.position, radius, p.enemyLayer);

            foreach (Collider2D enemyObject in enemyList)
            {
                // apply damage + force to enemy 
                IDamageable iDamageable = enemyObject.GetComponent<IDamageable>();
                if (iDamageable != null && !iDamageableSet.Contains(iDamageable))
                {
                    // knock up if uppercut
                    if (partOfCombo == TypeOfPunch.Uppercut)
                    {
                        audioManager.PlaySFX(audioManager.uppercut);
                        Vector2 force = p.uppercutForce;
                        force.x = Mathf.Abs(p.uppercutForce.x) * dir;
                        iDamageable.TakeUppercut(p.uppercutDamage, force);
                    }
                    else if (partOfCombo == TypeOfPunch.SuperUppercut)
                    {
                        audioManager.PlaySFX(audioManager.uppercut);
                        Vector2 force = p.superUppercutForce;
                        force.x = Mathf.Abs(p.superUppercutForce.x) * dir;
                        iDamageable.TakeUppercut(p.uppercutDamage, force);

                    }
                    else if (partOfCombo == TypeOfPunch.DownSlam)
                    {
                        Vector2 force = p.downSlamForce;
                        force.x = Mathf.Abs(p.downSlamForce.x) * dir;
                        iDamageable.TakeKick(p.downSlamDamage, force);
                        iDamageable.inDownSlam = true;
                    }
                    else
                    { // regular punch otherwise (apply slow down)
                        audioManager.PlaySFX(audioManager.punch);
                        iDamageable.TakePunch(p.punchDamage, p.airStunTime);
                    }

                    iDamageable.StopAttack(); // cancel enemy attack
                    iDamageableSet.Add(iDamageable);
                }
            }
            yield return null; // wait a frame
        }

        // post active-frame processing
        if (iDamageableSet.Count == 0) 
        {
            p.GM.audioSource.clip = p.GM.MissAudio;
            p.GM.audioSource.Play();
            p.anim.SetBool("inAirCombo", false);
        } 
        else if (iDamageableSet.Count > 0 && !p.isGrounded)
        {
            p.anim.SetBool("inAirCombo", true);
        }
        
        
        //     p.GM.audioSource.clip = p.GM.KickAudio;
        //     p.GM.audioSource.Play();

        // if (force.magnitude > p.hitStopForceThreshold) {
        //     StartCoroutine(p.GM.HitStop(force.magnitude * p.hitStopScaling));
        //     StartCoroutine(p.GM.ScreenShake(force.magnitude * p.hitStopScaling, force.magnitude * p.screenShakeScaling));
        // }
        // }
        if (partOfCombo != TypeOfPunch.Jab && iDamageableSet.Count > 0)
        {
            StartCoroutine(p.GM.HitStop(p.uppercutForce.magnitude * p.hitStopScaling));
            StartCoroutine(p.GM.ScreenShake(p.uppercutForce.magnitude * p.hitStopScaling, p.uppercutForce.magnitude * p.screenShakeScaling));
        }
        iDamageableSet.Clear();
    }

    // set end of animation
    public void EndPunch()
    {
        p.anim.SetBool("isKicking", false);
        p.anim.SetBool("isPunching", false);
    }
    #endregion

    #region One Way Platforms
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("OneWayPlatform")) p.currentOneWayPlatform = other.gameObject;
        if (other.gameObject.CompareTag("Explosion")) Debug.Log("PLAYER LETS GO");
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = p.currentOneWayPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(p.playerCollider, platformCollider);
        yield return new WaitForSeconds(p.waitTime);
        Physics2D.IgnoreCollision(p.playerCollider, platformCollider, false);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float kickRadius = p.kickRadius;
        if (p.kickCharge > 1f)  // during grapple 
        {
            float t = Mathf.Clamp((p.kickCharge - 1f) / (p.playerExtendedChargeMeter.GetComponent<Slider>().maxValue - 1f), 0f, 1f);
            kickRadius = Mathf.Lerp(p.kickRadius, p.extendedChargeRadius, t);
        }
        Gizmos.DrawWireSphere(p.kickPoint.transform.position, kickRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(p.punchPoint.transform.position, p.punchRadius);
        Gizmos.DrawWireSphere(p.punchPoint.transform.position, p.uppercutRadius);

        Gizmos.color = Color.grey;
        // Gizmos.DrawWireSphere(groundCheck.transform.position, checkRadius);
        // isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(2, 2), 0f);
        Gizmos.DrawCube(p.groundCheck.position, p.checkGroundSize);
    }

    #region Take Damage
    // Function to take damage + iframes + knockback
    public void TakeDamage(int damage, Vector2 force)
    {
        if (!p.isHit && damage > 0)
        {
            p.isHit = true; // for iframes
            p.GM.healthCurrent -= p.vulnerability * damage;

            // fx
            audioManager.PlaySFX(audioManager.playerHit);
            StartCoroutine(p.GM.HurtFlash());

            // if you get hurt, cancel attacks
            p.anim.SetBool("isKicking", false);
            p.anim.SetBool("isPunching", false);
            p.anim.SetBool("inAirCombo", false);
            EndShouldBeDamaging();
            p.playerChargeMeter.SetActive(false);
            p.grapplingGun.stopGrappling();

            // knock player back a bit
            p.rb.velocity = Vector2.zero; // so previous velocity doesn't interfere (would super stop player momentum tho? maybe change in future)
            p.rb.AddForce(force, ForceMode2D.Impulse);

            p.GM.updateHealth();

            //player didn't die, yay! iframes and hitstun sep to allow playermovement after hitstun while invic (get out of crowds)
            float time = damage * p.hitStunDamageMultiplier;
            StartCoroutine(WaitForHitStun(time));
            StartCoroutine(WaitForIframes(time));
        }
    }

    private IEnumerator WaitForIframes(float extraTime)
    {
        Material originalMat = p.spriteRenderer.material;

        p.isHit = true;
        //p.spriteRenderer.material = p.dullColor;
        yield return new WaitForSeconds(p.iFramesTime + extraTime);
        p.isHit = false;
        p.spriteRenderer.material = originalMat;
    }

    private IEnumerator WaitForHitStun(float extraTime)
    {
        p.anim.SetBool("isHurt", true);
        p.anim.SetBool("canMove", false);
        p.anim.SetBool("inAirCombo", false);
        yield return new WaitForSeconds(p.hitStunTime + extraTime);
        p.anim.SetBool("isHurt", false);
        p.anim.SetBool("canMove", true);
    }
    #endregion

    public void SetControls(bool status)
    {
        p.ControlsEnabled = status;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IDamageable enemyScript = collider.gameObject.GetComponent<IDamageable>();
            //enemyScript.Die();
        }
    }
}
