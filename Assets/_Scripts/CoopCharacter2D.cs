using System;
using UnityEngine;

namespace Coop
{
  public class CoopCharacter2D : MonoBehaviour
  {

    private const float GROUND_RAY_DISTANCE = .4f;

    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .25f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_Jumping = false;
    private bool m_Falling = false;


    /* Slope handling */
    [SerializeField] private float m_MaxSlope = 45f;
    [SerializeField] private float m_NormalGravity = 3f;
    [HideInInspector]
    public float NormalGravity
    {
      get { return m_NormalGravity; }
      set
      {
        m_NormalGravity = value;
        m_Rigidbody2D.gravityScale = value;
      }
    }

    [SerializeField] private float m_SlopeGravity = 6f;
    private CircleCollider2D m_CircleCollider;
    private float m_Slope;              // Calculated slope angle below player.
    private float m_SignedSlope;
    private float m_SignedSlope_front;
    private float m_SignedSlope_back;
    private float m_SignedSlope_mid;
    private Vector3 m_SlopeNormal;
    private Vector3 m_SlopePerpendicular;
    private LayerMask m_SlopeMask;
    private bool m_SlipperySlope = false;
    private GameObject debug_GroundObject; // TODO: Remove.

    private bool IsOnSlope 
    {
      get { return m_Slope > 15 && m_Slope < 90; }
    }


    [SerializeField] private BoxCollider2D floorTestCollider;
    [SerializeField] private PhysicsMaterial2D slipperySlopeMaterial;


    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private GravitySensitive m_GravitySensitive;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_InitialScale;

    private void Awake()
    {
      // Setting up references.
      m_GroundCheck = transform.Find("GroundCheck");
      m_CeilingCheck = transform.Find("CeilingCheck");
      m_Anim = GetComponent<Animator>();
      m_Rigidbody2D = GetComponent<Rigidbody2D>();
      m_CircleCollider = GetComponent<CircleCollider2D>();
      m_GravitySensitive = GetComponent<GravitySensitive>();
      m_Rigidbody2D.centerOfMass = m_CircleCollider.offset - ((Vector2)transform.up * m_CircleCollider.radius);
      m_SlopeMask = ~LayerMask.GetMask("Characters");
      m_InitialScale = transform.localScale;

      m_Rigidbody2D.gravityScale = 0;
    }


