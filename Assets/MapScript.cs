using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    private GameObject Canvas;
    private GameManager GM;
    private GameObject MapScreen;

    private void Awake()
    {
        AssignReferences();
    }


    private void AssignReferences()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        Canvas = GameObject.FindGameObjectWithTag("Canvas");

        MapScreen = this.gameObject;

        if (MapScreen)
        {
            GM.AssignButton(MapScreen.transform, "Menu", GM.OpenMenu);
            GM.AssignButton(MapScreen.transform, "Tutorial", PlayTutorial);
            GM.AssignButton(MapScreen.transform, "Shop", GM.OpenShop);
            GM.AssignButton(MapScreen.transform, "Level 1", PlayLvl1);
            GM.AssignButton(MapScreen.transform, "Level 2", PlayLvl2);
            GM.AssignButton(MapScreen.transform, "Level 3", PlayLvl3);
            GM.AssignButton(MapScreen.transform, "Final Level", FinalLvl);
        }

    }
        
    public void PlayTutorial()
    {
        SceneManager.LoadScene("TUTORIAL");
    }

    public void PlayLvl1()
    {
        SceneManager.LoadScene("LEVEL_1");
    }

    public void PlayLvl2()
    {
        SceneManager.LoadScene("LEVEL_2");
    }

    public void PlayLvl3()
    {
        SceneManager.LoadScene("LEVEL_3");
    }

    public void FinalLvl()
    {
        SceneManager.LoadScene("FINALE");
    }
}
