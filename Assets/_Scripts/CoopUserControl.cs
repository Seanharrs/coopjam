using System;
using System.Collections;
using UnityEngine;
// using UnityStandardAssets.CrossPlatformInput;

namespace Coop
{
  [Flags, Serializable]
  public enum PlayerInputMode
  {
    Game = 1,
    UI = 2
  }
  [RequireComponent(typeof(CoopCharacter2D), typeof(Health))]
  public class CoopUserControl : MonoBehaviour
  {
    private CoopCharacter2D m_Character;
    private bool m_Jump;
    private bool m_FiringPrimary = false;
    private bool m_FiringSecondary = false;
    private bool m_CanFire = true;
    private bool m_HideGun = false;
    private PlayerInputMode m_InputMode = PlayerInputMode.Game;

    [Header("Controls")]
    public PlayerControlData controlData;
    private bool isAiming = false;

    [Header("Character Customization")]
    [SerializeField] private Sprite m_HeadSprite;
    public Sprite HeadSprite
    {
      get { return m_HeadSprite; }
      set {
        if(value)
        {
          headSocket.GetComponentInChildren<SpriteRenderer>().sprite = value;
          m_HeadSprite = value;
        }
        else
          Debug.LogWarning("Did not set null head sprite.");
      }
    }

    [Header("Weapon")]
    public Gun gun;
    public GameObject gunSocket;
    public GameObject armSocket;
    public GameObject headSocket;
    public SpriteRenderer crosshair;

    [Header("Other")]
    [Tooltip("How long until first respawn attempt.")]
    [SerializeField] private float m_RespawnTimer = 2f;
    [Tooltip("How long between repeat respawn attempts.")]
    [SerializeField] private float m_RespawnRetryTimer = 1f;

    private MultiplayerFollow m_Cam;
    private Vector3 m_Bounds;
    
    private Health m_HP;
    private Animator m_Anim;
    private BoxCollider2D m_Coll;
    private LevelManager m_LevelControl;

    private void Awake()
    {
      m_LevelControl = FindObjectOfType<LevelManager>();

      m_Character = GetComponent<CoopCharacter2D>();
      m_HP = GetComponent<Health>();
      m_Anim = GetComponent<Animator>();
      m_Coll = GetComponent<BoxCollider2D>();

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;

      m_Cam = FindObjectOfType<MultiplayerFollow>();
      m_Bounds = GetComponent<SpriteRenderer>().sprite.bounds.extents;

      // Trigger logic in public property.
      if(m_HeadSprite != null) HeadSprite = m_HeadSprite;

      crosshair.transform.SetParent(null);
    }


