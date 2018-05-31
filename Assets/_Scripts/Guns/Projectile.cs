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

    [SerializeField]
    private bool m_DoNotOverrideColor = false;

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
      GetComponent<Rigidbody2D>().velocity = direction.normalized * m_ProjectileSpeed;
      //transform.localRotation = Quaternion.Euler(0, 0, Quaternion.LookRotation(-direction.normalized).eulerAngles.x);
      //if(Mathf.Sign(gun.GetComponentInParent<CoopCharacter2D>().transform.lossyScale.x) < 0)
      //{
      //  Debug.Log("Reversed/flipped.");
      //  var rot = transform.localRotation.eulerAngles;
      //  rot.z += 180;
      //  transform.localRotation = Quaternion.Inverse(Quaternion.Euler(rot));
      //}
      transform.right = direction.normalized;

      m_OwnerGun = gun;
      m_Type = type;

      if(target != null)
        crossTarget = target;

      if (!m_DoNotOverrideColor)
      {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        ParticleSystem system = GetComponent<ParticleSystem>();
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (type == FiringState.Primary)
        {
          renderer.color = m_PrimaryColor;
          if (system)
          {
            foreach (var s in system.GetComponentsInChildren<ParticleSystem>())
            {
              var main = s.main;
              main.startColor = m_PrimaryColor;
            }
          }
          //if(trail)
          //{
          //  trail.startColor = m_PrimaryColor;
          //}
        }
        else
        {
          renderer.color = m_SecondaryColor;
          if (system)
          {
            foreach (var s in system.GetComponentsInChildren<ParticleSystem>())
            {
              var main = s.main;
              main.startColor = m_SecondaryColor;
            }
          }
          //if (trail)
          //{
          //  trail.startColor = m_SecondaryColor;
          //}
        }
      }
    }
  }
}
