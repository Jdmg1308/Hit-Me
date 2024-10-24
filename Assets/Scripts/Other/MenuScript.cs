using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class MenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject controls_img;
    public GameObject PlayButton;
    public GameObject QuitButton;
    private bool showingControls = false;

    public void Play()
    {
        SceneManager.LoadScene("MAP");
    }

    public void Controls()
    {
        // disable other stuff and enable controls with exit button
        if (showingControls)
        {
            showingControls = false;
            controls_img.SetActive(showingControls);

        } else
        {
            showingControls = true;
            controls_img.SetActive(showingControls);
        }

    }

    public void exitControls()
    {
        // enable other stuff and disable controls with exit button
        controls_img.SetActive(false);
        PlayButton.SetActive(true);
        QuitButton.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
