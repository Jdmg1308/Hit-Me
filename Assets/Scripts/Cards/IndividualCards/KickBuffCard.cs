using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Kick Buff Card Data", menuName = "ScriptableObjects/Card/KickBuffCard")]
public class KickBuffCard : Card
{
    public override CardType cardType{get{return CardType.PlayerBuff;}}

    /* 
     * adds effect value to kick damage 
     */
    public override void use(GameManager GM) {
        GM.playerController.p.kickDamage += effectValue;
    }
}