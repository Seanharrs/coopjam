using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class ThermalProjectile : MonoBehaviour
    {
        [SerializeField]
        private WhichWeapon m_Type;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ThermalSensitive thermalObj = collision.GetComponent<ThermalSensitive>();
            if(thermalObj != null)
            {
                if(m_Type == WhichWeapon.Primary)
                    thermalObj.Freeze();
                else
                    thermalObj.Melt();
            }

            Destroy(gameObject);
        }
    }
}
