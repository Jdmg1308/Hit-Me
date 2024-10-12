using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fixed Heal Card Data", menuName = "ScriptableObjects/Card/FixedHealCard")]
public class FixedHealCard : Card
{
    public override CardType cardType{get{return CardType.Heal;}}

    /* 
     * heals the player for a fixed amount (effectValue)
     */
    public override void use(GameManager GM) {
        GM.healthCurrent += effectValue;

    }
}