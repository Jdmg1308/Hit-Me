using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionTrigger : MonoBehaviour
{
    private Enemy parent;
    public float ExplosionMultiplier = 1f;
    //[Header("Layers Settings:")]
    //[SerializeField] private LayerMask EnemyCollisonLayers;

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
            Vector2 impactVelocity;
            // barrel explosion
            if (collider.gameObject.layer == LayerMask.NameToLayer("Explosion"))
            {
                // Determine the direction for the force
                int dir = (parent.transform.position.x - collider.transform.position.x) > 0 ? 1 : -1;

                // Calculate the distance factor to scale the force; add an offset to avoid extreme values for very close distances
                float distance = Vector2.Distance(parent.transform.position, collider.transform.position);
                float distanceFactor = Mathf.Clamp(1 / (distance + 0.5f), 0.1f, 1.5f); // Limit force scaling for very close/very far

                // Create an initial force vector with horizontal direction
                Vector2 force = new Vector2(dir * distanceFactor * 30f, 10f); // Increase multiplier if you want a larger effect

                int collisionDamage = Mathf.RoundToInt(distanceFactor * 10f); // note: consider log max for extreme cases

                // Apply the force to launch the enemy
                parent.InImpact = true;
                parent.TakeUppercut(collisionDamage, force);
            }
            else
            {   // down slam explosion
                impactVelocity = collider.GetComponentInParent<Rigidbody2D>().velocity;

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
}
