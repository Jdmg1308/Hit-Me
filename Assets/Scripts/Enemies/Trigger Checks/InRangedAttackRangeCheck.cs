using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRangedAttackRangeCheck : MonoBehaviour
{
    public GameObject Player { get; set; }
    private HasRangedStates _enemy;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<RangedEnemy>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
            _enemy.SetInRangedAttackRange(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
            _enemy.SetInRangedAttackRange(false);
    }
}
