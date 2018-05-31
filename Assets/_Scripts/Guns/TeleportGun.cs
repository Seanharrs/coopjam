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
    // Reference to the instance of teleport portal being used for this gun, if any
    private GameObject m_TargetLocationObject;

    public override Projectile Fire(FiringState weapType, Vector2? direction = null, Nullable<Vector2> target = null)
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
      {
        m_TargetLocationObject.transform.position = targetLocation;
        m_TargetLocationObject.GetComponent<AudioSource>().Play();
      }

      AdjustForCollisions(m_TargetLocationObject.GetComponent<BoxCollider2D>());

      TryCompleteTeleport();
    }

    private void TryCompleteTeleport()
    {
      if (m_TargetObject != null && m_TargetLocation != null)
      {
        // Debug.Log("Completing teleport to " + m_TargetLocation.ToString());
        // TODO: Play sound, animation, effect, etc.
        m_TargetObject.transform.position = (Vector2)m_TargetLocation;
        //for(var i = 0; i < m_NumCollisionPasses; i++)
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
      //var colliderCenter = (Vector2)collider.transform.position + collider.offset;
      Collider2D[] results = new Collider2D[10];
      ContactFilter2D filter = new ContactFilter2D()
      {
        useLayerMask = true,
        layerMask = ~LayerMask.GetMask("Characters")
      };
      int overlappedCount = collider.OverlapCollider(filter, results);
      if (overlappedCount > 0)
      {
        //Debug.Log("target:" + m_TargetLocation.Value);
        bool adjustedX = false, adjustedY = false;
        for (int i = 0; i < results.Length; ++i)
        {
          Collider2D c = results[i];
          //Debug.Log("Checking... " + c);
          if(!c) return;

          if(!c.isTrigger && c != collider)
          {
            Vector3 cHitPos = c.transform.InverseTransformPoint(m_TargetLocation.Value) - (Vector3)c.offset;
            //Debug.Log("local hit pos: " + cHitPos);

            var p = collider.transform.position;
            //Debug.Log("Original Position: " + p);

            bool hitTopOrBottom = Mathf.Abs(cHitPos.y) >= c.bounds.extents.y;
            bool hitLeftOrRight = Mathf.Abs(cHitPos.x) >= c.bounds.extents.x;

            //Always adjust first hit, ignore latter "indirect" hits
            if(i > 0 && hitTopOrBottom && hitLeftOrRight)
              continue;

            if(hitTopOrBottom)
            {
              //Debug.Log("Adjusting on Y... " + c);
              if(adjustedY) Debug.Log("already adjusted");
              else p.y += collider.bounds.extents.y * Mathf.Sign(cHitPos.y);
              adjustedY = true;
            }

            if(hitLeftOrRight)
            {
              //Debug.Log("Adjusting on X... " + c);
              if(adjustedX) Debug.Log("already adjusted");
              else p.x += collider.bounds.extents.x * Mathf.Sign(cHitPos.x);
              adjustedX = true;
            }

            //Debug.Log("New position: " + p);
            collider.transform.position = p;

            //
            // DOT PRODUCT - sometimes adjusted along the wrong axis
            //

            //Vector3 cHitPos = c.transform.InverseTransformPoint(m_TargetLocation.Value) - (Vector3)c.offset;
            //Vector3 cHitDir = cHitPos.normalized;
            //float up = Vector3.Dot(cHitDir, Vector3.up);
            //float right = Vector3.Dot(cHitDir, Vector3.right);
            //Debug.Log("up dot: " + up);
            //Debug.Log("right dot: " + right);
            //var p = collider.transform.position; //+collider.offset
            //Debug.Log("Original Position: " + p);
            //if(Mathf.Abs(up) < Mathf.Abs(right))
            //{
            //  Debug.Log("Adjusting on Y... " + c);
            //  if(adjustedY) Debug.Log("already adjusted");
            //  else p.y += collider.bounds.extents.y * Mathf.Sign(up);
            //  adjustedY = true;
            //}
            //else
            //{
            //  Debug.Log("Adjusting on X... " + c);
            //  if(adjustedX) Debug.Log("already adjusted");
            //  else p.x += collider.bounds.extents.x * Mathf.Sign(right);
            //  adjustedX = true;
            //}
            //Debug.Log("New position: " + p);
            //collider.transform.position = p;

            //
            // ORIGINAL SOLUTION - edge cases
            //

            //var cCenter = (Vector2)c.transform.position + c.offset;

            //var desiredDistances = new Vector2(collider.bounds.extents.x + c.bounds.extents.x, collider.bounds.extents.y + c.bounds.extents.y);
            //var directionVector = colliderCenter - cCenter;
            //Debug.Log("Desired Distances: " + desiredDistances);

            //var actualDistance2D = (colliderCenter - cCenter);
            //Debug.Log("Actual Distances: " + actualDistance2D);

            //var p = collider.transform.position;
            //Debug.Log("Original Position: " + p);
            //if(Mathf.Abs(actualDistance2D.x) < Mathf.Abs(actualDistance2D.y))
            //{
            //  Debug.Log("Adjusting on X... " + c);
            //  p.x = cCenter.x + (desiredDistances.x * Mathf.Sign(directionVector.x));
            //}
            //else
            //{
            //  Debug.Log("Adjusting on Y... " + c);
            //  p.y = cCenter.y + (desiredDistances.y * Mathf.Sign(directionVector.y));
            //}
            //Debug.Log("New position: " + p);
            //collider.transform.position = p;

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