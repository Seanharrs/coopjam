using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(CircuitObject))]
public class NonDeadlyLaser : MonoBehaviour
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
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void TurnOff()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO act like camera with connected circuit object
    }
}
