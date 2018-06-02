using System;
using UnityEngine;

namespace Coop
{
  public class CoopCharacter2D : MonoBehaviour
  {

    private const float GROUND_RAY_DISTANCE = 2f;

    
    private Vector2 bottomFront;
    private Vector2 bottomMiddle;
    private Vector2 bottomBack;

    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .25f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded = false;            // Whether or not the player is grounded.
    private Rigidbody2D m_GroundRigidBody;
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
    [SerializeField, Tooltip("Which layers can be tested for ground.")]
    private LayerMask m_SlopeMask;
    private bool m_SlipperySlope = false;
    private GameObject m_GroundObject; // TODO: Remove.
    private BezierWalk platformParent;

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
      m_Rigidbody2D.centerOfMass = m_CircleCollider.offset + ((Vector2)transform.forward * m_CircleCollider.radius); // - ((Vector2)transform.up * m_CircleCollider.radius);
      m_InitialScale = transform.localScale;

      m_Rigidbody2D.gravityScale = 0;
    }


    private void FixedUpdate()
    {
      m_Grounded = false;

      // The player is grounded if a circle cast to the ground check position hits anything designated as ground
      // This can be done using layers instead but Sample Assets will not overwrite your project settings.
      Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].gameObject != gameObject)
          m_Grounded = true;
      }

      bottomFront = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) + (Vector2.right * Mathf.Sign(transform.lossyScale.x) * m_CircleCollider.radius);
      bottomBack = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) - (Vector2.right * Mathf.Sign(transform.lossyScale.x) * m_CircleCollider.radius);
      bottomMiddle = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius);

      // Need to ray cast as well to determine slope and angle.
      RaycastHit2D[] hits = new RaycastHit2D[3];
      hits[0] = Physics2D.Raycast(bottomFront, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      hits[1] = Physics2D.Raycast(bottomBack, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      hits[2] = Physics2D.Raycast(bottomMiddle, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);
      if (hits[0] && !hits[0].collider.isTrigger && hits[0].collider.sharedMaterial != null && hits[0].collider.sharedMaterial == slipperySlopeMaterial)
        m_SlipperySlope = true;
      else
        m_SlipperySlope = false;

      //var diff = Vector2.SignedAngle(hits[0].normal, hits[1].normal);
      //var diff2 = Vector2.SignedAngle(hits[1].normal, hits[0].normal);
      
      //if (Math.Abs(diff) > 0.1f)
      //  Debug.Log(diff + ":::" + diff2);

      //// TODO: Confirm hits exist and only do below averaging based on ray casts that hit.
      //float sumSlopes = 0;
      //Vector2 sumNormals = Vector2.zero;

      //int hitCount = 0;
      //foreach(var hit in hits)
      //{
      //  if(!hit || hit.collider.isTrigger) continue;
      //  m_GroundObject = (hit.collider.gameObject.layer & m_SlopeMask) == hit.collider.gameObject.layer ? hit.collider.gameObject : null;
      //  var rb = hit.collider.GetComponent<Rigidbody2D>();
      //  if (rb && !m_GroundRigidBody) m_GroundRigidBody = rb;
      //  if (!float.IsNaN(hit.normal.x) && !float.IsNaN(hit.normal.y))
      //  {
      //    sumSlopes += Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
      //    sumNormals += hit.normal;
      //    hitCount++;
      //  }
      //}
      //if (diff <= -15)
      //{
      //  m_SignedSlope = Mathf.Atan2(hits[0].normal.y, hits[0].normal.x) * Mathf.Rad2Deg - 90;
      //  m_SlopeNormal = hits[0].normal;
      //}
      //else if (diff >= 15)
      //{
      //  Debug.Log("Going down???");
      //  m_SignedSlope = Mathf.Atan2(hits[0].normal.y, hits[0].normal.x) * Mathf.Rad2Deg - 90;
      //  m_SlopeNormal = (hits[0].normal + hits[1].normal) / 2;
      //}
      //else
      //{
      //  m_SignedSlope = sumSlopes / hitCount;
      //  m_SlopeNormal = sumNormals / hitCount;
      //}
      m_SlopeNormal = hits[0].normal;
      m_SignedSlope = Vector2.SignedAngle(Vector2.up, m_SlopeNormal) * Mathf.Sign(transform.lossyScale.x);
      if ((m_SignedSlope < 1f && m_SignedSlope > -1f) || m_SignedSlope >= 89f)
        m_SlopeNormal = hits[0] ? hits[0].normal : hits[1] ? hits[1].normal : hits[2] ? hits[2].normal : Vector2.up;
      else
        m_SlopeNormal = Mathf.Sign(m_SignedSlope) > 0 ? hits[0].normal : hits[1].normal;

      m_Slope = Mathf.Abs(m_SignedSlope);

      m_SlopePerpendicular = -Vector2.Perpendicular(m_SlopeNormal);

      if (m_SlipperySlope)
      {
        m_Rigidbody2D.AddForce(Physics2D.gravity * m_SlopeGravity);
      }
      if (m_Grounded && IsOnSlope)
      {
        m_Rigidbody2D.AddForce(m_SlopeNormal * Physics2D.gravity * m_NormalGravity * 0.5f);
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

        if(move != 0)
        {
          // Move the character
          if (!(m_Jumping || m_Falling) && m_SlopePerpendicular != null && !float.IsNaN(m_SlopePerpendicular.x)) // (m_Slope > 0 && m_Slope < 90 && !(m_Jumping || m_Falling))
          {
            var newVelocity = move * m_MaxSpeed * m_SlopePerpendicular;
            if (newVelocity.magnitude > 0.1f)
              m_Rigidbody2D.velocity = newVelocity;
            else
              m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

          }
          else
            m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
        } else
        {
          
        }

        if (m_Grounded && m_GroundObject)
        {
          var ground_rb = m_GroundObject.GetComponent<Rigidbody2D>();
          if(ground_rb)
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, ground_rb.velocity.y);
          
          if(!transform.parent && m_GroundObject.GetComponent<BezierWalk>() != null)
            transform.SetParent(m_GroundObject.transform);
          
        }
        else if (transform.parent) transform.SetParent(null);

      }
      // If sliding
      else if (m_SlipperySlope)
      {
        Debug.Log("Slippery.");
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
        transform.SetParent(null);
        m_Grounded = false;
        m_Anim.SetBool("Ground", false);
        m_Anim.SetTrigger("Jump");
        m_Rigidbody2D.AddForce(new Vector2(0f, Mathf.Sign(m_NormalGravity) * m_JumpForce));
        m_Jumping = true;
      }
    }

    private void OnDestroy()
    {
      Debug.Log("Destroying now.");
    }

