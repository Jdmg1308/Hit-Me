using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionTrigger : MonoBehaviour
{
    private Enemy parent;

    void Awake()
    {
        parent = gameObject.GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // if you are not in impact and an enemy that is in impact collides with you, take damage
        if (!parent.InImpact && collider.gameObject.CompareTag("enemy") && collider.gameObject.GetComponent<Enemy>().InImpact)
        {
            Vector2 impactVelocity = collider.GetComponent<Rigidbody2D>().velocity;
            int collisionDamage = Mathf.RoundToInt(impactVelocity.magnitude * parent.collisionDamageMultiplier); // note: consider log max for extreme cases

            Vector2 bounceDirection = parent.enemyCollisionForce;
            bounceDirection.x = bounceDirection.x * (impactVelocity.x > 0 ? 1 : -1); 
            Vector2 force = bounceDirection * (impactVelocity.magnitude * parent.collisionForceMultiplier);

            parent.InImpact = true;
            parent.TakeUppercut(collisionDamage, force);
        }

        if (collider.gameObject.CompareTag("Explosion"))
        {
            Debug.Log("hit by explosion");
            Vector2 impactVelocity = collider.GetComponentInParent<Rigidbody2D>().velocity;
            int collisionDamage = Mathf.RoundToInt(impactVelocity.magnitude * parent.collisionDamageMultiplier); // note: consider log max for extreme cases

            Vector2 bounceDirection = parent.enemyCollisionForce;
            float xDirection = parent.transform.position.x - collider.transform.position.x;
            bounceDirection.x = bounceDirection.x * (xDirection > 0 ? 1 : -1); 
            Vector2 force = bounceDirection * (impactVelocity.magnitude * parent.collisionForceMultiplier);

            parent.InImpact = true;
            parent.TakeUppercut(collisionDamage, force);
        }
    }
}
