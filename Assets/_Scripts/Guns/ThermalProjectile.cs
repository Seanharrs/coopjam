using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class ThermalProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
			      Debug.Log("Thermal triggered.");
            ThermalSensitive thermalObj = collision.GetComponent<ThermalSensitive>();
            Projectile projectileComponent = GetComponent<Projectile>();
            if(thermalObj != null)
            {
                if(projectileComponent.type == FiringState.Primary)
                    thermalObj.Heat(projectileComponent.OwnerGun, projectileComponent.type);
                else
                    thermalObj.Cool(projectileComponent.OwnerGun, projectileComponent.type);
            }

            Destroy(gameObject);
        }
    }
}
