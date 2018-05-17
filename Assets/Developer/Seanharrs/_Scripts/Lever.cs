using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Lever : MonoBehaviour, IInteractable
{
    public enum ActivationType { Toggle, Timed };

    [SerializeField]
    private ActivationType m_ActivationType;

    [SerializeField]
    private CircuitObject m_ConnectedObj;

    [SerializeField, Tooltip("The time in seconds that the lever should remain active for, if Timed not Toggle")]
    private float m_TimeActive;
    private float m_LastActivated;

    private bool m_IsActive;

    public void Interact()
    {
        if(m_IsActive)
        {
            bool remainActive = (Time.time - m_LastActivated) < m_TimeActive;
            if(m_ActivationType == ActivationType.Timed && remainActive)
                return;

            m_IsActive = false;
            m_ConnectedObj.Deactivate();
        }
        else
        {
            m_IsActive = true;
            m_ConnectedObj.Activate();

            if(m_ActivationType == ActivationType.Timed)
            {
                m_LastActivated = Time.time;
                Invoke("Interact", m_TimeActive);
            }
        }
    }
}
