using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(CircuitObject), typeof(AudioSource))]
public class NonDeadlyLaser : MonoBehaviour
{
    private CircuitObject m_Circuit;
    
    [SerializeField]
    private float m_MaxAlertTime = 1f;
    private float m_AlertTimeLeft;

    private void Awake() { m_Circuit = GetComponent<CircuitObject>(); }

    public void TurnOn()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TurnOff()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
            return;

        if(m_AlertTimeLeft <= 0)
            StartCoroutine(TriggerLaser());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
            return;

        m_AlertTimeLeft = m_MaxAlertTime;
    }

    private IEnumerator TriggerLaser()
    {
        m_AlertTimeLeft = m_MaxAlertTime;

        if(!m_Circuit.active)
            m_Circuit.onTriggerStart.Invoke();

        while(m_AlertTimeLeft > 0)
        {
            m_AlertTimeLeft -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(m_Circuit.active)
            m_Circuit.onTriggerEnd.Invoke();

        yield return null;
    }
}
