using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : Hazards, IDamageable
{

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

            // Determine the direction for the force
            int dir = (collider.transform.position.x - transform.position.x) > 0 ? 1 : -1;
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            float distanceFactor = Mathf.Clamp(1 / (distance + 0.5f), 0.1f, 10.5f); // Limit force scaling for very close/very far

            Vector2 force = new Vector2(dir * distanceFactor * 300f, 50f); // Increase multiplier if you want a larger effect

            int collisionDamage = Mathf.RoundToInt(distanceFactor * 10f); // note: consider log max for extreme cases
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
