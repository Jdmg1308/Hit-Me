using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Full Heal Card Data", menuName = "ScriptableObjects/Card/EnemyFullHeal")]
public class EnemyFullHeal : Card
{
    public override CardType cardType{get{return CardType.EnemyBuff;}}

    /* 
     * heals the enemies for their MAX HP!!!!
     */
    public override void use(GameManager GM) {
        GM.GameEnemyManager.HealEnemies(effectValue);
    }
}