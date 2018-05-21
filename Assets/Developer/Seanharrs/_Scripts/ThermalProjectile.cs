using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class ThermalProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
          Debug.Log("THermal triggered.");
            ThermalSensitive thermalObj = collision.GetComponent<ThermalSensitive>();
            Projectile projectileComponent = GetComponent<Projectile>();
            if(thermalObj != null)
            {
                if(GetComponent<Projectile>().type == WhichWeapon.Primary)
                    thermalObj.Cool(projectileComponent.OwnerGun, projectileComponent.type);
                else
                    thermalObj.Heat(projectileComponent.OwnerGun, projectileComponent.type);
            }

            Destroy(gameObject);
        }
    }
}
