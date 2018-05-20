using System;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{
  [Serializable]
  public class EmEvent : UnityEvent<Gun, WhichWeapon> { }

  public class Electromagnetic : MonoBehaviour
  {
    public EmEvent OnStartPull = new EmEvent();
    public EmEvent OnStopPull = new EmEvent();

    public EmEvent OnStartPush = new EmEvent();
    public EmEvent OnStopPush = new EmEvent();

    [SerializeField]
    [Tooltip("Electromagnetic attraction (primary weapon) works on this object. Defaults to true.")]
    internal bool canAttract = true;
    [SerializeField]
    [Tooltip("Electromagnetic repelling (secondary weapon) works on this object. Defaults to true.")]
    internal bool canRepel = true;
  }
}