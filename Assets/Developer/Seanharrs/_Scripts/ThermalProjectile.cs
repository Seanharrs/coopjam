using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class ThermalProjectile : MonoBehaviour
    {
        [SerializeField]
        private Projectile.ProjectileType m_Type;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ThermalSensitive thermalObj = collision.GetComponent<ThermalSensitive>();
            if(thermalObj != null)
            {
                if(m_Type == Projectile.ProjectileType.Primary)
                    thermalObj.Freeze();
                else
                    thermalObj.Melt();
            }

            Destroy(gameObject);
        }
    }
}
