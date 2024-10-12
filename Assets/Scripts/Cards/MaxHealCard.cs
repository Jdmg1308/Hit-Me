using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Max Heal Card Data", menuName = "ScriptableObjects/Card/MaxHealCard")]
public class MaxHealCard : Card
{
    public override CardType cardType{get{return CardType.Heal;}}

    /* 
     * increases the players max HP by effectValue & heals them to full
     */
    public override void use(GameManager GM) {
        GM.healthMax += effectValue;
        GM.healthCurrent = GM.healthMax;
    }
}