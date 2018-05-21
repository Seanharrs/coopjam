using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{

  [RequireComponent(typeof (Animator), typeof (MultiSwitch))]
  public class MagneticSwitch : MonoBehaviour, IElectrostatic
  {

    Animator m_Animator;
    MultiSwitch m_MultiSwitch;

    bool isOn = false;
    SwitchState position = SwitchState.Off;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_MultiSwitch = GetComponent<MultiSwitch>();
    }

    public void OnStartCharge(Gun gun, WhichWeapon weaponType)
    {
      if(position == SwitchState.Off && weaponType == WhichWeapon.Primary)
      {
        m_Animator.SetTrigger("OnPositive");
        position = SwitchState.Positive;
        m_MultiSwitch.OnMultiSwitchStateChanged.Invoke(m_MultiSwitch, position);
      }
      if(position == SwitchState.Off && weaponType == WhichWeapon.Secondary)
      {
        m_Animator.SetTrigger("OnNegative");
        position = SwitchState.Negative;
        m_MultiSwitch.OnMultiSwitchStateChanged.Invoke(m_MultiSwitch, position);
      }
    }

    public void OnStopCharge(Gun gun, WhichWeapon weaponType)
    {
      if(position == SwitchState.Positive && weaponType == WhichWeapon.Primary
      || position == SwitchState.Negative && weaponType == WhichWeapon.Secondary)
      {
        m_Animator.SetTrigger("Off");
        position = SwitchState.Off;
        m_MultiSwitch.OnMultiSwitchStateChanged.Invoke(m_MultiSwitch, position);
      }
    }
  }
}