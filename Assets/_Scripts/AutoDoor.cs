using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_OpenPos;

    [SerializeField]
    private Vector3 m_ClosePos;

    private bool m_IsOpen = false;
    public bool isOpen
    {
        get { return m_IsOpen; }
        private set { m_IsOpen = value; }
    }

    private void FixedUpdate()
    {
        Debug.Log(isOpen);
        if(isOpen && transform.position != m_OpenPos)
            transform.position = Vector3.MoveTowards(transform.position, m_OpenPos, Time.fixedDeltaTime);
        else if(!isOpen && transform.position != m_ClosePos)
            transform.position = Vector3.MoveTowards(transform.position, m_ClosePos, Time.fixedDeltaTime);
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
