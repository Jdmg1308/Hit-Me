using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class DummyEnemy : BasicEnemy
{
    private Vector3 initPos;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        ChaseSpeed = 0;
        canAttack = false;
        MaxHealth = 1000;
        CurrentHealth = 1000;
        initPos = transform.position;
    }

    private Coroutine resetRoutine;
    public float resetTime = 2f;
    public override void Damage(int damage, float hitStunTime)
    {
        base.Damage(damage, hitStunTime);

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);
        resetRoutine = StartCoroutine(ResetPosition());
    }

    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(resetTime);
        transform.position = initPos;
    }
}