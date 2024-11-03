using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Caffiene Card Data", menuName = "ScriptableObjects/Card/Caffiene")]
public class Caffiene : Card
{
    public override CardType cardType{get{return CardType.StatusEffect;}}
    public AudioSource audioSource; // Assign this in the Inspector or get it via code.
    //public float oldSpeed;

    /* 
     * player had a few too many cups of coffee 
     * increases movement speed & speeds up music by effect value 
     */
    public override void use(GameManager GM) {
        if (audioSource == null)
        {
            audioSource = GM.GetComponent<AudioSource>();
        }

        if (!GM.statusApplied) 
        {
            //apply status effect
            Debug.Log("applying status effect");
            GM.playerController.p.moveSpeed += effectValue * 10;
            GM.playerController.p.XMaxSpeed += effectValue * 10;
            GM.playerController.p.YMaxSpeed += effectValue * 10;
            Debug.Log("Current movement speed: " + GM.playerController.p.moveSpeed);
            audioSource.pitch += (float)effectValue / 2; // increase to speed up, decrease to slow down.
            //Debug.Log("new HP: " + GM.healthCurrent);
        } 
        else 
        {
            Debug.Log("un-applying status affect");
            //unapply the status efect
            GM.playerController.p.moveSpeed -= effectValue * 10;
            GM.playerController.p.XMaxSpeed -= effectValue * 10;
            GM.playerController.p.YMaxSpeed -= effectValue * 10;
            audioSource.pitch = 1;
            //Debug.Log("new HP: " + GM.healthCurrent);
        }
    }

}