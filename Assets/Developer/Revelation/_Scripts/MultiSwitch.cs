using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{

  public enum SwitchState
  {
    Off = 0,
    Positive = 1,
    Negative = 2
  }

  [Serializable]
  public class MultiSwitchEvent : UnityEvent<MultiSwitch, SwitchState> {}
  
  public class MultiSwitch : MonoBehaviour {

    public MultiSwitchEvent OnMultiSwitchStateChanged = new MultiSwitchEvent();

  }
}