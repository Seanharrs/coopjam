using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitObject : MonoBehaviour
{
    public bool active { get; private set; }

    public Action Activate { get; private set; }
    public Action Deactivate { get; private set; }

    public void OnActivate(Action e) { Activate += e; }
    public void OnDeactivate(Action e) { Deactivate += e; }

    private void Awake()
    {
        Activate += () => active = true;
        Deactivate += () => active = false;
    }
}
