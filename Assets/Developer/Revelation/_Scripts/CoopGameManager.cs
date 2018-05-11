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

    public static string nextLevelOverride;

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
      var lm = FindObjectOfType<LevelManager>();
      if(lm == null) ShowMessage("LevelManager required.", 2f, true);

      var spawnPoints = FindObjectsOfType<SpawnPoint>();
      if(spawnPoints.Count() < playerData.Count()) 
      {
        ShowMessage("Please generate spawn points for this level.", 5f, true);
        return;
      }
      for (var i = 0; i < playerData.Count; i++)
      {
        Platformer2DUserControl characterRig = Instantiate(characterRigPrefab, spawnPoints[i].transform.position, Quaternion.identity);
        Debug.Log("Spawned character at: " + spawnPoints[i].transform.position + " (" + characterRig.transform.position + ")");
        characterRig.controlData = playerData[i].controlData;
        characterRig.SetGun(playerData[i].playerGun);
      }
    }

    public static void OpenLevel(int levelIndex)
    {
      // TODO: Do we need to collect information from the current scene about players before loading the next level?
      SceneManager.LoadScene(levelIndex);
    }

    public static void OpenLevel(string levelName)
    {
      if(!String.IsNullOrEmpty(nextLevelOverride))
      {
        SceneManager.LoadScene(nextLevelOverride);
        nextLevelOverride = null;
      } else {
        // TODO: Do we need to collect information from the current scene about players before loading the next level?
        SceneManager.LoadScene(levelName);
      }
    }

    public static IEnumerator ShowMessage(string message, float displayTime = 5f, bool isFatal = false)
    {
      var lm = FindObjectOfType<LevelManager>();
      if(lm.messageTextbox)
      {
        lm.messageTextbox.text = message;
        yield return new WaitForSeconds(displayTime);
        lm.messageTextbox.text = "";
      }
      else
      {
        Debug.LogError(message);
      }

      if(isFatal)
        Application.Quit();
    }
  }
}