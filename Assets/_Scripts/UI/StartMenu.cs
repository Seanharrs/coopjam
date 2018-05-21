using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coop
{
  public class StartMenu : MonoBehaviour
  {
    
    public string nextScene;

    public void PlayersSelected()
    {
      var playerSelectMenu = FindObjectOfType<PlayerSelectMenu>();
      
      CoopGameManager gameManager = CoopGameManager.instance;
      gameManager.playerData = playerSelectMenu.GeneratePlayerData();
      CoopGameManager.OpenLevel(nextScene);
    }

  }
}