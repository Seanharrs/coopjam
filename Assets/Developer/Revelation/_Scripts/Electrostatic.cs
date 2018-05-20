using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class Electrostatic : MonoBehaviour
  {
    public EmEvent OnStartCharge = new EmEvent();
    public EmEvent OnStopCharge = new EmEvent();

    [SerializeField]
    [Tooltip("Electrostatic disruption (either weapon mode) works on this object. Defaults to true.")]
    internal bool canInterrupt = true;
  }
}