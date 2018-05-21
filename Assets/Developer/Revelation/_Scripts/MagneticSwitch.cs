using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{

  [RequireComponent(typeof (Animator), typeof (CircuitObject))]
  public class MagneticSwitch : MonoBehaviour, IElectrostatic
  {

    Animator m_Animator;
    CircuitObject m_CircuitObject;

    bool isOn = false;
    CircuitState position = CircuitState.Off;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CircuitObject = GetComponent<CircuitObject>();
    }

    public void OnStartCharge(Gun gun, WhichWeapon weaponType)
    {
      if(position == CircuitState.Off && weaponType == WhichWeapon.Primary)
      {
        m_Animator.SetTrigger("OnPositive");
        position = CircuitState.Positive;
        m_CircuitObject.TriggerStateChange(position);
      }
      if(position == CircuitState.Off && weaponType == WhichWeapon.Secondary)
      {
        m_Animator.SetTrigger("OnNegative");
        position = CircuitState.Negative;
        m_CircuitObject.TriggerStateChange(position);
      }
    }

    public void OnStopCharge(Gun gun, WhichWeapon weaponType)
    {
      if(position == CircuitState.Positive && weaponType == WhichWeapon.Primary
      || position == CircuitState.Negative && weaponType == WhichWeapon.Secondary)
      {
        m_Animator.SetTrigger("Off");
        position = CircuitState.Off;
        m_CircuitObject.TriggerStateChange(position);
      }
    }
  }
}