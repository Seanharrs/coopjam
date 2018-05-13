﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coop {
  public class LevelManager : MonoBehaviour {

    [SerializeField]
    [Tooltip("Which scene should be loaded when level is complete?")]
    internal string nextLevel;

    [SerializeField]
    internal Text messageTextbox;

    [SerializeField]
    internal UnityEvent levelCompleted;

    internal MultiplayerFollow m_Cam;

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
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      m_Cam = FindObjectOfType<MultiplayerFollow>();
      if(!m_Cam) CoopGameManager.ShowMessage("Please add a follow camera to the scene.", 5f, true);        
      m_Cam.AcquirePlayerRefs();
    }

    public void LevelComplete()
    {
      levelCompleted.Invoke(); // Inform subscribers that level is complete.

      CoopGameManager.OpenLevel(nextLevel); // TODO: Show UI  and let player click continue before loading next level?
    }

  }
}