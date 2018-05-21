using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class Electrostatic : MonoBehaviour
  {
    [SerializeField, Tooltip("// TODO:")]
    private ProjectileEvent OnStartCharge = new ProjectileEvent();
    [SerializeField, Tooltip("// TODO:")]
    private ProjectileEvent OnStopCharge = new ProjectileEvent();

    [SerializeField]
    [Tooltip("Electrostatic disruption (either weapon mode) works on this object. Defaults to true.")]
    internal bool canInterrupt = true;

    internal bool StartCharge(Gun sourceGun, WhichWeapon weapType)
    {
      if(canInterrupt)
        OnStartCharge.Invoke(sourceGun, weapType);
      return canInterrupt;
    }

    internal bool StopCharge(Gun sourceGun, WhichWeapon weapType)
    {
      if(canInterrupt)
        OnStopCharge.Invoke(sourceGun, weapType);
      return canInterrupt;
    }
  }
}