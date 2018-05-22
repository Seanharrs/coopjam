using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop {
  public enum FiringState
  {
    None,
    Primary,
    Secondary
  }
  public class Gun : MonoBehaviour
  {

    protected Dictionary<FiringState, float> m_LastFired = new Dictionary<FiringState, float> {
      { FiringState.Primary, -5 },
      { FiringState.Secondary, -5 }
    };

    [Header("Details")]
    [SerializeField, Tooltip("Label used in game.")]
    private string m_GunName = "";
    public string GunName { get { return m_GunName; }}
    [SerializeField, Tooltip("Number of times you can fire this weapon in one second.")]
    private float m_FiringRate = 5;


    [Header("Prefabs/Asset References")]
    public Projectile PrimaryAmmoType;
    public Projectile SecondaryAmmoType;
    public Sprite portraitSprite;

    [Header("Game Objects")]
    public Transform AmmoSpawnLocation;

    public virtual Projectile FireAtTarget(FiringState weapType, Vector2 target, FiringState type = FiringState.Primary)
    {
      return Fire(weapType, (target - (Vector2)AmmoSpawnLocation.position).normalized, target);
    }

    public virtual Projectile Fire(FiringState weapType, Vector2? direction = null, Vector2? target = null)
    {
      if (Time.time > m_LastFired[weapType] + (1/m_FiringRate))
      {
        var AmmoToUse = weapType == FiringState.Primary ? PrimaryAmmoType : SecondaryAmmoType;
        if (AmmoSpawnLocation && AmmoToUse)
        {
          var projectile = Instantiate(AmmoToUse, AmmoSpawnLocation.position, Quaternion.identity);
          if (projectile) {
            projectile.Initiate(direction ?? (Vector2)AmmoSpawnLocation.lossyScale.normalized * AmmoSpawnLocation.right, this, weapType, target);
            m_LastFired[weapType] = Time.time;
            return projectile;
          }
        }
      }
      return null;
    }

    public virtual void StopFiring()
    {
      // Placeholder
    }

  }
}