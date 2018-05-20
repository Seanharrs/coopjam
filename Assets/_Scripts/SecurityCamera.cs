using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof(CircuitObject), typeof(AudioSource))]
  public class SecurityCamera : MonoBehaviour
  {
      private enum Direction { Clockwise = -1, CounterClockwise = 1 };

      [SerializeField]
      private float[] m_LookRotationsZ;

      [SerializeField]
      private Direction m_InitialDirection;

      [SerializeField]
      private float m_SearchSpeed = 40f;
      
      [SerializeField]
      private float m_MaxAlertTime = 10f;
      private float m_AlertTimeLeft;
      private bool m_OnAlert;

      private CircuitObject m_CircuitObj;
      private AudioSource m_Audio;

      [SerializeField, Tooltip("The rotation the camera should move to when shut down.")]
      private Vector3 m_ShutdownRot;

      private Vector3 m_InitRot;
      private Vector3 m_InitRotLeft;
      private float m_CurrRotZ;

      private bool m_Initialized = false;

      private void OnDrawGizmos()
      {
          Vector3 pos = transform.position;

          //In case Awake() hasn't run yet
          if(!m_Initialized)
              InitCam();

          //Draw current in blue
          Gizmos.color = Color.blue;
          Gizmos.DrawRay(pos, -transform.right);

          //Draw extents in red
          Gizmos.color = Color.red;
          foreach(float rot in m_LookRotationsZ)
              Gizmos.DrawRay(pos, Quaternion.Euler(0, 0, rot - m_InitRot.z) * m_InitRotLeft * 5f);
      }

      private void Awake() { InitCam(); }

      private void InitCam()
      {
          m_Initialized = true;

          m_InitRot = transform.rotation.eulerAngles;
          m_InitRotLeft = -transform.right;

          m_Audio = GetComponent<AudioSource>();
          m_CircuitObj = GetComponent<CircuitObject>();

          for(int i = 0; i < m_LookRotationsZ.Length; i++)
              if(m_LookRotationsZ[i] < 0)
                  m_LookRotationsZ[i] += 360;
      }

      public void ShutDown()
      {
          StopAllCoroutines();
          transform.GetChild(0).gameObject.SetActive(false);
          GetComponent<PolygonCollider2D>().enabled = false;
          StartCoroutine(ReturnToState(m_ShutdownRot));
      }

      public void StartUp()
      {
          StopAllCoroutines();
          transform.GetChild(0).gameObject.SetActive(true);
          GetComponent<PolygonCollider2D>().enabled = true;
          StartCoroutine(ReturnToState(m_InitRot));
      }

      private IEnumerator LookAround()
      {
          m_OnAlert = true;
          m_AlertTimeLeft = m_MaxAlertTime;
          m_Audio.Play();

          if(!m_CircuitObj.active)
              m_CircuitObj.TriggerStateChange(CircuitState.Positive);

          int i = 0;
          float newRotZ = m_LookRotationsZ[0];
          m_CurrRotZ = transform.rotation.eulerAngles.z;
          int direction = (int)m_InitialDirection;

          if(Mathf.Abs(m_CurrRotZ - newRotZ) < 1f)
          {
              i = (i + 1) % m_LookRotationsZ.Length;
              newRotZ = m_LookRotationsZ[i];
          }
          
          while(m_AlertTimeLeft > 0f)
          {
              if(Mathf.Abs(m_CurrRotZ - newRotZ) < 1f)
              {
                  i = (i + 1) % m_LookRotationsZ.Length;
                  direction *= -1;
                  newRotZ = m_LookRotationsZ[i];
              }

              Rotate(direction);

              m_AlertTimeLeft -= Time.fixedDeltaTime;

              yield return new WaitForFixedUpdate();
          }
          
          yield return ReturnToState(m_InitRot);

          m_OnAlert = false;
          m_Audio.Stop();

          if(m_CircuitObj.active)
              m_CircuitObj.TriggerStateChange(CircuitState.Off);

          yield return null;
      }

      private IEnumerator ReturnToState(Vector3 rot)
      {
          float rotDiff = rot.z - transform.rotation.eulerAngles.z;
          if(rotDiff > 180f)
              rotDiff -= 360f;
          else if(rotDiff < -180f)
              rotDiff += 360f;

          int direction = (int)Mathf.Sign(rotDiff);

          while(m_CurrRotZ != rot.z)
          {
              if(Mathf.Abs(m_CurrRotZ - rot.z) < 1f)
              {
                  transform.rotation = Quaternion.Euler(rot);
                  break;
              }
              Rotate(direction);
              yield return new WaitForFixedUpdate();
          }
      }

      private void Rotate(int dir)
      {
          m_CurrRotZ += Time.fixedDeltaTime * m_SearchSpeed * dir;
          if(m_CurrRotZ >= 360f)
              m_CurrRotZ -= 360f;
          else if(m_CurrRotZ < 0f)
              m_CurrRotZ += 360f;

          transform.rotation = Quaternion.Euler(0, 0, m_CurrRotZ);
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

          m_AlertTimeLeft = m_MaxAlertTime;
      }
  }
}