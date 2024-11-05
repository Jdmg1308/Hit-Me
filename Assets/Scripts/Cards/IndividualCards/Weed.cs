using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weed Card Data", menuName = "ScriptableObjects/Card/Weed")]
public class Weed : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}

    /* 
     * time to chillll oouuuuttt
     * effect value should be between 1 and 100, being the time scale % 
     * good starting place is probably 50 or 75
     */
    public override void use(GameManager GM) {

        if (!GM.statusApplied) 
        {
            Time.timeScale = (float)effectValue / 100; // Set the time scale (e.g., 0.5 for half speed)
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust physics updates for consistency
            var postProcessingController = GM.Camera.GetComponent<PostProcessingController>();
            if (postProcessingController != null)
            {
                postProcessingController.ToggleGreenEffect(true);
            }
            else
            {
                Debug.LogError("PostProcessingController not found on the Camera GameObject.");
            }
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
            var postProcessingController = GM.Camera.GetComponent<PostProcessingController>();
            if (postProcessingController != null)
            {
                postProcessingController.ToggleGreenEffect(false);
            }
            else
            {
                Debug.LogError("PostProcessingController not found on the Camera GameObject.");
            }
            //Debug.Log("new HP: " + GM.healthCurrent);
        }
    }

}