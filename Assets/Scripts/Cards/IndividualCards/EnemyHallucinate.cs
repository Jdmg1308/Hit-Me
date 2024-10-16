using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Hallucination Card Data", menuName = "ScriptableObjects/Card/EnemyHallucinate")]
public class EnemyHallucinate : Card
{
    public override CardType cardType{get{return CardType.PlayerDebuff;}}

    /* 
     * double the number of enemies (YOU'RE TRIPPING SO HARD)
     * they start with effectValue HP, should really be 1 though
     */
    public override void use(GameManager GM) {
        GM.GameEnemyManager.SpawnHallucinationClones(effectValue);
    }
}