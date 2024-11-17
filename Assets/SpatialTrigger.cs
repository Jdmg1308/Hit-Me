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
    public string NextSceneName;
    public GameObject tutorialWall;

    public void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    private void Update()
    {
        onTriggerEnter.Invoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Invoke the assigned methods
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (ChecklistComplete(NextSceneName))
                SceneManager.LoadScene(NextSceneName);
        }
    }

    public void NextScene()
    {
        if (GM && tutorialWall)
        {
            if (ChecklistComplete(NextSceneName))
                tutorialWall.SetActive(false);
        }
    }

    public bool ChecklistComplete(string SceneName)
    {
        switch (SceneName)
        {
            case "MOVEMENT_COMBAT":
                return (false);
            case "Combat_GOON":
                return (GM.ChecklistScript.taskNameToBool["PC"] && GM.ChecklistScript.taskNameToBool["PAC"] && GM.ChecklistScript.taskNameToBool["TK"] && GM.ChecklistScript.taskNameToBool["FCK"] && GM.ChecklistScript.taskNameToBool["PKC"]);
            case "GRAPPLE":
                return (false);
            case "Combat_RANGED":
                return (GM.ChecklistScript.taskNameToBool["GE"] && GM.ChecklistScript.taskNameToBool["GP"] && GM.ChecklistScript.taskNameToBool["SCK"]);
            case "CARDS":
                return (false);
            case "Combat_MORERANGED":
                return (GM.ChecklistScript.taskNameToBool["DC"]);
            case "SHOP":
                return (false);
            default:
                return false;
        }
    }
}
