using System.Collections;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof (AudioClip))]
    public class AutoDoor : MonoBehaviour, IMultiSwitchStateListener
    {
        [SerializeField]
        private Vector3 m_OpenPos;

        [SerializeField]
		private Vector3 m_ClosedPos;

        [SerializeField]
        private float m_Speed = 5f;

        [SerializeField]
        private AudioClip m_OpenSound;

        private AudioSource m_AudioSource;

		private bool m_IsOpen = false;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
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
        
        public void OpenDoor()
        {
            if(m_OpenSound && m_AudioSource)
            {
              Debug.Log("Playing door open sound. [AutoDoor > " + name + "] (" + Time.time + ")");
                m_AudioSource.clip = m_OpenSound;
                m_AudioSource.loop = false;
                m_AudioSource.Play();

            } else {
              if(!m_OpenSound)
                Debug.LogWarning("Open sound not provided.");
              if(!m_AudioSource)
                Debug.LogWarning("AUdiosource not provided.");
            }
			m_IsOpen = true;
            StartCoroutine(MoveDoor(m_OpenPos));
        }

        public void CloseDoor()
        {
			m_IsOpen = false;
            StartCoroutine(MoveDoor(m_ClosedPos));
        }

		public void ToggleDoor()
		{
			if(m_IsOpen) CloseDoor();
			else OpenDoor();
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