using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Decrease HP Status Effect Card Data", menuName = "ScriptableObjects/Card/DecHPStatusEffectCard")]
public class DecreaseHPEffectCard : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}
    public float oldHPRatio = 0.0f;
    // public float oldMaxHP = 0;

    /* 
     * temporarily halves the player's HP 
     */
    public override void use(GameManager GM) {
        if (!GM.statusApplied) 
        {
            //apply status effect
            // oldHP = GM.healthCurrent;
            // oldMaxHP = GM.healthMax;
            Debug.Log("applying status effect");
            //ex HP at 50/100 and decrease HP by 50 - don't drop the HP to 0, drop it to 1/2 of new amound
            //oldHPRatio = ((float)GM.healthCurrent / GM.healthMax);
            GM.healthMax -= effectValue;
            GM.healthCurrent -= effectValue;
            Debug.Log("new HP: " + GM.healthCurrent);
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            GM.healthMax += effectValue;
            GM.healthCurrent += effectValue;
            Debug.Log("new HP: " + GM.healthCurrent);
        }
    }
}