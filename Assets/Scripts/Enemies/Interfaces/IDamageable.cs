using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }
    Animator Anim { get; set; }
    object DeathLock { get; set; }
    bool IsDead { get; set; }
    bool InHitStun { get; set; }
    float HitStunTime { get; set; }
    bool inDownSlam { get; set; }
    public bool inAirStun { get; set; }

    void Damage(int damage, float hitStunTime);
    void Die();
    void TakeKick(int damage, Vector2 force);
    void TakePunch(int damage, float airStunTime);
    void TakeUppercut(int damage, Vector2 force);
    void StopAttack();
}
