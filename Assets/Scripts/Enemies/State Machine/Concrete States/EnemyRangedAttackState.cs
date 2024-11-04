using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyRangedAttackState : EnemyState
{
    private float _attackTime = 0f; // cd between attacks
    private GameObject _bulletPrefab;
    private bool _canShoot = true;

    public EnemyRangedAttackState(Enemy enemy, TransitionDecisionDelegate transitionDecision, float attackTime, GameObject bulletPrefab) 
        : base(enemy, transitionDecision)
    {
        id = EnemyStateMachine.EnemyStates.RangedAttack;
        _attackTime = attackTime; 
        _bulletPrefab = bulletPrefab;
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        switch (triggerType)
        {
            case AnimationTriggerType.Shoot:
                Shoot();
                break;
            case AnimationTriggerType.EndShoot:
                e.Anim.SetBool("isShooting", false);
                e.Anim.SetBool("isWalking", false);
                break;
        }
    }

    public override void EnterState()
    {
        if (e.canAttack && e.IsGrounded && !e.MidJump)
        {
            float dirToPlayer = e.Player.transform.position.x - e.transform.position.x;
            e.FlipCharacter(dirToPlayer > 0);
            if (_canShoot) e.Anim.SetBool("isShooting", true);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Object.Instantiate(_bulletPrefab, e.transform.position, Quaternion.identity);
        bullet.GetComponent<ProjectileScript>().Shoot(e.Player);
        e.StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_attackTime);
        _canShoot = true;
    }
}