    private void FixedUpdate()
    {
      m_Grounded = false;

      // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
      // This can be done using layers instead but Sample Assets will not overwrite your project settings.
      Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].gameObject != gameObject)
          m_Grounded = true;
      }

      // Need to raycast as well to determine slope and angle.
      RaycastHit2D[] hits = new RaycastHit2D[3];
      hits[0] = Physics2D.Raycast((Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) + (Vector2.right * Mathf.Sign(transform.localScale.x) * m_CircleCollider.radius), Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      hits[1] = Physics2D.Raycast((Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) - (Vector2.right * Mathf.Sign(transform.localScale.x) * m_CircleCollider.radius), Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      hits[2] = Physics2D.Raycast((Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius), Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      if (hits[0] && hits[0].collider.sharedMaterial != null && hits[0].collider.sharedMaterial == slipperySlopeMaterial)
        m_SlipperySlope = true;
      else
        m_SlipperySlope = false;

      // TODO: Confirm hits exist and only do below averaging based on raycasts that hit.
      float sumSlopes = 0;
      Vector2 sumNormals = Vector2.zero;
      int hitCount = 0;
      foreach(var hit in hits)
      {
        if(!hit) continue;
        sumSlopes += Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
        sumNormals += hit.normal;
        hitCount++;
      }
      if(hits[0].normal == Vector2.up && hits[1].normal == Vector2.up && Vector2.Angle(hits[1].normal, hits[2].normal) <= -15)
      {
        m_SignedSlope = Mathf.Atan2(hits[0].normal.y, hits[0].normal.x) * Mathf.Rad2Deg - 90;
        m_SlopeNormal = hits[0].normal;
      }
      else
      {
        m_SignedSlope = sumSlopes / hitCount;
        m_SlopeNormal = sumNormals / hitCount;
      }

      m_Slope = Mathf.Abs(m_SignedSlope);

      m_SlopePerpendicular = -Vector2.Perpendicular(m_SlopeNormal);

      if (m_SlipperySlope)
      {
        m_Rigidbody2D.AddForce(Physics2D.gravity * m_SlopeGravity);
      }
      if (m_Grounded && IsOnSlope)
      {
        m_Rigidbody2D.AddForce(m_SlopeNormal * Physics2D.gravity * m_NormalGravity);
      }
      else
      {
        m_Rigidbody2D.AddForce(Physics2D.gravity * m_NormalGravity);
      }

      if (m_Jumping && m_Rigidbody2D.velocity.y < 0)
      {
        m_Jumping = false;
        m_Falling = true;
      }
      else if (m_Falling && m_Grounded)
      {
        m_Falling = false;
      }
      else if(!m_Grounded && m_Rigidbody2D.velocity.y < 0)
      {
        m_Falling = true;
      }



      m_Anim.SetBool("Ground", m_Grounded);

      // Set the vertical animation
      m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
    }


    public void Move(float move, bool crouch, bool jump)
    {
      // If crouching, check to see if the character can stand up
      if (!crouch && m_Anim.GetBool("Crouch"))
      {
        // If the character has a ceiling preventing them from standing up, keep them crouching
        if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
        {
          crouch = true;
        }
      }

      // Set whether or not the character is crouching in the animator
      m_Anim.SetBool("Crouch", crouch);

      //only control the player if grounded or airControl is turned on
      if ((m_Grounded || m_AirControl) && !m_SlipperySlope)
      {
        // Reduce the speed if crouching by the crouchSpeed multiplier
        move = (crouch ? move * m_CrouchSpeed : move);

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        m_Anim.SetFloat("Speed", Mathf.Abs(move));

        // Move the character
        if (m_Slope > 15 && m_Slope < 90 && !(m_Jumping || m_Falling))
        {
          m_Rigidbody2D.velocity = move * m_MaxSpeed * m_SlopePerpendicular;
        }
        else
          m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

      }
      // If sliding
      else if (m_SlipperySlope)
      {
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
      }

      if (move != 0)
      {
        m_FacingRight = move > 0;
        Vector3 theScale = transform.localScale;
        theScale.x = m_FacingRight ? -1 * m_InitialScale.x : 1 * m_InitialScale.x;
        theScale.x *= m_GravitySensitive.state == GravityState.Reversed ? 1 : -1;
        transform.localScale = theScale;
      }

      // If the player should jump...
      if (m_Grounded && jump && m_Anim.GetBool("Ground") && !m_SlipperySlope)
      {
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Anim.SetBool("Ground", false);
        m_Anim.SetTrigger("Jump");
        m_Rigidbody2D.AddForce(new Vector2(0f, Mathf.Sign(m_NormalGravity) * m_JumpForce));
        m_Jumping = true;
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
      string output = "Slope: " + (m_Slope);
      output += "\nSlope Front: " + (m_SignedSlope_front);
      output += "\nSlope Mid: " + (m_SignedSlope_mid);
      if (debug_GroundObject != null)
      {
        output += "\n" + "Ground Object: " + debug_GroundObject.name;
      }

      GUI.TextArea(new Rect(0, 0, 200, 200), output);
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
      m_CircleCollider = GetComponent<CircleCollider2D>();
      m_SlopeMask = ~LayerMask.GetMask("Characters");
      var start = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius);
      var hit = Physics2D.Raycast(start, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);

      if (hit)
        debug_GroundObject = hit.collider.gameObject;

      DrawGroundHitGizmos(start, hit);

      start = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) + (Vector2.right * Mathf.Sign(transform.localScale.x) * m_CircleCollider.radius);
      hit = Physics2D.Raycast(start, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);

      DrawGroundHitGizmos(start, hit);
    }

    private void DrawGroundHitGizmos(Vector2 start, RaycastHit2D hit)
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawLine(start, start + Vector2.down * GROUND_RAY_DISTANCE);
      if (IsOnSlope)
        Gizmos.DrawSphere(hit.point, .25f);

      Gizmos.color = Color.yellow;
      Gizmos.DrawLine(hit.point, hit.point + hit.normal);
      Gizmos.DrawSphere(hit.point, .15f);

      var perpStart = hit.point + hit.normal;
      var perpendicular = Vector2.Perpendicular(hit.normal);
      Gizmos.color = Color.red;
      Gizmos.DrawLine(perpStart, perpStart + perpendicular);
      Gizmos.DrawSphere(perpStart, .15f);
    }
#endif
  }
}
