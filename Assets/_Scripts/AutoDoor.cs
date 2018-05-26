using System.Collections;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof (AudioClip))]
    public class AutoDoor : MonoBehaviour, IMultiSwitchStateListener
    {
        [SerializeField]
        public Vector3 m_OpenPos;

        [SerializeField]
        public Vector3 m_ClosedPos;

        [SerializeField]
        private float m_Speed = 5f;

        [SerializeField]
        private AudioClip m_OpenSound;

        private AudioSource m_AudioSource;

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
            if(m_OpenSound && m_AudioSource)
            {
                m_AudioSource.PlayOneShot(m_OpenSound);
            }
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