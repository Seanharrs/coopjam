using System;
using System.Collections;
using System.Collections.Generic;
using Coop;
using UnityEngine;

namespace Coop
{

  [RequireComponent(typeof(Projectile))]
  public class TeleportProjectile : MonoBehaviour
  {

    // References to other components.
    private Projectile m_Projectile;
    private Collider2D m_Collider;
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private Color primaryColor;
    [SerializeField]
    private Color secondaryColor;

    // Reference to the gun that fired this projectile.
    public TeleportGun TeleportGun { get; internal set; }

    void Start()
    {
      m_Projectile = GetComponent<Projectile>();
      m_Collider = GetComponent<Collider2D>();
      m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {

      if(other.gameObject.layer == (other.gameObject.layer & LayerMask.GetMask("Characters", "Default"))
        && other.gameObject != gameObject)
      {
        Debug.Log("// TODO: Adjust code to respond to appropriate layers?");
      }

      gameObject.SetActive(false); // To counteract bug from multiple collisions.
      GetComponent<Collider2D>().enabled = false;
      var position = transform.position;

      // Debug.Log("Triggered: " + other.name 
      //           + "\nType: " + m_Projectile.Type.ToString() 
      //           + "\nPosition: " + position);

      // If in primary mode and I hit a player or rigidbody with the CanTeleport component
      if (m_Projectile.Type == Projectile.ProjectileType.Primary)
      {
        // if (other.GetComponent<Platformer2DUserControl>() != null || other.GetComponent<Teleportable>() != null)
        // {
        //   // Debug.Log("hit something teleportable.");
        //   TeleportGun.MarkTargetObject(other.gameObject);
        // }
        // else 
        if (other.GetComponentInParent<Platformer2DUserControl>() != null)
        {
          // Debug.Log("hit whose parent is something teleportable.");
          GameObject target = other.GetComponentInParent<Platformer2DUserControl>().gameObject;
          TeleportGun.MarkTargetObject(target);
        }
        else if (other.GetComponentInParent<Teleportable>() != null)
        {
          // Debug.Log("hit whose parent is something teleportable.");
          GameObject target = other.GetComponentInParent<Teleportable>().gameObject;
          TeleportGun.MarkTargetObject(target);
        }
      }

      // If in secondary mode and I hit anything,
      // If I do not have a target to teleport, drop a portal here - this becomes the place the target will be teleported to.
      if ((m_Projectile.Type == Projectile.ProjectileType.Secondary))
      {
        // Debug.Log("setting teleport target location.");
        TeleportGun.MarkTargetLocation(position);
      }

      // Projectile should be destroyed last as we need it's transform information above.
      Destroy(gameObject);

    }

    void Update()
    {

      if(m_Projectile.Type == Projectile.ProjectileType.Primary)
        m_SpriteRenderer.color = primaryColor;
      else
        m_SpriteRenderer.color = secondaryColor;

      // If in secondary mode and a crosshair target was selected,
      // check whether we have reached that target.
      if (m_Projectile.crossTarget != null && m_Projectile.Type == Projectile.ProjectileType.Secondary)
      {
        if (m_Collider.OverlapPoint((Vector2)m_Projectile.crossTarget))
        {
          // Debug.Log("Marking target location for secondary in update loop.");
          gameObject.SetActive(false);
          TeleportGun.MarkTargetLocation((Vector2)m_Projectile.crossTarget);
          Destroy(gameObject);
        }
      }

    }

  }
}