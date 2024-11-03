using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LSD Card Data", menuName = "ScriptableObjects/Card/Hallucinate")]
public class Hallucinate : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}

    /* 
     * double the number of enemies (YOU'RE TRIPPING SO HARD)
     * they start with effectValue HP, should really be 1 though
     * also everything is rainbow :) 
     */
    public override void use(GameManager GM) {
        if (!GM.statusApplied) 
        {
            Debug.Log("applying status effect");
            GM.GameEnemyManager.SpawnHallucinationClones(effectValue);
                    // Ensure you get the PostProcessingController component
            var postProcessingController = GM.Camera.GetComponent<PostProcessingController>();
            if (postProcessingController != null)
            {
                postProcessingController.ToggleRainbowEffect(true);
            }
            else
            {
                Debug.LogError("PostProcessingController not found on the Camera GameObject.");
            }
        } 
        else 
        {
            // Ensure you get the PostProcessingController component
            var postProcessingController = GM.Camera.GetComponent<PostProcessingController>();
            if (postProcessingController != null)
            {
                postProcessingController.ToggleRainbowEffect(false);
            }
            else
            {
                Debug.LogError("PostProcessingController not found on the Camera GameObject.");
            }
        }
    }
}