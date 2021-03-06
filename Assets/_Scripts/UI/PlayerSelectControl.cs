﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Coop
{
  public class PlayerSelectControl : MonoBehaviour
  {

    private bool isInteractable = false;

    public int playerIndex = -1;
    public PlayerData playerData;

    public bool isReady = false;

    public string Label
    {
      get { return transform.Find("LabelText").GetComponent<Text>().text; }
      set { transform.Find("LabelText").GetComponent<Text>().text = value; }
    }

    public Button leftButton;
    public Button rightButton;
    public Image portraitImage;
    public Text portraitLabel;
    public Dropdown controllerDropdown;
    public Button readyButton;

    private float lastSwapped = -5;
    public float minSwapIntervalSeconds = .5f;

    internal bool SwapPortrait(bool usePreviousInsteadOfNext = false, bool overrideTimeControl = false)
    {
      if(isReady) return false;

      var imageControl = portraitImage; // Why did this happen? -> transform.Find("PortraitImage").GetComponent<Image>();

      if (!overrideTimeControl && Time.time - lastSwapped < minSwapIntervalSeconds)
        return false;

      lastSwapped = Time.time;

      if (!isInteractable)
      {
        imageControl.sprite = GetComponentInParent<PlayerSelectMenu>().placeholderPortrait;
        portraitLabel.text = "";
      }
      else
      {
        var menu = GetComponentInParent<PlayerSelectMenu>();
        imageControl.sprite = menu.GetAvailableSprite(imageControl.sprite, usePreviousInsteadOfNext);
        portraitLabel.text = menu.GetSpriteText(imageControl.sprite);
      }
      return true;
    }

    internal void SetInteractable(bool newEnabled, bool? overrideReadyButton = null)
    {
      isInteractable = newEnabled;

      leftButton.GetComponent<Button>().interactable = newEnabled;
      rightButton.GetComponent<Button>().interactable = newEnabled;
      // portraitImage.enabled = newEnabled;
      if (overrideReadyButton != null)
        readyButton.GetComponent<Button>().interactable = (bool)overrideReadyButton;
      else
        readyButton.GetComponent<Button>().interactable = newEnabled;

    }

    internal void SetReady(bool newReady)
    {
      if (isReady == newReady) return;

      SetInteractable(!newReady, true); // ready button should remain interactable.
      isReady = newReady;

      var t = readyButton.GetComponentInChildren<Text>();
      var tm = readyButton.GetComponentInChildren<TextMeshProUGUI>();
      if(t)
        t.text = isReady ? "Waiting..." : "Play";
      if(tm)
        tm.text = isReady ? "Waiting..." : "Play";
    }

    internal void ToggleReady()
    {
      SetReady(!isReady);
    }
  }
}