using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coop
{
  public class PauseMenu : MonoBehaviour {

    public void LoadMainMenu()
    {
      CoopGameManager.OpenLevel(0); // First level should be main menu.
      Time.timeScale = 1;
    }

    public void ResetLevel()
    {
      int scene = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
      Time.timeScale = 1;
    }

    public void QuitGame()
    {
      Application.Quit();
    }

  }
}