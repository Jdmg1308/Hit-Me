using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Fragile Card Data", menuName = "ScriptableObjects/Card/PlayerFragileCard")]
public class PlayerFragile : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}

    /* 
     * makes the player take double damage for ~15 seconds i think
     * double being effect value
     */
    public override void use(GameManager GM) {
        if (!GM.statusApplied)
        {
            //apply fragility
            GM.playerController.p.vulnerability = effectValue;
        }
        else 
        {
            //unapply fragility lol
            GM.playerController.p.vulnerability = 1;
        }
    }
}