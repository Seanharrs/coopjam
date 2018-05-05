using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    [Header("Prefabs")]
    public Projectile PrimaryAmmoType;
    public Projectile SecondaryAmmoType;

    [Header("Game Objects")]
    public Transform AmmoSpawnLocation;

    public bool Fire(WhichWeapon weapType, Vector2? direction = null)
    {
      if (Time.time > m_LastFired[weapType] + (1/FiringRate))
      {
        // TODO: Spawn projectile.
        if (AmmoSpawnLocation && PrimaryAmmoType)
        {
          var projectile = Instantiate(weapType == WhichWeapon.Primary ? PrimaryAmmoType : SecondaryAmmoType, AmmoSpawnLocation.position, Quaternion.identity);
          if (projectile) {
            projectile.Initiate(direction != null ? (Vector2)direction : (Vector2)AmmoSpawnLocation.lossyScale * AmmoSpawnLocation.right);
            m_LastFired[weapType] = Time.time;
            return true;
          }
        }
      }
      return false;
    }
  }
