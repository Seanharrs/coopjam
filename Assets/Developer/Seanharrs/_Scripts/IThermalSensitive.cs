namespace Coop
{
  internal interface IThermalSensitive
  {
    void OnThermalHit_Heat(Gun gun, WhichWeapon weaponType);
    void OnThermalHit_Cool(Gun gun, WhichWeapon weaponType);
  }
}