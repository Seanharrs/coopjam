using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public interface IMultiSwitchStateListener {
    void OnSwitchStateChanged(MultiSwitch multiSwitch, SwitchState state);
  }
}