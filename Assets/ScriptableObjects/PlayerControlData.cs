using UnityEngine;
using System.Collections;

namespace Coop
{
  /// <summary>
  /// Describes input mappings belonging to a single player.
  /// </summary>
  [CreateAssetMenu(fileName = "PlayerControlData", menuName = "Player/Controls")]
  public class PlayerControlData : ScriptableObject
  {

    public string controllerName = "";

    [Header("Gameplay Axes")]
    public string horizontalAxis = "Horizontal_P1";
    public string aimHorizontal = "AimHoriz_P1";
    public string aimVertical = "AimVert_P1";

    [Header("Gameplay Buttons")]
    public string jump = "Jump_P1";
    public string crouchButton = "Crouch_P1";
    public string primaryFire = "FirePrimary_P1";
    public string secondaryFire = "FireSecondary_P1";
    public string aimActivate = "AimActivate_P1";
    public string switchPlayerWeapon = "Switch_P1";
    public string interact = "Interact_P1";

    [Header("Menu Buttons")]
    public string openMenuPause = "MenuPause_P1";
    public string submitButton = "Submit_P1";
    public string cancelButton = "Cancel_P1";
  }
}