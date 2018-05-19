﻿using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class GravityProjectile : MonoBehaviour
    {
        private Projectile m_Projectile;
        
        private float m_EffectLength;

        private void Awake() { m_Projectile = GetComponent<Projectile>(); }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("test");
            GravitySensitive grav = collision.GetComponent<GravitySensitive>();
            if(grav != null)
            {
                Debug.Log("okay");
                grav.ChangeGravity(m_Projectile.type);
            }
            Destroy(gameObject);
        }
    }
}
