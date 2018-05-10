using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeathCollider), typeof(CircuitObject))]
public class DeadlyLaser : MonoBehaviour
{
    private CircuitObject m_Circuit;

    private void Awake()
    {
        m_Circuit = GetComponent<CircuitObject>();
        m_Circuit.OnActivate(TurnOn);
        m_Circuit.OnDeactivate(TurnOff);
    }

    private void TurnOn()
    {
        GetComponent<DeathCollider>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void TurnOff()
    {
        GetComponent<DeathCollider>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
