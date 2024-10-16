using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Despawn Card Data", menuName = "ScriptableObjects/Card/EnemyDespawn")]
public class EnemyDespawn : Card
{
    public override CardType cardType{get{return CardType.EnemyDebuff;}}

    /* 
     * instakill effectValue # of enemies (if they exist!)
     * if effectValue is -1, that means despawn half of currently alive enemies!
     */
    public override void use(GameManager GM) {
        if (effectValue == -1) 
        {
            GM.GameEnemyManager.DestroyExtraEnemies(GM.GameEnemyManager.EnemiesLeftInWave / 2);
        }
        else 
        {
            GM.GameEnemyManager.DestroyExtraEnemies(effectValue);
        }
    }
}