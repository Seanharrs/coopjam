using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircuitObject))]
public class AutoDoor : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_OpenPos;

    [SerializeField]
    private Vector3 m_ClosePos;

    [SerializeField]
    private float m_Speed = 5f;
    
    private CircuitObject m_Circuit;
    
    private void Awake()
    {
        m_Circuit = GetComponent<CircuitObject>();
        m_Circuit.OnActivate(OpenDoor);
        m_Circuit.OnDeactivate(CloseDoor);
    }

    private IEnumerator MoveDoor(Vector3 newPos)
    {
        while(transform.position != newPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.fixedDeltaTime * m_Speed);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    private void OpenDoor()
    {
        StartCoroutine(MoveDoor(m_OpenPos));
    }

    private void CloseDoor()
    {
        StartCoroutine(MoveDoor(m_ClosePos));
    }
}
