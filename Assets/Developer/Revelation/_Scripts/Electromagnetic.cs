using System;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{
  public class Electromagnetic : MonoBehaviour
  {
    [SerializeField, Tooltip("Event triggered when primary weapon effect begins affecting this object.")]
    private ProjectileEvent OnStartPull = new ProjectileEvent();
    [SerializeField, Tooltip("Event triggered when primary weapon effect stops affecting this object.")]
    private ProjectileEvent OnStopPull = new ProjectileEvent();

    [SerializeField, Tooltip("Event triggered when secondary weapon effect begins affecting this object.")]
    private ProjectileEvent OnStartPush = new ProjectileEvent();
    [SerializeField, Tooltip("Event triggered when secondary weapon effect stops affecting this object.")]
    private ProjectileEvent OnStopPush = new ProjectileEvent();

    [SerializeField]
    [Tooltip("Electromagnetic attraction (primary weapon) works on this object. Defaults to true.")]
    internal bool canAttract = true;
    [SerializeField]
    [Tooltip("Electromagnetic repelling (secondary weapon) works on this object. Defaults to true.")]
    internal bool canRepel = true;

    internal bool StartPull(Gun sourceGun, FiringState weapType)
    {
      if(canAttract)
        OnStartPull.Invoke(sourceGun, weapType);
      return canAttract;
    }
    internal bool StopPull(Gun sourceGun, FiringState weapType)
    {
      if(canAttract)
        OnStopPull.Invoke(sourceGun, weapType);
      return canAttract;
    }

    internal bool StartPush(Gun sourceGun, FiringState weapType)
    {
      if(canRepel)
        OnStartPush.Invoke(sourceGun, weapType);
      return canRepel;
    }
    internal bool StopPush(Gun sourceGun, FiringState weapType)
    {
      if(canRepel)
        OnStopPush.Invoke(sourceGun, weapType);
      return canRepel;
    }

  }
}