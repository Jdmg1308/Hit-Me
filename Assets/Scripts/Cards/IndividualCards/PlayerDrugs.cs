using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Drugs Card Data", menuName = "ScriptableObjects/Card/PlayerDrugsCard")]
public class PlayerDrugs : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}
    public float oldHPRatio = 0.0f;
    // public float oldMaxHP = 0;

    /* 
     * player is on too many drugs to count
     * increased damage, HP, and movement speed
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
            GM.healthMax -= effectValue * 10;
            GM.healthCurrent -= effectValue * 10;
            GM.playerController.p.kickDamage -= effectValue;
            GM.playerController.p.punchDamage -= effectValue;
            GM.playerController.p.uppercutDamage -= effectValue;
            GM.playerController.p.moveSpeed -= effectValue * 10;
            Debug.Log("new HP: " + GM.healthCurrent);
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            GM.healthMax += effectValue * 10;
            GM.healthCurrent += effectValue * 10;
            GM.playerController.p.kickDamage += effectValue;
            GM.playerController.p.punchDamage += effectValue;
            GM.playerController.p.uppercutDamage += effectValue;
            GM.playerController.p.moveSpeed += effectValue * 10;
            Debug.Log("new HP: " + GM.healthCurrent);
        }
    }
}