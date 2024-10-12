using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scaled Heal Card Data", menuName = "ScriptableObjects/Card/ScaledHealCard")]
public class ScaledHealCard : Card
{
    public override CardType cardType{get{return CardType.Heal;}}
    /* 
     * heals the player for a scaled amount based on missing HP
     * this one is 1/effectValue * missing hp
     */
    public override void use(GameManager GM) {
        float scale = 1 / (float)(effectValue);
        GM.healthCurrent += (int)(scale * (GM.healthMax - GM.healthCurrent));

    }
}