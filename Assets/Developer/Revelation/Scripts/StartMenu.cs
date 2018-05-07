using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coop
{
  public class StartMenu : MonoBehaviour
  {
    CoopGameManager gameManager;
    public Object nextScene;

    public void PlayersSelected()
    {
      gameManager = FindObjectOfType<CoopGameManager>();
      var playerSelectMenu = FindObjectOfType<PlayerSelectMenu>();
      gameManager.playerData = playerSelectMenu.GeneratePlayerData();
      gameManager.OpenLevel(nextScene.name);
    }

  }
}