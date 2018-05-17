using System.Collections;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_OpenPos;

    [SerializeField]
    private Vector3 m_ClosePos;

    [SerializeField]
    private float m_Speed = 5f;

    private IEnumerator MoveDoor(Vector3 newPos)
    {
        while(transform.position != newPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.fixedDeltaTime * m_Speed);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    
    public void OpenDoor()
    {
        StartCoroutine(MoveDoor(m_OpenPos));
    }

    public void CloseDoor()
    {
        StartCoroutine(MoveDoor(m_ClosePos));
    }
}
