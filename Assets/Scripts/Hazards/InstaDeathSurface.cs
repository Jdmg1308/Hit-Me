using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaDeathSurface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Code to handle electricity field behavior (e.g., apply knockback or stun)
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(10000, this.transform.position);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IDamageable enemyScript = other.gameObject.GetComponent<IDamageable>();
            enemyScript.Die();
        }
    }
}