#if UNITY_EDITOR
    ///// <summary>
    ///// OnGUI is called for rendering and handling GUI events.
    ///// This function can be called multiple times per frame (one call per event).
    ///// </summary>
    void OnGUI()
    {
      if (FindObjectsOfType<CoopCharacter2D>()[0] != this) return;
      string output = "Slope: " + (m_SignedSlope);
      output += "\nSigned up/right: " + (Vector2.SignedAngle(Vector2.up, (Vector2.up + Vector2.right) / 2));
      output += "\nSigned up/left: " + (Vector2.SignedAngle(Vector2.up, -Vector2.right));
      if (m_GroundObject != null)
      {
        output += "\n" + "Ground Object: " + m_GroundObject.name;
      }

      GUI.TextArea(new Rect(0, 0, 200, 200), output);
    }

    /// <summary>
    /// Callback to draw Gizmos that are Pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
      m_CircleCollider = GetComponent<CircleCollider2D>();
      m_SlopeMask = ~LayerMask.GetMask("Characters");
      var start = bottomMiddle; // (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius);
      var hit = Physics2D.Raycast(start, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);

      DrawGroundHitGizmos(start, hit);

      //if (hit)
      //  m_GroundObject = hit.collider.gameObject;

      if (m_SlopePerpendicular != null)
      {
        if (m_Rigidbody2D)
        {
          Gizmos.color = Color.magenta;
          Gizmos.DrawRay(transform.position, m_Rigidbody2D.velocity.normalized * 4);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position - transform.up, m_SlopePerpendicular.normalized * 4);
      } else
      {
          Gizmos.color = Color.magenta;
          Gizmos.DrawWireCube(transform.position, Vector3.one);
      }


      start = bottomFront; // (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius) + (Vector2.right * Mathf.Sign(transform.localScale.x) * m_CircleCollider.radius);
      hit = Physics2D.Raycast(start, Vector2.down, GROUND_RAY_DISTANCE, m_SlopeMask);

      DrawGroundHitGizmos(start, hit);

      start = bottomBack; 
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
      var perpendicular = Vector2.Perpendicular(hit.normal) * -Mathf.Sign(transform.lossyScale.x);
      Gizmos.color = Color.red;
      Gizmos.DrawLine(perpStart, perpStart + perpendicular);
      Gizmos.DrawSphere(perpStart, .15f);
    }
#endif
  }
}
