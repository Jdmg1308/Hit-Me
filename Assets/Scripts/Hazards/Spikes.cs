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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Code to handle spike behavior (e.g., damage to player or enemy)
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            Rigidbody2D RB = other.gameObject.GetComponent<Rigidbody2D>();
            RB.velocity = Vector2.zero;

            player.TakeDamage(10, PlayerHitKnockBackVectorNormalized() * spikeKnockbackMultiplier);
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
