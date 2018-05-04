using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private enum Direction { Left = -1, Right = 1 };

    [SerializeField]
    private float[] m_LookRotationsZ;

    [SerializeField]
    private Direction m_InitialDirection;

    private const float SPEED = 40f;

    private AutoDoor connectedDoor;

    private const float MAX_ALERT_TIME = 10f;
    private float m_AlertTimeLeft;
    private bool m_OnAlert;

    private void Awake()
    {
        connectedDoor = FindObjectOfType<AutoDoor>();
        connectedDoor.OpenDoor();

        for(int i = 0; i < m_LookRotationsZ.Length; i++)
        {
            if(m_LookRotationsZ[i] < 0)
                m_LookRotationsZ[i] += 360;
        }

        transform.rotation = Quaternion.Euler(0, 0, m_LookRotationsZ[0]);
    }

    private void Update()
    {
        if(m_OnAlert && connectedDoor.isOpen)
            connectedDoor.CloseDoor();
        else if(!m_OnAlert && !connectedDoor.isOpen)
            connectedDoor.OpenDoor();
    }

    private IEnumerator LookAround()
    {
        m_OnAlert = true;
        m_AlertTimeLeft = MAX_ALERT_TIME;

        int i = 0;
        float newRotZ = m_LookRotationsZ[0];
        int direction = (int)m_InitialDirection;
        while(m_AlertTimeLeft > 0f)
        {
            float currRotZ = transform.rotation.eulerAngles.z;

            if(Mathf.Abs(currRotZ - newRotZ) < 1f)
            {
                i = (i + 1) % m_LookRotationsZ.Length;
                direction *= -1;
                newRotZ = m_LookRotationsZ[i];
            }

            currRotZ += Time.fixedDeltaTime * SPEED * direction;
            if(currRotZ >= 360f)
                currRotZ = 0;
            else if(currRotZ < 0f)
                currRotZ += 360f;

            transform.rotation = Quaternion.Euler(0, 0, currRotZ);
            m_AlertTimeLeft -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        m_OnAlert = false;
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!m_OnAlert)
            StartCoroutine(LookAround());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        m_AlertTimeLeft = MAX_ALERT_TIME;
    }
}
