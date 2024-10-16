using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Heal Card Data", menuName = "ScriptableObjects/Card/EnemyHeal")]
public class EnemyHeal : Card
{
    public override CardType cardType{get{return CardType.EnemyBuff;}}

    /* 
     * heals the enemies for effectValue amount of HP 
     */
    public override void use(GameManager GM) {
        GM.GameEnemyManager.HealEnemies(effectValue);
    }
}