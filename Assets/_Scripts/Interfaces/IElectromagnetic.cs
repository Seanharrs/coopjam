namespace Coop
{
  interface IElectromagnetic
  {
    void OnStartPull(Gun gun, FiringState weaponType);
    void OnStopPull(Gun gun, FiringState weaponType);
    void OnStartPush(Gun gun, FiringState weaponType);
    void OnStopPush(Gun gun, FiringState weaponType);
  }
}