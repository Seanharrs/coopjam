using System;
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
    
    public List<Gun> guns;
    public Sprite placeholderPortrait;
    private List<Sprite> availablePortraits;

    private int m_NumPlayers = 2;
    public int NumPlayers
    {
      get { return m_NumPlayers; }
      set
      {
        m_NumPlayers = Mathf.Clamp(value, 2, 4); // Allow 2 to 4 players.
      }
    }

    private CoopGameManager gameManager;
    public Dictionary<PlayerControlData, PlayerSelectControl> playerControlsMap = new Dictionary<PlayerControlData, PlayerSelectControl>();

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

    void Start()
    {
      gameManager = FindObjectOfType<CoopGameManager>();
    }

    void OnEnable()
    {

      availablePortraits = guns.Select(gun => gun.portraitSprite).ToList();

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

    void Update()
    {
      foreach (var controller in gameManager.playerControlData)
      {
        if (!playerControlsMap.ContainsKey(controller))
        {
          if (Input.GetButtonDown(controller.submitButton) || Input.GetButtonDown(controller.openMenuPause))
          {
            Debug.Log("Trying to activate because " + controller.submitButton + " or " + controller.openMenuPause + " was pressed.");
            TryActivateController(controller);
          }
        }
        else
        {
          // TODO: This is causing weird behavior that affects the ready status of the control. 
          //       However, its not integral to the functionality, so come back to it later if there's time.
                // if (Input.GetButtonDown(controller.cancelButton))
                // {
                //   TryDeactivateController(controller);
                // }
          if (Input.GetAxis(controller.horizontalAxis) < 0) // Left
          {
            GetUIControlFor(controller).SwapPortrait(true); // previous
          }
          else if (Input.GetAxis(controller.horizontalAxis) > 0) // Right
          {
            GetUIControlFor(controller).SwapPortrait(false); // next
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
      //Debug.Log(guns.First(g => g.portraitSprite == playerControlsMap.First(x => true).Value.portraitImage.sprite));
      return playerControlsMap
        .Select(
          x => new PlayerData { 
            playerActive = true,
            controlData = x.Key,
            playerGun = guns.First(g => g.portraitSprite == x.Value.portraitImage.sprite)
          }).ToList();
    }

    // Attempts to attach the controller that pressed 'submit' or 'pause' to a player selection control
    internal bool TryActivateController(PlayerControlData controller)
    {
      if (!playerControlsMap.ContainsKey(controller))
      {
        var control = FindAvailableControl();
        if (control != null)
        {
          playerControlsMap.Add(controller, control);
          control.SetInteractable(true);
          control.SwapPortrait();
          control.Label = controller.controllerName;
          // Debug.Log("Activated controller: " + controller.name);
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
        if(playerControlsMap.Remove(controller)) {
          uiControl.SetInteractable(false);
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
      Debug.Log("Left button was clicked for player: " + (uiControl.playerIndex + 1));
      uiControl.SwapPortrait(true); // previous
    }
    void RightButton_Click(PlayerSelectControl uiControl)
    {
      Debug.Log("Right button was clicked for player: " + (uiControl.playerIndex + 1));
      uiControl.SwapPortrait(false); // next
    }
    void ReadyButton_Click(PlayerSelectControl uiControl)
    {
      // Debug.Log("Ready button was clicked for player: " + (uiControl.playerIndex + 1));

      // enable/disable controls for clicking
      uiControl.ToggleReady();

      if (uiControl.isReady)
      {
        var readyCount = 0;
        availablePortraits.Remove(uiControl.portraitImage.sprite);
        foreach (var otherControl in playerControlsMap)
        {
          // Debug.Log("Foreach is Checking... ");
          var otherUiControl = otherControl.Value;
          
          // as a secondary effect, this loop will count how many are ready in total so we can start the game when everybody is ready to go.
          readyCount = otherUiControl.isReady ? readyCount + 1 : readyCount;
          
          if(otherUiControl == uiControl) continue; // don't do this if its the same control.

          // If currently on the picture that was just used, select an image from what is available.
          if (otherUiControl.portraitImage.sprite == uiControl.portraitImage.sprite)
          {
            otherUiControl.SwapPortrait();
          }
        }

        if(readyCount == playerControlsMap.Count() && playerControlsMap.Count() > 1)
        {
          AllReady();
        }

      }
      else
      {
        availablePortraits.Add(uiControl.portraitImage.sprite);
      }
    }

    internal void AllReady() 
    {
      allPlayersReady.Invoke();
    }

  }

}