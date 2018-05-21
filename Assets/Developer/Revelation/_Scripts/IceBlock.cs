﻿using UnityEngine;

namespace Coop {
  public class IceBlock : MonoBehaviour, IThermalSensitive
  {
    [SerializeField]
    private int freezeLevel = 5;

    public void OnThermalHit_Cool(Gun gun, WhichWeapon weaponType)
    {
      return;
    }

    public void OnThermalHit_Heat(Gun gun, WhichWeapon weaponType)
    {
      // TODO: Gradual size reduction Visuals, etc.?
      if(freezeLevel == 0)
        Destroy(gameObject);
      else
        freezeLevel--;
    }
  }
}