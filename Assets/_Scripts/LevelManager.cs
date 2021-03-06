﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coop {
  public class LevelManager : MonoBehaviour {

    [SerializeField]
    [Tooltip("Which scene should be loaded when level is complete?")]
    internal string m_NextLevel;
    
    [SerializeField]
    [Tooltip("Turn guns off for this level? (Intended for early tutorial level)")]
    internal bool m_TurnGunsOff = false;


    [SerializeField]
    internal Image m_MessagePanel;
    [SerializeField]
    internal TextMeshProUGUI m_MessageTextbox;

    [SerializeField]
    private Checkpoint m_ActiveCheckpoint;
    internal Checkpoint ActiveCheckpoint
    {
      get { return m_ActiveCheckpoint; }
      set {
        if(m_ActiveCheckpoint)
          m_ActiveCheckpoint.SetActive(false);
        m_ActiveCheckpoint = value;
        m_ActiveCheckpoint.SetActive(true);
      }
    }

    [SerializeField]
    internal UnityEvent m_LevelCompleted;

    [SerializeField]
    internal AudioClip ambientBackgroundSound;
    [SerializeField]
    internal List<AudioClip> musicPlaylist;
    [SerializeField]
    [Tooltip("Time in seconds between the end of a playlist item and the beginning of the next. (Default 10 seconds.)")]
    internal float musicWaitTime = 10f;

    internal MultiplayerFollow m_Cam;
    private PauseMenu m_PauseMenu;
    internal PauseMenu PauseMenu
    {
      get {
        if (!m_PauseMenu)
        {
          var existingMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
          if(existingMenus.Length == 0)
            SceneManager.LoadScene("Pause_Menu", LoadSceneMode.Additive);
          existingMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
          if (existingMenus.Length == 0) return null;
          m_PauseMenu = existingMenus[0];
          m_PauseMenu.gameObject.SetActive(false);
        }
        return m_PauseMenu;
      }
    }

    private static LevelManager m_Instance;

    void Awake()
    {
      var gameManager = FindObjectOfType<CoopGameManager>();
      if(gameManager == null)
      {
        CoopGameManager.OpenLevel(0);
        CoopGameManager.nextLevelOverride = SceneManager.GetActiveScene().name;
      } else {
        SceneManager.sceneLoaded += SceneLoaded;
      }

      if(m_Instance) {
        Destroy(gameObject);
        return;
      }
      else
        m_Instance = this;

      if(m_ActiveCheckpoint)
        m_ActiveCheckpoint.SetActive(true);

      if (!m_PauseMenu)
      {
        var existingMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
        if (existingMenus.Length == 0)
          SceneManager.LoadScene("Pause_Menu", LoadSceneMode.Additive);
        existingMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
        if (existingMenus.Length != 0)
        {
          m_PauseMenu = existingMenus[0];
          m_PauseMenu.gameObject.SetActive(false);
        }
      }

    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      m_Cam = FindObjectOfType<MultiplayerFollow>();
      if(!m_Cam) CoopGameManager.ShowMessage("Please add a follow camera to the scene.", 5f, true);        
      else m_Cam.AcquirePlayerRefs();
    }

    public void LevelComplete()
    {
      m_LevelCompleted.Invoke(); // Inform subscribers that level is complete.

      CoopGameManager.OpenLevel(m_NextLevel); // TODO: Show UI  and let player click continue before loading next level?
    }

    internal static Vector3 GetRespawnLocation()
    {
      return m_Instance.m_ActiveCheckpoint.spawnAtPoint.position;
    }

    internal void ShowPauseMenu()
    {
      PauseMenu.gameObject.SetActive(true);
    }

    internal void HidePauseMenu()
    {
      PauseMenu.gameObject.SetActive(false);
    }
  }
}