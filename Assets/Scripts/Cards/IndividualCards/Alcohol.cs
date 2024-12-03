using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Alcohol Card Data", menuName = "ScriptableObjects/Card/Alcohol")]
public class Alcohol : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}

    public Material effectMaterial;
    /* 
     * somebody had too much to drink,,,,
     * drunk camera wiggling effect & player does less damage 
     */
    public override void use(GameManager GM) {
        if (!GM.statusApplied) 
        {
            Debug.Log("applying status effect");
            //do less damage, ur so drunk
            GM.playerController.p.kickDamage -= effectValue;
            GM.playerController.p.punchDamage -= effectValue;
            GM.playerController.p.uppercutDamage -= effectValue;
            FullscreenEffectRendererFeature.instance?.SetEffectMaterial(effectMaterial);
        } 
        else 
        {
            Debug.Log("removing status effect");
            GM.playerController.p.kickDamage += effectValue;
            GM.playerController.p.punchDamage += effectValue;
            GM.playerController.p.uppercutDamage += effectValue;
            FullscreenEffectRendererFeature.instance?.SetEffectMaterial(null);
        }
    }
}