namespace Coop
{
  internal interface IElectrostatic
  {
    void OnStartCharge(Gun gun, FiringState weaponType);
    void OnStopCharge(Gun gun, FiringState weaponType);
  }
}