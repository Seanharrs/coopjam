using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircuitObject))]
public class SecurityCamera : MonoBehaviour
{
    private enum Direction { Clockwise = -1, AntiClockwise = 1 };

    [SerializeField]
    private float[] m_LookRotationsZ;

    [SerializeField]
    private Direction m_InitialDirection;

    [SerializeField]
    private float m_SearchSpeed = 40f;
    
    [SerializeField]
    private CircuitObject connectedObj;

    [SerializeField]
    private float m_AlertedTimer = 10f;

    private float m_AlertTimeLeft;
    private bool m_OnAlert;

    private Vector3 initRot;

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        //Draw current in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, -transform.up);

        //Draw extents in red
        Gizmos.color = Color.red;
        foreach(float rot in m_LookRotationsZ)
            Gizmos.DrawRay(pos, Quaternion.Euler(0, 0, rot) * initRot * 5f);
    }

    private void Awake()
    {
        initRot = -transform.up;
        
        connectedObj.Deactivate();

        for(int i = 0; i < m_LookRotationsZ.Length; i++)
        {
            if(m_LookRotationsZ[i] < 0)
                m_LookRotationsZ[i] += 360;
        }
    }

    private void Update()
    {
        if(m_OnAlert && connectedObj.active)
            connectedObj.Deactivate();
        else if(!m_OnAlert && !connectedObj.active)
            connectedObj.Activate();
    }

    private IEnumerator LookAround()
    {
        m_OnAlert = true;
        m_AlertTimeLeft = m_AlertedTimer;

        int i = 0;
        float newRotZ = m_LookRotationsZ[0];
        float currRotZ = transform.rotation.eulerAngles.z;
        int direction = (int)m_InitialDirection;
        while(m_AlertTimeLeft > 0f)
        {
            if(Mathf.Abs(currRotZ - newRotZ) < 1f)
            {
                i = (i + 1) % m_LookRotationsZ.Length;
                direction *= -1;
                newRotZ = m_LookRotationsZ[i];
            }

            Rotate(ref currRotZ, direction);

            m_AlertTimeLeft -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        direction = (int)Mathf.Sign(Vector3.SignedAngle(-transform.up, initRot, transform.forward));

        while(currRotZ != initRot.z)
        {
            if(Mathf.Abs(currRotZ - newRotZ) < 1f)
            {
                transform.up = initRot;
                break;
            }
            Rotate(ref currRotZ, direction);
            yield return new WaitForFixedUpdate();
        }

        m_OnAlert = false;
        yield return null;
    }

    private void Rotate(ref float z, int dir)
    {
        z += Time.fixedDeltaTime * m_SearchSpeed * dir;
        if(z >= 360f)
            z = 0;
        else if(z < 0f)
            z += 360f;

        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
            return;

        if(!m_OnAlert)
            StartCoroutine(LookAround());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
            return;

        m_AlertTimeLeft = m_AlertedTimer;
    }
}
