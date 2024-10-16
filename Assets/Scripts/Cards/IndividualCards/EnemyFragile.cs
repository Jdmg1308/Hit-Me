using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Fragile Card Data", menuName = "ScriptableObjects/Card/EnemyFragileCard")]
public class EnemyFragile : Card
{
    public override CardType cardType{get{return CardType.EnemyDebuff;}}

    /* 
     * makes the ENEMIES take double damage for effectValue # of seconds
     */
    public override void use(GameManager GM) {
        GM.GameEnemyManager.DoubleDamageTimer(effectValue);
    }
}