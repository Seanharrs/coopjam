using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class GravityProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GravitySensitive grav = collision.GetComponent<GravitySensitive>();
            if(grav != null)
                grav.ChangeGravity(GetComponent<Projectile>().type);

            Destroy(gameObject);
        }
    }
}