    private void Update()
    {
      if(m_HP.isDead)
        return;
      
      if (!m_Jump)
      {
        // Read the jump input in Update so button presses aren't missed.
        m_Jump = Input.GetButtonDown(controlData.jump);
      }

      //TODO: Package this and move it to an appropriate script/method
      #region Manage Game Input
      if (m_InputMode == PlayerInputMode.Game)
      {
        if(m_CanFire)
        {
          var crossPos = crosshair.transform.position;
          if(isAiming)
          {
            // Bi-directional ('Crosshair') aiming.
            crossPos += new Vector3(Input.GetAxis(controlData.aimHorizontal), Input.GetAxis(controlData.aimVertical), 0);
            crosshair.transform.position = crossPos;

            // Rotate arm to point gun at target.
            LookAtRotate(crossPos, armSocket);
            LookAtRotate(crossPos, headSocket, -40, 40);

          }
          else {
            // Up/Down only
            float verticalAim = Input.GetAxis(controlData.aimVertical);
            if (Mathf.Abs(verticalAim) > 0)
            {
              UnidirectionalRotate(verticalAim, armSocket);
            }
          }

          // manage game controls
          #region Axis Firing Input
          float fireAxis = Input.GetAxis(controlData.primaryFire);
          if (fireAxis != 0 && m_CanFire)
          {
            FiringState weap = fireAxis > 0 ? FiringState.Primary : FiringState.Secondary;
            if(weap == FiringState.Primary)
            {
              m_FiringPrimary = true;
            }
            else
            {
              m_FiringSecondary = true;
            }
            gun.Fire(weap, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
          }
          #endregion
          #region Keyboard Firing Input
          else if (Input.GetButton(controlData.primaryFire) && m_CanFire)
          {
            m_FiringPrimary = true;
            if(isAiming)
              gun.FireAtTarget(FiringState.Primary, crossPos);
            else
              gun.Fire(FiringState.Primary, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
          }
          else if (Input.GetButton(controlData.secondaryFire) && m_CanFire)
          {
            m_FiringSecondary = true;
            if(isAiming)
              gun.FireAtTarget(FiringState.Secondary, crossPos);
            else
              gun.Fire(FiringState.Secondary, gunSocket.transform.right * Mathf.Sign(transform.localScale.x));
          }
          #endregion
          else if (m_FiringPrimary || m_FiringSecondary) // These may be "true" from the previous frame while buttons are not still held this frame.
          {
            gun.StopFiring();
            m_FiringPrimary = false;
            m_FiringSecondary = false;
          }
          else if (Input.GetButtonDown(controlData.aimActivate) && m_CanFire)
          {
            if(isAiming) {
              // TODO: Use reticle/crosshairs for cursor
              Cursor.lockState = CursorLockMode.Locked;
              //Cursor.visible = false;
              crosshair.gameObject.SetActive(false);
              headSocket.transform.rotation = Quaternion.Euler(headSocket.transform.rotation.eulerAngles.x, headSocket.transform.rotation.eulerAngles.y, 0);
              isAiming = false;
            } else {
              Cursor.lockState = CursorLockMode.Locked;
              //Cursor.visible = true;
              crosshair.gameObject.SetActive(true);
              crosshair.transform.position = gun.AmmoSpawnLocation.position;
              // Debug.Log("Crosshair should show now.");
              isAiming = true;
            }
          }
          else if (Input.GetButtonDown(controlData.switchPlayerWeapon) && m_CanFire)
          {
            // Debug.Log("Pressed: switchPlayerWeapon " + Input.GetAxis(controlData.switchPlayerWeapon));
            
            SetGun(CoopGameManager.instance.GetAvailableGun(this));
            
          }
        }
        
        if (Input.GetButtonDown(controlData.interact))
        {
          // Debug.Log("Pressed: interact");

          Collider2D[] res = new Collider2D[1];
          ContactFilter2D filter = new ContactFilter2D()
          {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Interactable")
          };
          m_Coll.OverlapCollider(filter, res);
          if(res[0])
          {
            Interactable obj = res[0].GetComponent<Interactable>();
            if(obj != null) obj.Interact();
          }
        }
        else if (Input.GetButtonDown(controlData.openMenuPause))
        {
          // Debug.Log("Pressed: Pause");
          Time.timeScale = 0;
          // TODO: Show pause menu
          SetInputMode(Coop.PlayerInputMode.UI);
        }
      }
      #endregion

      //TODO: Package this and move it to an appropriate script/method??? 
      //      ... or is this section even necessary? Feels more like the menu should have the code.
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

    private void LookAtRotate(Vector3 lookAtPos, GameObject rotateObject, int? minZRotation = null, int? maxZRotation = null)
    {
      var direction = (lookAtPos - rotateObject.transform.position).normalized;
      var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      var newAngle = targetAngle;

      if(Mathf.Sign(transform.lossyScale.x) > 0) // Facing forward / right
      {
        if(minZRotation != null)
          newAngle = Mathf.Clamp(newAngle, (int)minZRotation, newAngle);
        if(maxZRotation != null)
          newAngle = Mathf.Clamp(newAngle, newAngle, (int)maxZRotation);
      } 
      else 
      {
        if(minZRotation != null && maxZRotation != null)
        {
          if(newAngle >= -90 && newAngle <= 0)
            newAngle = -(int)minZRotation - 180;
          if(newAngle > 0 && newAngle <= 90)
            newAngle = 180 - (int)maxZRotation;
          if(newAngle > 90 && newAngle < 180)
            newAngle = Mathf.Clamp(newAngle, 180 - (int)maxZRotation, 180);
          if(newAngle > -180 && newAngle < -90)
            newAngle = Mathf.Clamp(newAngle, -180, - (int)minZRotation - 180);
        }
        newAngle += 180;
      }
      //newAngle +=  (Mathf.Sign(transform.lossyScale.x) < 0 ? 180 : 0);
      

      // Simply rotating an additional 180 degrees if the player is facing backward works and makes sense but feels like a hack. What's the better way to do this?
      rotateObject.transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    private static void UnidirectionalRotate(float verticalAim, GameObject rotateObject)
    {
      rotateObject.transform.Rotate(0, 0, verticalAim);
      var zRotation = rotateObject.transform.rotation.eulerAngles.z;
      if (zRotation > 90 && zRotation < 270)
      {
        if (zRotation < 180) zRotation = 90; else zRotation = 270;
        rotateObject.transform.rotation = Quaternion.Euler(0, 0, zRotation);
      }
    }

    /// <summary>
    /// Replace the current gun with a new one.
    /// </summary>
    /// <param name="playerGun">A reference to a prefab to instantiate a player gun from.</param>
    internal void SetGun(Gun playerGun)
    {
      if(this.gun != null) Destroy(this.gun.gameObject);
      if(playerGun == null) // null gun will let us do a tutorial level without guns, just switches, doors, etc.
      {
        m_HideGun = true;
        this.gun = null;
        // TODO: make CoopGameManager.instance.SetPlayerGun(this, playerGun); work
        return; // Temp
      }
      else
      {
        this.gun = Instantiate(playerGun, gunSocket.transform.position, gunSocket.transform.rotation, gunSocket.transform);
      }
      // TODO: This seems so wrong. Gotta clean up the pipeline somehow.
      CoopGameManager.instance.SetPlayerGun(this, playerGun);
    }

    private void FixedUpdate()
    {
      if(m_HP.isDead)
        return;
      
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
      if(m_HP.isDead)
        return;
      
      transform.position = m_Cam.ConstrainToView(transform.position, m_Bounds);
      crosshair.transform.position = m_Cam.ConstrainToView(crosshair.transform.position, crosshair.GetComponent<SpriteRenderer>().sprite.bounds.extents, true);
    }
    
    public void Die()
    {
      m_Anim.SetTrigger("Die");      
      m_Coll.enabled = false;
      armSocket.SetActive(false);
      headSocket.SetActive(false);
    }
    
    /// <summary>Called by an animation event at the end of the player's death animation.</summary>
    public IEnumerator Respawn()
    {
      //keep player dead for 2 seconds
      yield return new WaitForSeconds(m_RespawnTimer);
      while(m_HP.isDead)
      {
        RespawnPlayer();
        yield return new WaitForSeconds(m_RespawnRetryTimer);
      }
    }

    private void RespawnPlayer()
    {
      if(!m_LevelControl.ActiveCheckpoint.IsVisible)
      {
        var cam = FindObjectOfType<MultiplayerFollow>();
        var camPos = cam.transform.position;
        // GetComponent<SpriteRenderer>().enabled = false;
        // transform.position = new Vector3(camPos.x, camPos.y, 0);
        return;
      } 
      //GetComponent<SpriteRenderer>().enabled = true;

      m_Anim.SetTrigger("Respawn");
      m_Coll.enabled = true;
      armSocket.SetActive(true);
      headSocket.SetActive(true);

      m_HP.enabled = true;
      m_HP.ResetHealth();

      // TODO: proper respawning
      transform.position = LevelManager.GetRespawnLocation();
      StartCoroutine(RespawnFlash());
    }

    private System.Collections.IEnumerator RespawnFlash()
    {
      float numFlashes = 4f;
      SpriteRenderer sprite = GetComponent<SpriteRenderer>();
      SpriteRenderer armSprite = armSocket.GetComponentInChildren<SpriteRenderer>();
      SpriteRenderer headSprite = headSocket.GetComponentInChildren<SpriteRenderer>();
      Color initColor = sprite.color;
      
      float r = 150f / 255f, g = 150f / 255f, b = 150f / 255f;
      Color flashColor = new Color(r, g, b);

      for(int i = 0; i < numFlashes * 2; ++i)
      {
        Color color = sprite.color == initColor ? flashColor : initColor;
        sprite.color = color;
        armSprite.color = color;
        headSprite.color = color;

        yield return new WaitForSeconds(0.5f);
      }

      sprite.color = initColor;
      armSprite.color = initColor;
      headSprite.color = initColor;
      yield return null;
    }

    public void SetInputMode(PlayerInputMode mode)
    {
      m_InputMode = mode;
    }

    public PlayerInputMode GetInputMode()
    {
      return m_InputMode;
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
      if(!isAiming) return;

      var direction = (crosshair.transform.position - headSocket.transform.position).normalized;
      var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      string output = "Current: " + (headSocket.transform.rotation.eulerAngles.z).ToString() + ", Target: " + angle;

      GUI.TextArea(new Rect(headSocket.transform.position, new Vector2(100, 100)) , output);
    }

    void OnDrawGizmos() {
      if(!isAiming) return;

      Gizmos.color = Color.yellow;
      
      Gizmos.DrawLine(gunSocket.transform.position, crosshair.transform.position);

    }

  }
}
