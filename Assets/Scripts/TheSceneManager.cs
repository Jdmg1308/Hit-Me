using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TheSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OpenMap()
    {
        SceneManager.LoadScene("MAP");
    }
    public void OpenMenu()
    {
        SceneManager.LoadScene("MENU");
    }
    public void OpenShop()
    {
        SceneManager.LoadScene("SHOP");
    }
    public void PlayTutorial()
    {
        SceneManager.LoadScene("MOVEMENT_COMBAT");
    }

    public void PlayAlleyway1()
    {
        SceneManager.LoadScene("ALLEYWAY_1");
    }

    public void PlayAlleyway2()
    {
        SceneManager.LoadScene("ALLEYWAY_2");
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
