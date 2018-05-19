using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class TeleportGun : Gun
  {

    [Header("Teleport Gun specific properties")]
    [SerializeField]
    private GameObject m_TargetLocationPrefab;
    [SerializeField]
    private int m_NumCollisionPasses = 3;

    // The thing that will be teleported.
    private GameObject m_TargetObject;
    // The location that the teleported object will be moved to.
    private Nullable<Vector2> m_TargetLocation;
    private GameObject m_TargetLocationObject;

    public override Projectile Fire(WhichWeapon weapType, Vector2? direction = null, Nullable<Vector2> target = null)
    {
      var projectile = base.Fire(weapType, direction, target);
      if (projectile == null)
      {
        return null;
      }

      TeleportProjectile tProjectile = projectile.GetComponent<TeleportProjectile>();
      if (tProjectile == null)
      {
        Destroy(projectile.gameObject);
        return null;
      }

      tProjectile.TeleportGun = this;

      return null;
    }

    internal void MarkTargetObject(GameObject targetObject)
    {
      // Debug.Log("Marking target object.");
      m_TargetObject = targetObject;
      TryCompleteTeleport();
    }

    internal void MarkTargetLocation(Vector2 targetLocation)
    {
      // Debug.Log("Marking target location.");
      m_TargetLocation = targetLocation;
      if(m_TargetLocationObject == null)
        m_TargetLocationObject = Instantiate(m_TargetLocationPrefab, targetLocation, Quaternion.identity);
      else
        m_TargetLocationObject.transform.position = targetLocation;

      for(var i = 0; i < m_NumCollisionPasses; i++)
      {
        AdjustForCollisions(m_TargetLocationObject.GetComponent<BoxCollider2D>());
      }
      TryCompleteTeleport();
    }

    private void TryCompleteTeleport()
    {
      if (m_TargetObject != null && m_TargetLocation != null)
      {
        // Debug.Log("Completing teleport to " + m_TargetLocation.ToString());
        // TODO: Play sound, animation, effect, etc.
        m_TargetObject.transform.position = (Vector2)m_TargetLocation;
        for(var i = 0; i < m_NumCollisionPasses; i++)
        {
          AdjustForCollisions(m_TargetObject.GetComponent<BoxCollider2D>());
        }

        // Debug.Log("Nullifying stuff.");
        m_TargetLocation = null;
        Destroy(m_TargetLocationObject);
        m_TargetObject = null;
        // TEST: 
        // Debug.Log("Set target location to null: " + (m_TargetLocation == null).ToString());
        // Debug.Log("Set target object to null: " + (m_TargetObject == null).ToString());

      // } else {
      //   Debug.Log("Can't complete teleport");
      }
    }

    private void AdjustForCollisions(Collider2D collider)
    {
      //var collider = m_TargetLocationObject.GetComponent<BoxCollider2D>();
      var colliderCenter = (Vector2)collider.transform.position + collider.offset;
      Collider2D[] results = new Collider2D[10];
      int overlappedCount = collider.OverlapCollider(new ContactFilter2D(), results);
      if (overlappedCount > 0)
      {
        foreach (var c in results)
        {
          Debug.Log("Checking... " + c);
          if(!c) return;
          if (!c.isTrigger && c != collider)
          {
            var cCenter = (Vector2)c.transform.position + c.offset;

            var desiredDistances = new Vector2(collider.bounds.extents.x + c.bounds.extents.x, collider.bounds.extents.y + c.bounds.extents.y);
            var directionVector = colliderCenter - cCenter;
            Debug.Log("Desired Distances: " + desiredDistances);

            var actualDistance2D = (colliderCenter - cCenter);
            Debug.Log("Actual Distances: " + actualDistance2D);

            var p = collider.transform.position;
            Debug.Log("Original Position: " + p);
            if(Mathf.Abs(actualDistance2D.x) < Mathf.Abs(actualDistance2D.y))
            {
              Debug.Log("Adjusting on X... " + c);
              p.x = cCenter.x + (desiredDistances.x * Mathf.Sign(directionVector.x));
            }
            else
            {
              Debug.Log("Adjusting on Y... " + c);
              p.y = cCenter.y + (desiredDistances.y * Mathf.Sign(directionVector.y));
            }
            Debug.Log("New position: " + p);
            collider.transform.position = p;

          }
        }
      }
    }


    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
      // Make sure we don't have residual teleport portals.
      if(m_TargetLocationObject != null)
        Destroy(m_TargetLocationObject);
    }

  }
}