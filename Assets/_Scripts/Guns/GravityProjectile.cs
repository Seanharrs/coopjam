using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class GravityProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GravitySensitive grav = collision.GetComponent<GravitySensitive>();

            Projectile projectileComponent = GetComponent<Projectile>();
            
            if(grav != null)
                grav.ChangeGravity(projectileComponent.OwnerGun, projectileComponent.type);

            Destroy(gameObject);
        }
    }
}
