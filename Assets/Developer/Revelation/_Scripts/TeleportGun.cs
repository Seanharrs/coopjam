using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class TeleportGun : Gun
  {

    // The thing that will be teleported.
    private GameObject m_TargetObject;
    // The location that the teleported object will be moved to.
    private Nullable<Vector2> m_TargetLocation;

    public override Projectile Fire(WhichWeapon weapType, Vector2? direction = null, Nullable<Vector2> target = null)
    {
      Debug.Log("Firing " + (int)weapType);
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
      Debug.Log("Marking target object.");
      m_TargetObject = targetObject;
      TryCompleteTeleport();
    }

    internal void MarkTargetLocation(Vector2 targetLocation)
    {
      Debug.Log("Marking target location.");
      m_TargetLocation = targetLocation;
      TryCompleteTeleport();
    }

    private void TryCompleteTeleport()
    {
      if (m_TargetObject != null && m_TargetLocation != null)
      {
        // TODO: Play sound, animation, effect, etc.
        m_TargetObject.transform.position = (Vector2)m_TargetLocation;

        m_TargetLocation = null;
        m_TargetObject = null;
      }
    }

  }
}