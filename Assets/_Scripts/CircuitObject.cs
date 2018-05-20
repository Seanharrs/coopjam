﻿using System;
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
      private CircuitEvent onStateChanged_Positive = new CircuitEvent();
      [SerializeField]
      private CircuitEvent onStateChanged_Off = new CircuitEvent();
      [SerializeField]
      private CircuitEvent onStateChanged_Negative = new CircuitEvent();

      private void Awake()
      {
          onStateChanged_Positive.AddListener((c) => state = CircuitState.Positive );
          onStateChanged_Off.AddListener((c) => state = CircuitState.Off );
          onStateChanged_Negative.AddListener((c) => state = CircuitState.Negative );
      }

      public void TriggerStateChange(CircuitState newState)
      {
        switch (newState)
        {
          case CircuitState.Off:
            if(state != newState)
            {
              state = newState;
              onStateChanged_Off.Invoke(this);
            }
            break;
          case CircuitState.Positive:
            if(state != newState)
            {
              state = newState;
              onStateChanged_Positive.Invoke(this);
            }
            break;
          case CircuitState.Negative:
            if(state != newState)
            {
              state = newState;
              onStateChanged_Negative.Invoke(this);
            }
            break;
        }
      }
  }
}