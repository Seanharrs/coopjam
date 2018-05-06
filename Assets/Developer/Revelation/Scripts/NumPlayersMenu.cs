using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coop
{
  public class NumPlayersMenu : MonoBehaviour
  {

    public bool allowKeyboard = false;
    public Dropdown playerCountDropdown;
    public PlayerSelectMenu playerSelectMenu;

    // Use this for initialization
    void Start()
    {
      var maxPlayers = Input.GetJoystickNames().Length;
      if(allowKeyboard) maxPlayers++;
      
      if (maxPlayers < 2)
      {
        Debug.LogError("Minimum of 2 players to enjoy this game. Please plug in at least one controller.");
        // TODO: Present above as dialog.and hide player selection menus.
      }
      else
      {
        Debug.Log("Refilling options.");
        playerCountDropdown.ClearOptions();
        for (var i = 2; i <= maxPlayers; i++)
        {
          playerCountDropdown.options.Add(new Dropdown.OptionData(i + " Players"));
        }
        playerCountDropdown.value = -1;
      }
    }

    public void ContinueButton_Clicked()
    {
      var maxPlayers = 1 + Input.GetJoystickNames().Length;
      if (maxPlayers >= 2)
      {
        playerSelectMenu.NumPlayers = playerCountDropdown.value + 2; // first index is 2, next is 3 and so on.
        playerSelectMenu.gameObject.SetActive(true);
        gameObject.SetActive(false);
      }
    }

  }
}