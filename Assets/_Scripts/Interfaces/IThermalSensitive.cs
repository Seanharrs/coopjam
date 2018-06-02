namespace Coop
{
  internal interface IThermalSensitive
  {
    void OnThermalHit_Heat(Gun gun, FiringState weaponType);
    void OnThermalHit_Cool(Gun gun, FiringState weaponType);
  }
}