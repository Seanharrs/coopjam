namespace Coop
{
  interface IElectromagnetic
  {
    void OnStartPull(Gun gun, WhichWeapon weaponType);
    void OnStopPull(Gun gun, WhichWeapon weaponType);
    void OnStartPush(Gun gun, WhichWeapon weaponType);
    void OnStopPush(Gun gun, WhichWeapon weaponType);
  }
}