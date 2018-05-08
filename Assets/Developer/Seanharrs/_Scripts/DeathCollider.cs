using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DeathCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<DeathAnimation>())
            return;

        Health hp = other.GetComponent<Health>();
        if(hp)
            hp.ForceKill();
    }
}
