using System;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{
  [Serializable]
  public class ProjectileEvent : UnityEvent<Gun, FiringState> { }

  [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
  public class Projectile : MonoBehaviour
  {
    [Header("Projectile")]
    [SerializeField]
    private float m_ProjectileSpeed = 10;
    
    [SerializeField]
    private Color m_PrimaryColor = Color.white;

    [SerializeField]
    private Color m_SecondaryColor = Color.white;

    private FiringState m_Type = FiringState.Primary;
    internal FiringState type { get { return m_Type; } }

    internal Vector2? crossTarget = null;

    private Gun m_OwnerGun;
    public Gun OwnerGun
    {
      get { return m_OwnerGun; }
    }

    public virtual void Initiate(Vector2 direction, Gun gun, FiringState type = FiringState.Primary, Vector2? target = null)
    {
      GetComponent<Rigidbody2D>().velocity = direction * m_ProjectileSpeed;
      var systems = GetComponentsInChildren<ParticleSystem>();
      foreach(var ps in systems) {
        var m = ps.main;
        m.startRotation = Quaternion.LookRotation(GetComponent<Rigidbody2D>().velocity, Vector3.up).eulerAngles.z;
        Debug.Log("Set rotation to: " + m.startRotation);
        ps.Play();
      }
      m_OwnerGun = gun;
      m_Type = type;

      if(target != null)
        crossTarget = target;
      
      SpriteRenderer renderer = GetComponent<SpriteRenderer>();
      if(type == FiringState.Primary)
        renderer.color = m_PrimaryColor;
      else
        renderer.color = m_SecondaryColor;
    }
  }
}
