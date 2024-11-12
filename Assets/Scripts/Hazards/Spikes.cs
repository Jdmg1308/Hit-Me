using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardType
{
    Spikes,
    MovingPlatform,
    BreakableSurface,
    ExplodingBarrel,
    ElectricityField,
    DeathSurface
}

public class Spikes : Hazards, IDamageable
{

    public float spikeKnockbackMultiplier = 20;
    public Vector2 teleportPosition; // define the position player will teleport in

    // Start is called before the first frame update
    void Start()
    {
        // Initialize teleport position if needed, or set it in Inspector
        teleportPosition = new Vector2(0, 0);
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Code to handle spike behavior (e.g., damage to player or enemy)
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController p = other.gameObject.GetComponent<PlayerController>();
            Rigidbody2D RB = other.gameObject.GetComponent<Rigidbody2D>();
            RB.velocity = Vector2.zero;

            // player.TakeDamage(10, PlayerHitKnockBackVectorNormalized() * spikeKnockbackMultiplier);
            player.TakeDamage(0, PlayerHitKnockBackVectorNormalized() * spikeKnockbackMultiplier);

            // teleport player to specific position
            player.transform.position = teleportPosition;

            //Vector2 force = KnockbackForce(other.transform, spikeKnockbackMultiplier);
            // int collisionDamage = Mathf.RoundToInt(force.magnitude * 100f); // note: consider log max for extreme cases

            // p.TakeDamage(collisionDamage, force);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IDamageable enemyScript = other.gameObject.GetComponent<IDamageable>();
            enemyScript.Die();
        }
    }



    #region Not Implemented
    [HideInInspector] public int MaxHealth { get; set; }
    [HideInInspector] public int CurrentHealth { get; set; }
    [HideInInspector] public Animator Anim { get; set; }
    [HideInInspector] public object DeathLock { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    [HideInInspector] public bool InHitStun { get; set; }
    [HideInInspector] public float HitStunTime { get; set; }
    [HideInInspector] public bool inDownSlam { get; set; }
    [HideInInspector] public bool inAirStun { get; set; }

    public void Damage(int damage, float hitStunTime)
    {
        return;
    }

    public void TakeKick(int damage, Vector2 force)
    {
        return;
    }

    public void TakePunch(int damage, float airStunTime)
    {
        return;
    }

    public void Die()
    {
        return;
    }

    public void StopAttack()
    {
        return;
    }

    public void TakeUppercut(int damage, Vector2 force)
    {
        return;
    }
    #endregion 

}
