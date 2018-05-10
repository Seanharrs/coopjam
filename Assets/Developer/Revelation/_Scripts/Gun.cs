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

    private Dictionary<WhichWeapon, float> m_LastFired = new Dictionary<WhichWeapon, float> {
      { WhichWeapon.Primary, -5 },
      { WhichWeapon.Secondary, -5 }
    };

    [Header("Details")]
    [Tooltip("Number of times you can fire this weapon in one second.")]
    public float FiringRate = 5;

    [Header("Prefabs/Asset References")]
    public Projectile PrimaryAmmoType;
    public Projectile SecondaryAmmoType;
    public Sprite portraitSprite;

    [Header("Game Objects")]
    public Transform AmmoSpawnLocation;

    public virtual bool FireAtTarget(WhichWeapon weapType, Vector2 target) 
    {
      return Fire(weapType, (target - (Vector2)AmmoSpawnLocation.position).normalized);
    }

    public virtual bool Fire(WhichWeapon weapType, Vector2? direction = null)
    {
      if (Time.time > m_LastFired[weapType] + (1/FiringRate))
      {
        var AmmoToUse = weapType == WhichWeapon.Primary ? PrimaryAmmoType : SecondaryAmmoType;
        if (AmmoSpawnLocation && AmmoToUse)
        {
          var projectile = Instantiate(AmmoToUse, AmmoSpawnLocation.position, Quaternion.identity);
          if (projectile) {
            projectile.Initiate(direction ?? (Vector2)AmmoSpawnLocation.lossyScale * AmmoSpawnLocation.right);
            m_LastFired[weapType] = Time.time;
            return true;
          }
        }
      }
      return false;
    }
  }
}