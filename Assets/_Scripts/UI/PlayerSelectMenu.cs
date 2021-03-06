﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Coop
{
  public class PlayerSelectMenu : MonoBehaviour
  {

    public UnityEvent allPlayersReady = new UnityEvent();

    [SerializeField]
    private List<GameObject> playerSelectAnchors;
    private List<PlayerSelectControl> playerSelectControls = new List<PlayerSelectControl>();

    [SerializeField]
    internal Sprite placeholderPortrait;
    internal List<Sprite> availablePortraits;

    private Dictionary<PlayerControlData, PlayerSelectControl> playerControlsMap = new Dictionary<PlayerControlData, PlayerSelectControl>();

    public AudioClip m_SFX_MenuNavigate;
    public AudioClip m_SFX_MenuConfirm;

    internal Sprite GetAvailableSprite(Sprite currentSprite, bool usePreviousInsteadOfNext)
    {
      if (availablePortraits.Count == 0) return null;

      var currentSpriteIndex = availablePortraits.FindIndex(x => x == currentSprite);
      if (currentSpriteIndex == -1)
      {
        return availablePortraits[0]; // if no current selection, use first sprite.
      }
      else
      {
        var returnedIndex = -1;
        if (usePreviousInsteadOfNext)
        {
          returnedIndex = currentSpriteIndex == 0 ? availablePortraits.Count() - 1 : currentSpriteIndex - 1;
        }
        else
        {
          returnedIndex = currentSpriteIndex == availablePortraits.Count() - 1 ? 0 : currentSpriteIndex + 1;
        }
        return availablePortraits[returnedIndex];
      }
    }

    public void PlayNavigateSound()
    {
      CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_SFX_MenuNavigate);
    }
    public void PlayConfirmSound()
    {
      CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_SFX_MenuConfirm);
    }

    internal string GetSpriteText(Sprite sprite)
    {
      return CoopGameManager.instance.allGuns.Find(g => g.portraitSprite == sprite).GunName;
    }

    void OnEnable()
    {

      availablePortraits = CoopGameManager.instance.allGuns.Select(gun => gun.portraitSprite).ToList();

      foreach (var anchor in playerSelectAnchors)
      {
        var selectControl = anchor.GetComponentInChildren<PlayerSelectControl>();
        playerSelectControls.Add(selectControl);
        selectControl.playerIndex = playerSelectAnchors.IndexOf(anchor);
        selectControl.leftButton.onClick.AddListener(delegate { LeftButton_Click(selectControl); });
        selectControl.rightButton.onClick.AddListener(delegate { RightButton_Click(selectControl); });
        selectControl.readyButton.onClick.AddListener(delegate { ReadyButton_Click(selectControl); });
      }
    }

    void Start()
    {
        var snap = CoopGameManager.instance.m_AudioMixer.FindSnapshot("MainMenu");
        snap.TransitionTo(1.5f); 
    }

    void Update()
    {
      foreach (var controller in CoopGameManager.instance.playerControlData)
      {
        if (!playerControlsMap.ContainsKey(controller))
        {
          if (Input.GetButtonDown(controller.submitButton) || Input.GetButtonDown(controller.openMenuPause))
          {
            // "Trying to activate because " + controller.submitButton + " or " + controller.openMenuPause + " was pressed."
            TryActivateController(controller);
          }
        }
        else
        {
          // TODO: This may still be causing weird behavior. Further testing needed to ensure things work correctly.
          if (Input.GetButtonDown(controller.cancelButton))
          {
            if (playerControlsMap[controller].isReady) //playerControlsMap[controller].SetReady(false);
              ReadyButton_Click(playerControlsMap[controller]);
            else
              TryDeactivateController(controller);
          }
          if (Input.GetAxis(controller.horizontalAxis) < 0) // Left
          {
            if(GetUIControlFor(controller).SwapPortrait(true)) // previous
            {
              PlayNavigateSound();
            }
          }
          else if (Input.GetAxis(controller.horizontalAxis) > 0) // Right
          {
            if(GetUIControlFor(controller).SwapPortrait(false)) // next
            {
              PlayNavigateSound();
            }
          }
          else if (Input.GetButtonDown(controller.submitButton))
          {
            ReadyButton_Click(playerControlsMap[controller]);
          }
        }
      }
    }

    // convenience method
    private PlayerSelectControl GetUIControlFor(PlayerControlData controller)
    {
      return playerControlsMap[controller];
    }

    internal List<PlayerData> GeneratePlayerData()
    {
      var i = 0;
      return playerControlsMap
        .Select(
          x => new PlayerData
          {
            controlData = x.Key,
            playerGun = CoopGameManager.instance.allGuns.First(g => g.portraitSprite == x.Value.portraitImage.sprite),
            headSprite = CoopGameManager.instance.headSprites[i++]
          }).ToList();
    }

    // Attempts to attach the controller that pressed 'submit' or 'pause' to a player selection control
    internal bool TryActivateController(PlayerControlData controller)
    {
      if (!playerControlsMap.ContainsKey(controller))
      {
        var uiControl = FindAvailableControl();
        if (uiControl != null)
        {
          playerControlsMap.Add(controller, uiControl);
          uiControl.SetInteractable(true);
          PlayNavigateSound();
          uiControl.SwapPortrait();
          uiControl.Label = controller.controllerName;
          return true;
        }
        else
          return false;
      }
      return false;
    }

    internal bool TryDeactivateController(PlayerControlData controller)
    {
      if (playerControlsMap.ContainsKey(controller))
      {
        var uiControl = playerControlsMap[controller];
        if (playerControlsMap.Remove(controller))
        {
          var sprite = uiControl.portraitImage.sprite;
          if(sprite != placeholderPortrait && !availablePortraits.Contains(sprite)) 
            availablePortraits.Add(sprite);
          
          uiControl.SetReady(false);
          uiControl.SetInteractable(false);
          uiControl.SwapPortrait();
          uiControl.Label = "...";
          return true;
        }
        else
          return false;
      }
      return false;
    }

    private PlayerSelectControl FindAvailableControl()
    {
      foreach (var control in playerSelectControls)
      {
        if (!playerControlsMap.ContainsValue(control)) return control;
      }
      return null;
    }

    void LeftButton_Click(PlayerSelectControl uiControl)
    {
      
      if(uiControl.SwapPortrait(true)) // previous
      {
        PlayNavigateSound();
      }
    }
    void RightButton_Click(PlayerSelectControl uiControl)
    {
      if(uiControl.SwapPortrait(false)) // next
      {
        PlayNavigateSound();
      }
    }
    void ReadyButton_Click(PlayerSelectControl uiControl)
    {
      // enable/disable controls for clicking
      PlayConfirmSound();
      uiControl.ToggleReady();

      if (uiControl.isReady)
      {
        // Debug.Log("Ready...");
        var readyCount = 0;
        availablePortraits.Remove(uiControl.portraitImage.sprite);
        foreach (var otherControl in playerControlsMap)
        {
          var otherUiControl = otherControl.Value;

          // as a secondary effect, this loop will count how many are ready in total so we can start the game when everybody is ready to go.
          readyCount = otherUiControl.isReady ? readyCount + 1 : readyCount;

          if (otherUiControl == uiControl) continue; // don't do this if its the same control.

          // If currently on the picture that was just used, select an image from what is available.
          if (otherUiControl.portraitImage.sprite == uiControl.portraitImage.sprite)
          {
            otherUiControl.SwapPortrait(false, true);
          }
        }

        if (readyCount == playerControlsMap.Count() && playerControlsMap.Count() > 1)
        {
          AllReady();
          // Debug.Log("Ready: " + readyCount + "/" + playerControlsMap.Count());
        } else {
          // Debug.Log("Not ready: " + readyCount + "/" + playerControlsMap.Count());
        }

      }
      else
      {
        // Debug.Log("Returning portrait to list.");
        availablePortraits.Add(uiControl.portraitImage.sprite);
      }
    }

    internal void AllReady()
    {
      allPlayersReady.Invoke();
    }

  }

}