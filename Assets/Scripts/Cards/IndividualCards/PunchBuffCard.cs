using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Punch Buff Card Data", menuName = "ScriptableObjects/Card/PunchBuffCard")]
public class PunchBuffCard : Card
{
    public override CardType cardType{get{return CardType.PlayerBuff;}}

    /* 
     * adds effect value to punch & uppercut damage 
     */
    public override void use(GameManager GM) {
        GM.playerController.p.punchDamage += effectValue;
        GM.playerController.p.uppercutDamage += effectValue;
    }
}