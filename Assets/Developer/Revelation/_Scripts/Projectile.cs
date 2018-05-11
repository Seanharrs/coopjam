using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof (Rigidbody2D), typeof (Collider2D))]
  public class Projectile : MonoBehaviour {

    public enum ProjectileType { Primary, Secondary };

    [Header("Projectile")]
    public float projectileSpeed = 10;
    
    private Nullable<ProjectileType> m_Type = null;
    internal Nullable<ProjectileType> Type
    {
      get { return m_Type; }
    }

    internal Nullable<Vector2> crossTarget = null;

    public virtual void Initiate(Vector2 direction, WhichWeapon type = WhichWeapon.Primary, Nullable<Vector2> target = null) {
      GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
      m_Type = type == WhichWeapon.Primary ? ProjectileType.Primary : ProjectileType.Secondary;
      if(target != null)
      {
        crossTarget = target;
      }
    }

  }
}