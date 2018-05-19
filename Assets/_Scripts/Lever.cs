using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Interactable), typeof(CircuitObject))]
public class Lever : MonoBehaviour
{
    public enum ActivationType { Toggle, Timed };
    
    [SerializeField]
    private ActivationType m_ActivationType;

    [SerializeField, Tooltip("The time in seconds that the lever should remain active for, if Timed not Toggle")]
    private float m_TimeActive;
    private float m_LastActivated;

    private CircuitObject m_CircuitObj;

    private bool m_IsActive;

    private const float ACTIVE_ROT = 310f;
    private const float INACTIVE_ROT = 50f;

    private const float FLIP_SPEED = 180f;

    private void Awake() { m_CircuitObj = GetComponent<CircuitObject>(); }

    public void Interact()
    {
        if(m_IsActive)
        {
            bool remainActive = (Time.time - m_LastActivated) < m_TimeActive;
            if(m_ActivationType == ActivationType.Timed && remainActive)
                return;

            ToggleLever(false);
        }
        else
        {
            ToggleLever(true);

            if(m_ActivationType == ActivationType.Timed)
            {
                m_LastActivated = Time.time;
                Invoke("Interact", m_TimeActive + 0.1f);
            }
        }
    }

    private void ToggleLever(bool state)
    {
        m_IsActive = state;

        if(state && !m_CircuitObj.active)
            m_CircuitObj.onTriggerStart.Invoke();
        else if(!state && m_CircuitObj.active)
            m_CircuitObj.onTriggerEnd.Invoke();

        StartCoroutine(FlipBar());
    }

    private IEnumerator FlipBar()
    {
        Transform barTransform = transform.GetChild(0);

        float newRotZ = m_IsActive ? ACTIVE_ROT : INACTIVE_ROT;
        float currRotZ = barTransform.rotation.eulerAngles.z;
        int direction = m_IsActive ? -1 : 1; // right : left

        while(Mathf.Abs(currRotZ - newRotZ) >= 1f)
        {
            currRotZ += Time.fixedDeltaTime * FLIP_SPEED * direction;
            if(currRotZ >= 360f)
                currRotZ -= 360f;
            else if(currRotZ < 0f)
                currRotZ += 360f;

            barTransform.rotation = Quaternion.Euler(0, 0, currRotZ);

            yield return new WaitForFixedUpdate();
        }

        barTransform.rotation = Quaternion.Euler(0, 0, newRotZ);

        yield return null;
    }
}
