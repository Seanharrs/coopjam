using System.Collections;
using UnityEngine;

namespace Coop
{
	[RequireComponent(typeof(BoxCollider2D), typeof(CircuitObject), typeof(AudioSource))]
	public class NonDeadlyLaser : MonoBehaviour
	{
		private CircuitObject m_Circuit;

		[SerializeField]
		private float m_MaxAlertTime = 1f;
		private float m_AlertTimeLeft;

		private void Awake() { m_Circuit = GetComponent<CircuitObject>(); }

		public void TurnOn()
		{
			GetComponent<BoxCollider2D>().enabled = true;
			GetComponent<SpriteRenderer>().enabled = true;
		}

		public void TurnOff()
		{
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<SpriteRenderer>().enabled = false;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.GetComponent<Projectile>())
				return;

			if(m_AlertTimeLeft <= 0)
				StartCoroutine(TriggerLaser());
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if(collision.GetComponent<Projectile>())
				return;

			m_AlertTimeLeft = m_MaxAlertTime;
		}

		private IEnumerator TriggerLaser()
		{
			m_AlertTimeLeft = m_MaxAlertTime;

			if(!m_Circuit.active)
				m_Circuit.TriggerStateChange(CircuitState.Positive);

			while(m_AlertTimeLeft > 0)
			{
				m_AlertTimeLeft -= Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			if(m_Circuit.active)
				m_Circuit.TriggerStateChange(CircuitState.Off);

			yield return null;
		}
	}
}
