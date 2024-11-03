using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Steriods Card Data", menuName = "ScriptableObjects/Card/Steriods")]
public class Steroids : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}
    //public float oldSpeed;

    /* 
     * it's bulking season  
     * effect value should be "10" or so, which would decrease damage taken by 10%
     * and I'm just gonna make it heal the player foor the same amount
     */
    public override void use(GameManager GM) {

        if (!GM.statusApplied) 
        {
            //apply status effect
            Debug.Log("applying status effect");
            GM.playerController.p.vulnerability -= effectValue / 100;
            GM.healthCurrent += effectValue;
            //Debug.Log("new HP: " + GM.healthCurrent);
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            GM.playerController.p.vulnerability += effectValue / 100;
            //Debug.Log("new HP: " + GM.healthCurrent);
        }
    }

}