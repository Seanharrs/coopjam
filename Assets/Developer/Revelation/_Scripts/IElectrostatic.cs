namespace Coop
{
  internal interface IElectrostatic
  {
    void OnStartCharge(Gun gun, WhichWeapon weaponType);
    void OnStopCharge(Gun gun, WhichWeapon weaponType);
  }
}