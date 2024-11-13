using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : Hazards, IDamageable
{
    public float damageMultiplier = 0.21f;
    public float knockBackMultiplier = 300f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerController p = collider.GetComponent<PlayerController>();
            Debug.Log("Player hit by Barrel Explosion");

            Rigidbody2D RB = p.GetComponent<Rigidbody2D>();
            RB.velocity = Vector2.zero;

            Vector2 force = KnockbackForce(collider.transform, knockBackMultiplier);
            int collisionDamage = Mathf.RoundToInt(force.magnitude * damageMultiplier); // note: consider log max for extreme cases

            p.TakeDamage(collisionDamage, force);
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
