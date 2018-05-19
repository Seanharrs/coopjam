using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class ThermalProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ThermalSensitive thermalObj = collision.GetComponent<ThermalSensitive>();
            if(thermalObj != null)
            {
                if(GetComponent<Projectile>().type == WhichWeapon.Primary)
                    thermalObj.Freeze();
                else
                    thermalObj.Melt();
            }

            Destroy(gameObject);
        }
    }
}
