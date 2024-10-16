using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Spawn Card Data", menuName = "ScriptableObjects/Card/EnemySpawn")]
public class EnemySpawn : Card
{
    public override CardType cardType{get{return CardType.EnemyBuff;}}

    /* 
     * spawn effectValue # of enemies :3 gotta kill them all 
     */
    public override void use(GameManager GM) {
        GM.GameEnemyManager.SpawnExtraEnemies(effectValue);
    }
}