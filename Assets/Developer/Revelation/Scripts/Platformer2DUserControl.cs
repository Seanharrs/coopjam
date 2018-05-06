using System;
using UnityEngine;
using UnityStandardAssets._2D;
// using UnityStandardAssets.CrossPlatformInput;

namespace Coop
{
  // TODO: Flags makes it possible to implement dual input mode (since I've seen it done in Unreal).
  //       If we don't find a use for this, remove Flags attribute.
  [Flags, Serializable]
  public enum PlayerInputMode
  {
    Game = 1,
    UI = 2
  }
  [RequireComponent(typeof(PlatformerCharacter2D))]
  public class Platformer2DUserControl : MonoBehaviour
  {
    private PlatformerCharacter2D m_Character;
    private bool m_Jump;
    private PlayerInputMode m_InputMode = PlayerInputMode.Game;

    [Header("Controls")]
    public PlayerControlData controlData;
    private bool isAiming = false;

    [Header("Weapon")]
    public Gun gun;
    public GameObject gunSocket;

    private void Awake()
    {
      m_Character = GetComponent<PlatformerCharacter2D>();
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }


    private void Update()
    {
      //TESTING 
      //re-enable cursor
      if(Input.GetKeyDown(KeyCode.BackQuote))
      {
        Cursor.lockState ^= CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
      }

      if (!m_Jump)
      {
        // Read the jump input in Update so button presses aren't missed.
        m_Jump = Input.GetButtonDown(controlData.jump);
      }

      //TODO: Package this and move it to an appropriate script/method
      #region Manage Game Input
      if (m_InputMode == PlayerInputMode.Game)
      {
        if(isAiming) {
          // Bi-directional ('Crosshair') aiming.
          var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
          var direction = (mousePos - gunSocket.transform.position).normalized;
          var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
          // TODO: simply rotating an additional 180 degrees if the player is facing backward works and makes sense but feels like a hack. What's the better way to do this?
          gunSocket.transform.rotation = Quaternion.Euler(0, 0, angle + (Mathf.Sign(transform.lossyScale.x) < 0 ? 180 : 0));
        } else {
          // Up/Down only
          float verticalAim = Input.GetAxis(controlData.aimVertical);
          if (Mathf.Abs(verticalAim) > 0) {
            gunSocket.transform.Rotate(0, 0, verticalAim);
            var zRotation = gunSocket.transform.rotation.eulerAngles.z;
            if(zRotation > 90 && zRotation < 270) {
              if(zRotation < 180) zRotation = 90; else zRotation = 270;
              gunSocket.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            }
          }
        }

        // manage game controls
        if (Input.GetButtonDown(controlData.jump))
        {
          Debug.Log("Pressed: jump");
        }
        else if (Input.GetButtonDown(controlData.crouchButton))
        {
          Debug.Log("Pressed: crouchButton");
        }
        else if (Input.GetAxis(controlData.primaryFire) != 0)
        {
          Debug.Log("Pressed: Fire " + Input.GetAxis(controlData.primaryFire));
          gun.Fire(Input.GetAxis(controlData.primaryFire) > 0 ? WhichWeapon.Primary : WhichWeapon.Secondary, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
        }
        else if (Input.GetButton(controlData.primaryFire))
        {
          Debug.Log("Pressed: Primary Fire");
          if(isAiming)
            gun.FireAtTarget(WhichWeapon.Primary, Camera.main.ScreenToWorldPoint(Input.mousePosition));
          else
            gun.Fire(WhichWeapon.Primary, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
        }
        else if (Input.GetButton(controlData.secondaryFire))
        {
          Debug.Log("Pressed: Secondary Fire");
          
          if(isAiming)
            gun.FireAtTarget(WhichWeapon.Secondary, Camera.main.ScreenToWorldPoint(Input.mousePosition));
          else
            gun.Fire(WhichWeapon.Secondary, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
        }
        else if (Input.GetButtonDown(controlData.aimActivate))
        {
          Debug.Log("Pressed: aimActivate");
          if(isAiming) {
            // TODO: Use reticle/crosshairs for cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isAiming = false;
          } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isAiming = true;
          }
        }
        else if (Input.GetButtonDown(controlData.switchPlayerWeapon))
        {
          Debug.Log("Pressed: switchPlayerWeapon " + Input.GetAxis(controlData.switchPlayerWeapon));
        }
        else if (Input.GetButtonDown(controlData.interact))
        {
          Debug.Log("Pressed: interact");
        }
        else if (Input.GetButtonDown(controlData.openMenuPause))
        {
          Debug.Log("Pressed: Pause");
          Time.timeScale = 0;
          // TODO: Show pause menu
          SetInputMode(Coop.PlayerInputMode.UI);
        }
      }
      #endregion

      //TODO: Package this and move it to an appropriate script/method
      #region Manage UI Input
      else if (m_InputMode == PlayerInputMode.UI)
      {
        // manage UI controls
        if (Input.GetButtonDown(controlData.openMenuPause))
        { // && !canGoBack
          // Close menu
          SetInputMode(PlayerInputMode.Game);
          Time.timeScale = 1;
          Debug.Log("Pressed: Unpause");
        }
        else if (Input.GetButtonDown(controlData.submitButton))
        {
          // Activate menu item
          Debug.Log("Pressed: submitButton");
        }
        else if (Input.GetButtonDown(controlData.cancelButton))
        { // && canGoBack
          // Go back
          Debug.Log("Pressed: cancelButton");
        }
      }
      #endregion

    }

    private void FixedUpdate()
    {
      // Read the inputs.
      bool crouch = Input.GetButton(controlData.crouchButton);
      float h = Input.GetAxis(controlData.horizontalAxis);
      // Pass all parameters to the character control script.
      m_Character.Move(h, crouch, m_Jump);
      m_Jump = false;
    }
    
    //Clamp player position to within camera view
    private void LateUpdate()
    {
      Vector3 pos = transform.position;
      Vector3 camPos = Camera.main.transform.position;
      float vertLim = Camera.main.orthographicSize;
      float horizLim = vertLim * Screen.width / Screen.height;

      pos.x = Mathf.Clamp(pos.x, camPos.x - horizLim, camPos.x + horizLim);
      pos.y = Mathf.Clamp(pos.y, camPos.y - vertLim, camPos.y + vertLim);
      
      //TODO take into account bounds of player sprite

      //TODO kill player if they fall off the bottom of the camera?
      //     how do we then handle reverse gravity sending player off the top?

      transform.position = pos;
    }

    public void SetInputMode(PlayerInputMode mode)
    {
      m_InputMode = mode;
    }

    public PlayerInputMode GetInputMode()
    {
      return m_InputMode;
    }

    void OnDrawGizmos() {
      if(!isAiming) return;

      Gizmos.color = Color.yellow;
      
      Gizmos.DrawLine(gunSocket.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

    }

  }
}
