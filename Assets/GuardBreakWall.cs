using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBreakWall : MonoBehaviour, IDamageable
{
    public float minForce; // min force to break wall
    public GameObject strongPowPrefab;

    public void TakeKick(int damage, Vector2 force)
    {
        Debug.Log(force);
        SpawnDamageVFX(force.x);
    }

    // spawning damage vfx
    private void SpawnDamageVFX(float xForce)
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 1.5f;
        Vector3 spawnPosition = new Vector3(randomPoint.x, randomPoint.y, 0) + transform.position;
        GameObject newVFX = Instantiate(strongPowPrefab, spawnPosition, Quaternion.identity);
        // make vfx 'weaker' if kick not strong enough
        if (xForce < minForce)
            newVFX.GetComponent<SpriteRenderer>().color = newVFX.GetComponent<SpriteRenderer>().color * 0.5f;
        else
            Destroy(gameObject);
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
    
    public void Damage(int damage, float hitStunTime)
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

    public void TakePunch(int damage, float airStunTime)
    {
        return;
    }

    public void TakeUppercut(int damage, Vector2 force)
    {
        return;
    }
    #endregion
}
