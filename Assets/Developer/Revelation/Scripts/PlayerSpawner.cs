using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class PlayerSpawner : MonoBehaviour
  {
    private CoopGameManager gameManager;

    void Start()
    {
      gameManager = FindObjectOfType<CoopGameManager>();

      var spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawn");
      for (var i = 0; i < gameManager.playerData.Count; i++)
      {
        Platformer2DUserControl characterRig = Instantiate(gameManager.characterRigPrefab, spawnPoints[i].transform.position, Quaternion.identity);
        characterRig.controlData = gameManager.playerData[i].controlData;
        characterRig.gun = Instantiate(gameManager.playerData[i].playerGun, characterRig.gunSocket.transform.position, Quaternion.identity, characterRig.gunSocket.transform);
      }
    }
  }
}
