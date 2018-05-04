using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_OpenPos;

    [SerializeField]
    private Vector3 m_ClosePos;

    [SerializeField]
    private float m_Speed = 5f;

    private bool m_IsOpen = false;
    public bool isOpen
    {
        get { return m_IsOpen; }
        private set { m_IsOpen = value; }
    }

    private void FixedUpdate()
    {
        if(isOpen && transform.position != m_OpenPos)
            transform.position = Vector3.MoveTowards(transform.position, m_OpenPos, Time.fixedDeltaTime * m_Speed);
        else if(!isOpen && transform.position != m_ClosePos)
            transform.position = Vector3.MoveTowards(transform.position, m_ClosePos, Time.fixedDeltaTime * m_Speed);
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}
