using UnityEngine;

namespace Coop 
{
  [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
  public class ThermalSensitive : MonoBehaviour
  {
    [SerializeField, Tooltip("Event triggered when primary weapon projectile hits this object.")]
    private ProjectileEvent OnThermalHit_Heat = new ProjectileEvent();
    [SerializeField, Tooltip("Event triggered when secondary weapon projectile hits this object.")]
    private ProjectileEvent OnThermalHit_Cool = new ProjectileEvent();

    [SerializeField, Tooltip("Thermal heating works on this object. Defaults to true.")]
    internal bool m_canHeat = true;
    [SerializeField, Tooltip("Thermal cooling works on this object. Defaults to true.")]
    internal bool m_canCool = true;

    internal bool Cool(Gun sourceGun, FiringState type)
    {
      if(m_canCool)
        OnThermalHit_Cool.Invoke(sourceGun, type);
      return m_canCool;
    }

    internal bool Heat(Gun sourceGun, FiringState type)
    {
      if(m_canHeat)
        OnThermalHit_Heat.Invoke(sourceGun, type);
      return m_canHeat;
    }
  }
}
