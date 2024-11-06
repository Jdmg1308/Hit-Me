using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class SpatialTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;  // Event for when a collider enters this trigger
    private GameManager GM;

    public void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Invoke the assigned methods
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            onTriggerEnter.Invoke();
    }

    public void NextScene(string SceneName)
    {
        if (GM)
        {
            if (ChecklistComplete(SceneName))
                SceneManager.LoadScene(SceneName);
        }
    }

    public bool ChecklistComplete(string SceneName)
    {

        Debug.Log("THIS SHIT GOT CALLED");
        switch (SceneName)
        {
            case "MOVEMENT COMBAT":
                return (true);
            case "Combat_1":
                return (GM.ChecklistScript.taskNameToBool["PC"] && GM.ChecklistScript.taskNameToBool["PAC"] && GM.ChecklistScript.taskNameToBool["TK"] && GM.ChecklistScript.taskNameToBool["FCK"] && GM.ChecklistScript.taskNameToBool["PKC"]);
            case "GRAPPLE":
                return (true);
            case "Combat_2":
                return (GM.ChecklistScript.taskNameToBool["GE"] && GM.ChecklistScript.taskNameToBool["GP"] && GM.ChecklistScript.taskNameToBool["SCK"]);
            case "CARDS":
                return (true);
            case "Combat_3":
                return (GM.ChecklistScript.taskNameToBool["DC"]);
            case "SHOP":
                return (true);

            default:
                return false;
        }
    }
}
