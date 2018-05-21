using System.Collections;
using UnityEngine;

namespace Coop
{
    public class AutoDoor : MonoBehaviour, IMultiSwitchStateListener
    {
        [SerializeField]
        private Vector3 m_OpenPos;

        [SerializeField]
        private Vector3 m_ClosedPos;

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
            StartCoroutine(MoveDoor(m_ClosedPos));
        }

        public void OnSwitchStateChanged(MultiSwitch multiSwitch, SwitchState state)
        {
            if(state == SwitchState.Positive)
                OpenDoor();
            else
                CloseDoor();
        }
    }
}