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
      m_TargetObject = targetObject;
      TryCompleteTeleport();
    }

    internal void MarkTargetLocation(Vector2 targetLocation)
    {
      m_TargetLocation = targetLocation;
      if(m_TargetLocationObject == null)
        m_TargetLocationObject = Instantiate(m_TargetLocationPrefab, targetLocation, Quaternion.identity);
      else
        m_TargetLocationObject.transform.position = targetLocation;
      TryCompleteTeleport();
    }

    private void TryCompleteTeleport()
    {
      if (m_TargetObject != null && m_TargetLocation != null)
      {
        // TODO: Play sound, animation, effect, etc.
        m_TargetObject.transform.position = (Vector2)m_TargetLocation;

        m_TargetLocation = null;
        Destroy(m_TargetLocationObject);
        // TEST: Debug.Log("Set target location to null: " + (m_TargetLocation == null).ToString());
        m_TargetObject = null;
      }
    }

  }
}