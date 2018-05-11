using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    [SerializeField]
    internal List<Gun> allGuns;

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
        // TEST: Debug.Log("Spawned character at: " + spawnPoints[i].transform.position + " (" + characterRig.transform.position + ")");
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


    [MenuItem("Coop Jam Tools/Check Level")]
    static void CheckLevel()
    {
      List<string> errors = new List<string>();

      var levels = FindObjectsOfType<LevelManager>();
      // should have exactly one level object, no more, no less
      if(levels.Count() != 1)
        errors.Add("Should have exactly one level object, no more, no less.");
      var characters = FindObjectsOfType<Platformer2DUserControl>();
      // should not have characters directly, use spawn points instead.
      if(characters.Count() > 0)
        errors.Add("Should not add characters directly, use spawn points instead.");
      var spawnPoints = FindObjectsOfType<SpawnPoint>();
      // should have exactly 4 spawn points. no more, no less.
      if(spawnPoints.Count() != 4)
        errors.Add("Should have exactly 4 spawn points. no more, no less.");
      var cameras = FindObjectsOfType<Camera>().Where(c => c.tag == "MainCamera").ToList();
      if(cameras.Count() != 1)
        errors.Add("Should have exactly one main camera, no more, no less. (You may have additional cameras that are not set as the main camera.)");
      // One camera, which should be a MultiplayerFollow as well. 
      else if(cameras[0].GetComponent<MultiplayerFollow>() == null)
        errors.Add("Camera should also have a MultiplayerFollow component.");
      else
      {
        var followCam = cameras[0].GetComponent<MultiplayerFollow>();
        // Warning if MultiplayerFollow does not have corner selectors
        if(followCam.m_BottomLeftIndicator == null || followCam.m_TopRightIndicator == null)
        {
          errors.Add("Warning: Missing indicator(s). It is much easier to design a level with these.");
        }
        // Error if both corner selectors are null and min/max values are all zero
        if ( (followCam.m_BottomLeftIndicator == null || followCam.m_TopRightIndicator == null)
          && followCam.m_MinCamX == 0 && followCam.m_MaxCamX == 0 && followCam.m_MinCamY == 0 && followCam.m_MaxCamY == 0
        )
        {
          errors.Add("Error: Missing indicator(s) and no min/max values have been provided. Camera will not move.");
        }
      }
      if (SceneManager.GetActiveScene().buildIndex == -1)
      {
        errors.Add("Scene has not been added to build settings.");
      }

      if(errors.Count() == 0)
        EditorUtility.DisplayDialog("Level Check", "No errors encountered." , "OK", "Cancel");
      else
        EditorUtility.DisplayDialog("Level Check", 
          "Errors ecountered:\n - " + String.Join("\n - ", errors.ToArray()) 
          , "OK", "Cancel");

    }

  }
}