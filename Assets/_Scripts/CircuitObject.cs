using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{
  public enum CircuitState
  {
    Off,
    Positive,
    Negative
  }

  [Serializable]
  public class CircuitEvent : UnityEvent<CircuitObject> {}

  public class CircuitObject : MonoBehaviour
  {
      public CircuitState state = CircuitState.Off;
      public bool active { get { return state != CircuitState.Off; } }

      [SerializeField]
      private bool m_IsMultiState = false;

      private bool m_SwitchEnabled = true;
      internal bool SwitchEnabled {
        get { return m_SwitchEnabled; }
        set {
          m_SwitchEnabled = value;
        }
      }

      [SerializeField]
      private CircuitEvent m_OnStateChanged_Positive = new CircuitEvent();
      [SerializeField]
      private CircuitEvent m_OnStateChanged_Off = new CircuitEvent();
      [SerializeField]
      private CircuitEvent m_OnStateChanged_Negative = new CircuitEvent();

      private void Awake()
      {
          m_OnStateChanged_Positive.AddListener((c) => state = CircuitState.Positive );
          m_OnStateChanged_Off.AddListener((c) => state = CircuitState.Off );
          m_OnStateChanged_Negative.AddListener((c) => state = CircuitState.Negative );
      }

      public void DisableSwitchEvents()
      {
        m_SwitchEnabled = false;
      }

      internal void TriggerStateChange(CircuitState newState)
      {
        if(!m_SwitchEnabled) return;
        switch (newState)
        {
          case CircuitState.Off:
            if(state != newState)
            {
              state = newState;
              m_OnStateChanged_Off.Invoke(this);
            }
            break;
          case CircuitState.Positive:
            if(state != newState)
            {
              state = newState;
              m_OnStateChanged_Positive.Invoke(this);
            }
            break;
          case CircuitState.Negative:
            if(state != newState)
            {
              state = newState;
              m_OnStateChanged_Negative.Invoke(this);
            }
            break;
        }
      }
  }
}