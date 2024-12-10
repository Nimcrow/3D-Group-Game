using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // On Start button click transition scene to level 1 scene
    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }

    // On Quit button click quit the application
    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(5);
    }
}
