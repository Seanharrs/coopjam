using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coop
{
  public class PlayerSelectControl : MonoBehaviour
  {

    public int playerIndex = -1;
    public PlayerData playerData;

    public bool isReady = false;

    public Button leftButton;
    public Button rightButton;
    public Image portraitImage;
    public Dropdown controllerDropdown;
    public Button readyButton;

    private float lastSwapped = -5;
    public float minSwapIntervalSeconds = .5f;

    internal void SwapPortrait(bool usePreviousInsteadOfNext = false)
    {
      if(Time.time - lastSwapped < minSwapIntervalSeconds) 
        return;
      lastSwapped = Time.time;

      var imageControl = transform.Find("PortraitImage").GetComponent<Image>();
      imageControl.sprite = GetComponentInParent<PlayerSelectMenu>().GetAvailableSprite(imageControl.sprite, usePreviousInsteadOfNext);
    }

    internal void SetInteractable(bool newEnabled, bool? overrideReadyButton = null)
    {
      leftButton.GetComponent<Button>().interactable = newEnabled;
      rightButton.GetComponent<Button>().interactable = newEnabled;
      // portraitImage.enabled = newEnabled;
      controllerDropdown.GetComponent<Dropdown>().interactable = newEnabled;
      if(overrideReadyButton != null) 
        readyButton.GetComponent<Button>().interactable = (bool)overrideReadyButton;
      else
        readyButton.GetComponent<Button>().interactable = newEnabled;
    }

    internal void SetReady(bool newReady) {
      if(isReady == newReady) return;

      isReady = newReady;
      SetInteractable(!newReady, true); // ready button should remain interactable.

      readyButton.GetComponentInChildren<Text>().text = isReady ? "Cancel" : "Play";
    }

    internal void ToggleReady() {
      SetReady(!isReady);
    }
  }
}