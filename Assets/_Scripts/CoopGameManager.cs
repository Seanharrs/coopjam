using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets._2D;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Coop
{
  [Serializable]
  public class PlayerData
  {
    [Space]
    [HideInInspector,
     Tooltip("Reference to currently used GameObject instance.")]
    public CoopUserControl playerCharacter;

    [Space]
    [HideInInspector,
     Tooltip("Controller scriptable objects")]
    public PlayerControlData controlData;
    [HideInInspector,
     Tooltip("Will hold a reference to the current Gun's prefab.")]
    public Gun playerGun;

    public Sprite headSprite;

  }

#if UNITY_EDITOR
  [InitializeOnLoad]
#endif
  public class CoopGameManager : MonoBehaviour
  {

#if UNITY_EDITOR
    static CoopGameManager()
    {
      EditorSceneManager.sceneSaved += CheckScene;
    }

    private static void CheckScene(Scene scene)
    {
      CheckLevel();
    }

#endif

    internal static CoopGameManager m_Instance;
    internal static CoopGameManager instance
    {
      get { return m_Instance; }
    }

    internal static string nextLevelOverride;

    [Header("Options")]
    public bool allowKeyboard = false;

    [Header("Local Asset References:")]
    [SerializeField]
    internal AudioSource ambientAudioSource;
    [SerializeField]
    internal AudioSource musicAudioSource;

    [Header("Project Asset References:")]
    [SerializeField]
    internal List<PlayerControlData> playerControlData;
    [SerializeField]
    internal CoopUserControl characterRigPrefab;
    [SerializeField]
    internal List<Gun> allGuns;
    [SerializeField]
    internal List<Sprite> headSprites;
    [SerializeField]
    internal AudioMixer m_AudioMixer;

    private LevelManager m_LevelManager;
    private int m_MusicIndex = -1;
    private float m_LastMusicPlayTime = -1;

    // [Header("Development/Debugging")]
    [HideInInspector]
    internal List<PlayerData> playerData = new List<PlayerData>();


    void Awake()
    {
      if (CoopGameManager.instance && CoopGameManager.instance != this)
      {
        Destroy(this.gameObject);
        return;
      }
      m_Instance = this;
      DontDestroyOnLoad(this);

      // Ensure existence of AudioSources.
      if (m_AudioMixer != null)
      {
        // First get ambient channel
        if (ambientAudioSource == null)
        {
          try
          {
            ambientAudioSource = instance.gameObject.AddComponent<AudioSource>();
            ambientAudioSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("Ambience")[0];
          }
          catch
          {
            Debug.LogError("Can't find AudioMixerGroup 'Ambience'");
          }
        }
        // Then get music channel
        if (musicAudioSource == null)
        {
          try
          {
            musicAudioSource = instance.gameObject.AddComponent<AudioSource>();
            musicAudioSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("Music")[0];
          }
          catch
          {
            Debug.LogError("Can't find AudioMixerGroup 'Music'");
          }
        }

        m_AudioMixer.FindSnapshot("FadedOut").TransitionTo(0f);

      }
      else
      {
        Debug.LogError("Mixer reference not found.");
      }

      SceneManager.sceneLoaded += SceneLoaded;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
      if(!m_LevelManager) return;

      if (m_LevelManager.musicPlaylist.Count > 0 && musicAudioSource)
      {
        if (!musicAudioSource.isPlaying && (Time.time - m_LastMusicPlayTime) > m_LevelManager.musicWaitTime)
        {
          PlayNextClip();
        }
        else if (musicAudioSource.isPlaying)
        {
          m_LastMusicPlayTime = Time.time;
        }
      }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {

      // TODO: This wont help with the menu scenes unless we implement level manager to work with them as well.
      m_LevelManager = FindObjectOfType<LevelManager>();
      if (m_LevelManager == null) {
        ShowMessage("LevelManager required.", 2f, true);
        return;
      }

      // TODO: Fade in visual

      // Fade in sound
      if (m_LevelManager && m_LevelManager.ambientBackgroundSound)
      {
        ambientAudioSource.clip = m_LevelManager.ambientBackgroundSound;
        ambientAudioSource.loop = true;
        ambientAudioSource.Play();
        // TODO: Should we continue to use snapshots or if we can, should we affect individual channels?
        m_AudioMixer.FindSnapshot("Music0Ambient100").TransitionTo(2f);
      }

      if(m_LevelManager)
      {
        m_MusicIndex = -1;
        PlayNextClip();
      }

      var spawnPoints = FindObjectsOfType<SpawnPoint>();
      if (spawnPoints.Count() < playerData.Count())
      {
        ShowMessage("Please generate spawn points for this level.", 5f, true);
        return;
      }
      for (var i = 0; i < playerData.Count; i++)
      {
        CoopUserControl characterRig = Instantiate(characterRigPrefab, spawnPoints[i].transform.position, Quaternion.identity);
        // TEST: Debug.Log("Spawned character at: " + spawnPoints[i].transform.position + " (" + characterRig.transform.position + ")");
        characterRig.controlData = playerData[i].controlData;
        if (m_LevelManager.m_TurnGunsOff)
        {
          characterRig.CanFire = false;
        }
        else
        {
          characterRig.SetGun(playerData[i].playerGun);
        }
        characterRig.HeadSprite = playerData[i].headSprite;
        playerData[i].playerCharacter = characterRig;
      }
    }

    private void PlayNextClip()
    {
      if (m_LevelManager && m_LevelManager.musicPlaylist.Count > 0)
      {
        m_MusicIndex = (m_MusicIndex + 1) % m_LevelManager.musicPlaylist.Count;
        musicAudioSource.clip = m_LevelManager.musicPlaylist[m_MusicIndex];
        musicAudioSource.loop = false;
        musicAudioSource.Play();
        m_AudioMixer.FindSnapshot("Music100Ambient50").TransitionTo(2f);
      }
    }

    public static void OpenLevel(int levelIndex)
    {
      // TODO: Do we need to collect information from the current scene about players before loading the next level?
      SceneManager.LoadScene(levelIndex);
    }

    public static void OpenLevel(string levelName)
    {
      // TODO: Do we need to collect any additional information from the current scene about players before loading the next level?

      if (!String.IsNullOrEmpty(nextLevelOverride))
      {
        SceneManager.LoadScene(nextLevelOverride);
        nextLevelOverride = null;
      }
      else
      {
        Debug.Log("Loading: " + levelName);
        SceneManager.LoadScene(levelName);
        Debug.Log("Loaded: " + levelName);
      }
    }

    public static void SelectPlayersThenOpen(string levelName)
    {
      nextLevelOverride = levelName;
      SceneManager.LoadScene("Player_Select");
    }

    public static IEnumerator ShowMessage(string message, float displayTime = 5f, bool isFatal = false)
    {
      var lm = FindObjectOfType<LevelManager>();
      if (lm.m_MessageTextbox)
      {
        lm.m_MessageTextbox.text = message;
        yield return new WaitForSeconds(displayTime);
        lm.m_MessageTextbox.text = "";
      }
      else
      {
        Debug.LogError(message);
      }

      if (isFatal)
        Application.Quit();
    }



    internal Gun GetAvailableGun(CoopUserControl controller)
    {
      return GetAvailableGun(playerData.Find(p => p.playerCharacter == controller).playerGun);
    }

    internal Gun GetAvailableGun(Gun currentGun = null)
    {
      var guns = allGuns.FindAll(gun => !playerData.Any(player => player.playerGun == gun) || gun == currentGun);
      // Debug.Log(guns.Count() + " guns found: " + String.Join(", ", guns.Select(g => g.name).ToArray()) );

      if (guns.Count() == 0) return null;
      if (currentGun == null)
        return guns[0];
      else
      {
        var index = guns.IndexOf(currentGun);
        if (index == guns.Count() - 1)
        {
          return guns[0];
        }
        return guns[index + 1];
      }
    }

    internal void SetPlayerGun(CoopUserControl character, Gun gun)
    {
      var c = playerData.Find(p => p.playerCharacter == character);
      if (c != null) c.playerGun = gun;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Coop Jam/Check Level")]
    static void CheckLevel()
    {
      List<string> errors = new List<string>();

      // should have exactly one level object, no more, no less
      var levels = FindObjectsOfType<LevelManager>();
      if (levels.Count() != 1)
      {
        errors.Add("Should have exactly one level object, no more, no less.");
      }
      // Level Manager should have an active checkpoint selected.
      else if (levels[0].ActiveCheckpoint == null)
      {
        errors.Add("Need to set up an initial active checkpoint in the LevelManager.");
      }

      // should not have characters directly, use spawn points instead.
      var characters = FindObjectsOfType<CoopUserControl>();
      if (characters.Count() > 0)
        errors.Add("Should not add characters directly, use spawn points instead.");

      // should have exactly 4 spawn points. no more, no less.
      var spawnPoints = FindObjectsOfType<SpawnPoint>();
      if (spawnPoints.Count() != 4)
        errors.Add("Should have exactly 4 spawn points. no more, no less.");

      // One camera, which should be a MultiplayerFollow as well. 
      var cameras = FindObjectsOfType<Camera>().Where(c => c.tag == "MainCamera").ToList();
      if (cameras.Count() != 1)
        errors.Add("Should have exactly one main camera, no more, no less. (You may have additional cameras that are not set as the main camera.)");
      else if (cameras[0].GetComponent<MultiplayerFollow>() == null)
        errors.Add("Camera should also have a MultiplayerFollow component.");
      //else
      //{
      //  var followCam = cameras[0].GetComponent<MultiplayerFollow>();
      //  // Warning if MultiplayerFollow does not have corner selectors
      //  if(followCam.m_BottomLeftIndicator == null || followCam.m_TopRightIndicator == null)
      //  {
      //    errors.Add("Warning: Missing indicator(s). It is much easier to design a level with these.");
      //  }
      //  // Error if both corner selectors are null and min/max values are all zero
      //  if ( (followCam.m_BottomLeftIndicator == null || followCam.m_TopRightIndicator == null)
      //    && followCam.m_MinCamX == 0 && followCam.m_MaxCamX == 0 && followCam.m_MinCamY == 0 && followCam.m_MaxCamY == 0
      //  )
      //  {
      //    errors.Add("Error: Missing indicator(s) and no min/max values have been provided. Camera will not move.");
      //  }
      //}

      // At least one LevelGoal object.
      var levelGoal = FindObjectsOfType<LevelGoal>();
      if (levelGoal.Count() == 0)
        errors.Add("Each level should have at least one LevelGoal (You may have multiple).");

      // At least one checkpoint
      var checkpoints = FindObjectsOfType<Checkpoint>();
      if (checkpoints.Count() == 0)
        errors.Add("Need at least one checkpoint in your level.");
      else if (checkpoints.Count() < 3)
        errors.Add("Less than 3 checkpoints... Do you need more?");

      // Scene should be in build settings.
      if (SceneManager.GetActiveScene().buildIndex == -1)
      {
        errors.Add("Scene has not been added to build settings.");
      }

      // if(errors.Count() == 0)
      //   EditorUtility.DisplayDialog("Level Check", "No errors encountered." , "OK", "Cancel");
      // else
      if (errors.Count() > 0)
        EditorUtility.DisplayDialog("Level Check",
          "Errors ecountered:\n - " + String.Join("\n - ", errors.ToArray())
          , "OK", "Cancel");

    }
#endif
  }
}