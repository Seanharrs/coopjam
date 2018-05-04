using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private AutoDoor connectedDoor;
    private int m_EntitiesInView = 0;

    private void Awake()
    {
        connectedDoor = FindObjectOfType<AutoDoor>();
        connectedDoor.OpenDoor();
    }

    private void Update()
    {
        if(m_EntitiesInView > 0 && connectedDoor.isOpen)
            connectedDoor.CloseDoor();
        else if(m_EntitiesInView == 0 && !connectedDoor.isOpen)
            connectedDoor.OpenDoor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ++m_EntitiesInView;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        --m_EntitiesInView;
    }
}
