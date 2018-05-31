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
  [RequireComponent(typeof (AudioSource))]
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
    [SerializeField]
    internal Projectile PrimaryAmmoType;
    [SerializeField]
    internal AudioClip m_PrimaryAmmoFireSound;
    [SerializeField]
    internal Projectile SecondaryAmmoType;
    [SerializeField]
    internal AudioClip m_SecondaryAmmoFireSound;
    [SerializeField]
    internal Sprite portraitSprite;

    [Header("Game Objects")]
    [SerializeField]
    internal Transform AmmoSpawnLocation;
    internal AudioSource m_AudioSource;

    internal CoopCharacter2D m_OwnerCharacter;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public virtual Projectile FireAtTarget(FiringState weapType, Vector2 target, FiringState type = FiringState.Primary)
    {
      return Fire(weapType, (target - (Vector2)AmmoSpawnLocation.position).normalized, target);
    }

    public virtual Projectile Fire(FiringState ammoType, Vector2? direction = null, Vector2? target = null)
    {

      if (Time.time > m_LastFired[ammoType] + (1/m_FiringRate))
      {
        var AmmoToUse = ammoType == FiringState.Primary ? PrimaryAmmoType : SecondaryAmmoType;
        if (AmmoSpawnLocation && AmmoToUse)
        {
          var projectile = Instantiate(AmmoToUse, AmmoSpawnLocation.position, Quaternion.identity);
          if (projectile) {
            
            m_AudioSource.clip = ammoType == FiringState.Primary ? m_PrimaryAmmoFireSound : m_SecondaryAmmoFireSound;
            m_AudioSource.loop = false;
            m_AudioSource.Play();
            
            projectile.Initiate(direction ?? (Vector2)AmmoSpawnLocation.lossyScale.normalized * AmmoSpawnLocation.right, this, ammoType, target);
            m_LastFired[ammoType] = Time.time;
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