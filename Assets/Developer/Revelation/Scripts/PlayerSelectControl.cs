using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coop
{
  public class PlayerSelectControl : MonoBehaviour
  {

    public int playerIndex = -1;

    public bool isReady = false;

    public Button leftButton;
    public Button rightButton;
    public Image portraitImage;
    public Dropdown controllerDropdown;
    public Button readyButton;

  }
}