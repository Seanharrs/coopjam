using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets._2D;

namespace Coop
{
  [Serializable]
  public class PlayerData
  {
    [Space]
    [HideInInspector,
     Tooltip("Reference to currently used GameObject instance.")]
    public PlatformerCharacter2D playerCharacter;
    
    [Space]
    [HideInInspector,
     Tooltip("Controller scriptable objects")]
    public PlayerControlData controlData;
    [HideInInspector,
     Tooltip("Will hold a reference to the current Gun's prefab.")] 
    public Gun playerGun; 

  }

  public class CoopGameManager : MonoBehaviour
  {

    public static CoopGameManager instance;

    [Header("Options")]
    public bool allowKeyboard = false;

    [Header("Asset References:")]
    public List<PlayerControlData> playerControlData;
    public Platformer2DUserControl characterRigPrefab;

    [Header("Development/Debugging")]
    [HideInInspector]
    public List<PlayerData> playerData = new List<PlayerData>();

    void Awake()
    {
      if(CoopGameManager.instance && CoopGameManager.instance != this) {
        Destroy(this.gameObject);
        return;
      }
      instance = this;
      DontDestroyOnLoad(this);

      SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
      var spawnPoints = FindObjectsOfType<SpawnPoint>();
      if(spawnPoints.Count() == 0) return;
      for (var i = 0; i < playerData.Count; i++)
      {
        Platformer2DUserControl characterRig = Instantiate(characterRigPrefab, spawnPoints[i].transform.position, Quaternion.identity);
        characterRig.controlData = playerData[i].controlData;
        characterRig.SetGun(playerData[i].playerGun);
      }
    }

    public static void OpenLevel(string levelName)
    {
      // TODO: Do we need to collect information from the current scene about players before loading the next level?
      SceneManager.LoadScene(levelName);
    }
  }
}