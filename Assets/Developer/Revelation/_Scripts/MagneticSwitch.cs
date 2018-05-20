using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof (Animator))]
  public class MagneticSwitch : MonoBehaviour, IElectrostatic
  {

    Animator m_Animator;
    CircuitObject m_CircuitObject;

    bool isOn = false;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CircuitObject = GetComponent<CircuitObject>();
    }

    public void OnStartCharge(Gun gun, WhichWeapon weaponType)
    {
      if(isOn && weaponType == WhichWeapon.Secondary)
      {
        m_Animator.SetTrigger("Off");
        m_CircuitObject.onTriggerEnd.Invoke();
        isOn = false;
      }
      else if(!isOn && weaponType == WhichWeapon.Primary)
      {
        m_Animator.SetTrigger("On");
        m_CircuitObject.onTriggerStart.Invoke();
        isOn = true;
      }

    }

    public void OnStopCharge(Gun gun, WhichWeapon weaponType)
    {
      // TODO: anything?
    }
  }
}