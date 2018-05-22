using System;
using UnityEngine;

namespace Coop
{
    public class CoopCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .25f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.


        /* Slope handling */
        [SerializeField] private float m_MaxSlope = 45f;
        [SerializeField] private float m_NormalGravity = 3f;
        [HideInInspector] public float NormalGravity
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
        private Vector3 m_SlopeNormal;
        private Vector3 m_SlopePerpendicular;
        private LayerMask m_SlopeMask;
        private bool m_SlipperySlope = false;
        private GameObject debug_GroundObject;
        private bool debug_IsOnSlope = false;
        [SerializeField] private BoxCollider2D floorTestCollider;


        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_CircleCollider = GetComponent<CircleCollider2D>();
            m_Rigidbody2D.centerOfMass = m_CircleCollider.offset - ((Vector2)transform.up * m_CircleCollider.radius);
            m_SlopeMask = ~LayerMask.GetMask("Characters");

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
            var hit = Physics2D.Raycast((Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius), Vector2.down, 2f, m_SlopeMask);
            if(hit && hit.collider.sharedMaterial != null && hit.collider.sharedMaterial.name == "Slippery") 
              m_SlipperySlope = true;
            else
              m_SlipperySlope = false;

            m_SignedSlope = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
            m_Slope = Mathf.Abs(m_SignedSlope);
            debug_IsOnSlope = m_Slope > 15 && m_Slope < 90;
            m_SlopeNormal = hit.normal;
            m_SlopePerpendicular = -Vector2.Perpendicular(hit.normal);

            if(m_Grounded && debug_IsOnSlope)
            {
              //m_Rigidbody2D.gravityScale = 0;
              m_Rigidbody2D.AddForce(m_SlopeNormal * Physics2D.gravity * m_NormalGravity);
            }
            else
            {
              Debug.Log("Not on slope: Angle -> " + m_Slope);
              m_Rigidbody2D.AddForce(Physics2D.gravity * m_NormalGravity);
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

            Collider2D[] results = new Collider2D[10];
            if(floorTestCollider.OverlapCollider(new ContactFilter2D(), results) > 0)
            {
              bool foundSlippery = false;
              foreach(var collider in results)
              {
                if(collider && collider.gameObject == gameObject) continue;
                if(collider && collider.sharedMaterial != null && collider.sharedMaterial.name == "Slippery")
                {
                  foundSlippery = true;
                }
              }
              m_SlipperySlope = foundSlippery && m_Slope > m_MaxSlope;
            }

            //only control the player if grounded or airControl is turned on
            if ( (m_Grounded || m_AirControl) && (!m_SlipperySlope || m_Slope < m_MaxSlope) ) 
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                if(m_Slope > 15 && m_Slope < 90)
                {
                  //var previousVelocity = m_Rigidbody2D.velocity / m_SlopePerpendicular;
                  //m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, previousVelocity.y) * m_SlopePerpendicular;
                  m_Rigidbody2D.velocity = move * m_MaxSpeed * m_SlopePerpendicular;
                }
                else
                  m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            // If sliding
            if (m_SlipperySlope && m_Slope >= m_MaxSlope)
            {
              m_Rigidbody2D.gravityScale = m_SlopeGravity;
            }
            else
            {
              //m_Rigidbody2D.gravityScale = m_NormalGravity;
            }

            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Anim.SetTrigger("Jump");
                m_Rigidbody2D.AddForce(new Vector2(0f, Mathf.Sign(m_NormalGravity) * m_JumpForce));
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        /// <summary>
      /// OnGUI is called for rendering and handling GUI events.
      /// This function can be called multiple times per frame (one call per event).
      /// </summary>
      void OnGUI()
      {
        string output = "Slope: " + (m_Slope);
        if(debug_GroundObject != null)
        {
          output += "\n" + "Ground Object: " + debug_GroundObject.name;
        }

        GUI.TextArea(new Rect(100, 0, 100, 100) , output);
      }

      /// <summary>
      /// Callback to draw gizmos that are pickable and always drawn.
      /// </summary>
      void OnDrawGizmos()
      {
          #if UNITY_EDITOR
            m_CircleCollider = GetComponent<CircleCollider2D>();
            m_SlopeMask = ~LayerMask.GetMask("Characters");
          #endif
          var start = (Vector2)m_CircleCollider.bounds.center + (Vector2.down * m_CircleCollider.radius);
          var hit = Physics2D.Raycast(start, Vector2.down, 2f, m_SlopeMask);
          
          if(hit)
            debug_GroundObject = hit.collider.gameObject;

            

          Gizmos.color = Color.blue;
          Gizmos.DrawLine(start, start + Vector2.down * 2f);
          if(debug_IsOnSlope)
            Gizmos.DrawSphere(hit.point, .5f);

          Gizmos.color = Color.yellow;
          Gizmos.DrawLine(hit.point, hit.point + hit.normal);
          Gizmos.DrawSphere(hit.point, .15f);

          var perpStart = hit.point + hit.normal;
          var perpendicular = Vector2.Perpendicular(hit.normal);
          Gizmos.color = Color.red;
          Gizmos.DrawLine(perpStart, perpStart + perpendicular);
          Gizmos.DrawSphere(perpStart, .15f);
      }
    }
}
