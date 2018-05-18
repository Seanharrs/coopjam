using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CircuitObject : MonoBehaviour
{
    public bool active { get; private set; }

    public UnityEvent onTriggerStart;
    
    public UnityEvent onTriggerEnd;

    private void Awake()
    {
        onTriggerStart.AddListener(() => active = true);
        onTriggerEnd.AddListener(() => active = false);
    }
}
