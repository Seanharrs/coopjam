using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop {
  public enum WhichWeapon
  {
    Primary,
    Secondary
  }
  public class Gun : MonoBehaviour
  {

    protected Dictionary<WhichWeapon, float> m_LastFired = new Dictionary<WhichWeapon, float> {
      { WhichWeapon.Primary, -5 },
      { WhichWeapon.Secondary, -5 }
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

    public virtual Projectile FireAtTarget(WhichWeapon weapType, Vector2 target, WhichWeapon type = WhichWeapon.Primary)
    {
      return Fire(weapType, (target - (Vector2)AmmoSpawnLocation.position).normalized, target);
    }

    public virtual Projectile Fire(WhichWeapon weapType, Vector2? direction = null, Vector2? target = null)
    {
      Debug.Log("Firing " + weapType);
      if (Time.time > m_LastFired[weapType] + (1/m_FiringRate))
      {
        var AmmoToUse = weapType == WhichWeapon.Primary ? PrimaryAmmoType : SecondaryAmmoType;
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