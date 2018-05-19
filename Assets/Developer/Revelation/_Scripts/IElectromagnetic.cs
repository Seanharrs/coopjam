namespace Coop
{
  interface IElectromagnetic
  {
    void OnStartPull(Gun gun);
    void OnStopPull(Gun gun);
    void OnStartPush(Gun gun);
    void OnStopPush(Gun gun);
  }
}