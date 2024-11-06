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

    public void StartAttack()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.StartAttack);
    }

    public void EndAttack()
    {
        sprite.color = Color.white;
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.EndAttack);
    }

    public void EndAttackDamaging()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.EndAttackDamaging);
    }

    public void ChargeUp()
    {
        _enemy.AnimationTriggerEvent(EnemyState.AnimationTriggerType.ChargeUp);
    }
}
