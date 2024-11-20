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
            Debug.Log("applying status effect");
            //ex HP at 50/100 and decrease HP by 50 - don't drop the HP to 0, drop it to 1/2 of new amound
            ////oldHPRatio = ((float)GM.healthCurrent / GM.healthMax);
            //GM.healthMax -= effectValue;
            //GM.healthCurrent -= effectValue;

            // Halve max health
            GM.healthMax = Mathf.Max(1, Mathf.FloorToInt(GM.healthMax / 2f));
            // Halve current health, ensuring it doesn't drop below 1
            GM.healthCurrent = Mathf.Max(1, Mathf.FloorToInt(GM.healthCurrent / 2f));

            Debug.Log("new HP: " + GM.healthCurrent);
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            //GM.healthMax += effectValue;
            //GM.healthCurrent += effectValue;

            // Double max health back to the original value
            GM.healthMax *= 2;
            // Snap healthMax to 100 if close
            const int tolerance = 3; // Example: snap if within 3 units
            if (Mathf.Abs(GM.healthMax - 100) <= tolerance)
                GM.healthMax = 100;

            // Double current health proportionally but cap it to max health
            GM.healthCurrent = Mathf.Min(GM.healthMax, GM.healthCurrent * 2);

            Debug.Log("new HP: " + GM.healthCurrent);
        }
    }
}