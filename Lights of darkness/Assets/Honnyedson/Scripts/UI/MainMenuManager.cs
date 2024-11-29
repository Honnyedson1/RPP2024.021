using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject Options;
    public GameObject PauseImage;
    public void PlayGame()
    {
        SceneManager.LoadScene("Vilarejo");
    }
    public void OptionsGame()
    {
        Options.gameObject.SetActive(true);
        PauseImage.gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Voltar()
    {
        Options.gameObject.SetActive(false);
        PauseImage.gameObject.SetActive(true);
    }
    
}
