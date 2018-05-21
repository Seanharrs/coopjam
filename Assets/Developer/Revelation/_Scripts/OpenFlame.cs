using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof (ThermalSensitive))]
  public class OpenFlame : MonoBehaviour, IThermalSensitive
  {
    public void OnThermalHit_Cool(Gun gun, WhichWeapon weaponType)
    {
      // TODO: More visual effects (smoke trail particle system?), SFX (sizzle) etc.
      Destroy(gameObject);
    }

    public void OnThermalHit_Heat(Gun gun, WhichWeapon weaponType)
    {
      return;
    }
  }
}