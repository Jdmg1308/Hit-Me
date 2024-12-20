using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Player
{
    // Game Manager
    public GameManager GM = null;
    public SpriteRenderer spriteRenderer;

    [Header("Controls")]
    public bool ControlsEnabled = true;
    public MovementJoystick MovementJoystickScript;
    public ButtonsAndClick ButtonsAndClickScript;

    [Header("Health")]
    public bool isHit = false;
    public int attackCharge;
    public int healCharge;
    public int bleedValue;
    public int vulnerability = 1; // when fragile/vulnerable, take extra damage ex 2x 
    public float iFramesTime = .7f;
    public float hitStunTime = .5f;
    [Range(0, 1)] public float hitStunDamageMultiplier; // how much damage contributes to amount of hit stun/iframes

    public Animator anim;

    [Header("Bank Account")]
    public float money = 0f;
    
    // grappling
    [Header("Grappling")]
    public GrapplingRope grappleRope;
    public GrapplingGun grapplingGun;
    public SpringJoint2D m_springJoint2D;
    public float XMaxSpeed = 20f, YMaxSpeed = 30f;

    // player movement
    [Header("Player Movement")]
    public float moveSpeed;
    public float baseMoveSpeed;
    public float jumpForce, moveDirection;
    public bool isJumping, facingRight = false, isGrounded, midJump;
    public float defaultGravity = 2.5f; // base gravity when jumping
    public float fallingGravity = 3.5f; // set higher gravity when falling for less floatiness
    public Transform groundCheck;
    public LayerMask groundObjects;
    public Vector2 checkGroundSize; // width height of ground check box
    public Rigidbody2D rb;
    public GameObject currentOneWayPlatform;
    [SerializeField] public BoxCollider2D playerCollider;
    public float waitTime = 0f;
    [Range(0, 1)] public float airControl, grappleAirControl, baseFriction, friction; 
    // degree of player air control, air grappling control, and friction on surfaces
    
    // attack tuning
    [Header("Attack Tuning")]
    public GameObject kickPoint;

    // properties of actual kick
    public float fastKickForce; // weak kick force for fast kick, static and sep from charging
    public float heavyKickForce = 0.5f; // non-charged kick force for heavy attack, used for charge calculations
    public float maxKickForce; // non-extended fully charged kick force
    public float forceIncrease; // diff increase in force from base to max (private)
    public float extendedMaxKickForce; // extended fully charged kick force (for grapple kick) (determines amount of extension relative to maxKickForce)
    public float kickUpForce; // base up force (for grounded kick)
    public float kickRadius; // size of kick hitbox

    // kick charge
    // base = 5 max = 6 extendedMax = 7
    // 1 (max of bar) <= maxKickForce (kickCharge * (max - base)) => e.g. 50/100 * (6-5) = .5 * 1 = .5 => 5.5 force
    // x (max of extended bar) <= extendedMaxKickForce 
    // (ext-base - max-base)/(max-base) * 100 percent increase => (7-5 - 6-5)/(6-5) * 100 = (2 - 1)/(1) * 100 = 1
    // extended bar max value = 1 (max of bar) + (extended percent)
    // kick force = baseKickForce * (kickCharge (clamped at 1 or ext max) * forceIncrease)
    public float maxChargeTime; // time it takes to reach regular max charge
    public float kickCharge; // current charge of kick (0 if non-charged) (added force to kick is base + (kickCharge/maxKickCharge * baseKickForce) (private)
    public float kickChargeRate; // rate at which charge increases per physics frame
    [Range(0, 1)] public float movementChargeRateMultiplier; // how much charge rate is affected by player current speed
    [Range(0, 1)] public float chargeUpForceMultiplier; // how much charge affects upforce amount (grounded attack)
    public LayerMask enemyLayer;
    public int kickDamage = 1;
    public int baseKickDamage = 1;
    public float kickChargeMaxDamage; // damage of non-extended max charge 
    public GameObject playerChargeMeter;
    public GameObject playerExtendedChargeMeter; 
    public float extendedChargeRadius; // max radius when in grapple state
    public bool charging = false;

    [Header("Punch Vars")]
    public GameObject punchPoint;
    public float punchRadius;
    public float uppercutRadius;
    public int punchDamage;     // punch damage that dynamically increases/decreases based on cards
    public int basePunchDamage; // base punch damage
    public int uppercutDamage;  //same as above but just for upper cut 
    public int baseUppercutDamage;
    public Vector2 uppercutForce;
    public float airStunTime;
    public Vector2 forwardPunchMovement; // how much punches move player forward
    public Vector2 superUppercutForce;
    public int downSlamDamage;
    public Vector2 downSlamForce;

    // fx
    [Header("VFX")]
    [Range(0, 1)] public float hitStopScaling; // how much force affects level of hit stop
    public float screenShakeScaling; // how much force affects level of screen shake
    public float hitStopForceThreshold; // force threshold required before hit stop + screenshake is applied
    public Material dullColor;
}