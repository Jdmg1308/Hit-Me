using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    private Enemy _enemy;
    private SpriteRenderer sprite;

    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Punch()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.StartPunch);
    }

    public void EndPunch()
    {
        sprite.color = Color.white;
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.EndPunch);
    }

    public void EndShouldBeDamaging()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.EndPunchDamaging);
    }

    public void Shoot()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.Shoot);
    }

    public void EndShoot()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.EndShoot);
    }
}
