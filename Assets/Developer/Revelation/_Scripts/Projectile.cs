using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
  public class Projectile : MonoBehaviour
  {
    [Header("Projectile")]
    [SerializeField]
    private float m_ProjectileSpeed = 10;
    
    [SerializeField]
    private Color m_PrimaryColor;

    [SerializeField]
    private Color m_SecondaryColor;

    private WhichWeapon m_Type = WhichWeapon.Primary;
    internal WhichWeapon type { get { return m_Type; } }

    internal Vector2? crossTarget = null;

    public virtual void Initiate(Vector2 direction, WhichWeapon type = WhichWeapon.Primary, Vector2? target = null)
    {
      GetComponent<Rigidbody2D>().velocity = direction * m_ProjectileSpeed;
      m_Type = type;
      if(target != null)
        crossTarget = target;
      
      SpriteRenderer renderer = GetComponent<SpriteRenderer>();
      if(type == WhichWeapon.Primary)
        renderer.color = m_PrimaryColor;
      else
        renderer.color = m_SecondaryColor;
    }
  }
}
