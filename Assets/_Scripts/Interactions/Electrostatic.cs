using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
	public class Electrostatic : MonoBehaviour
	{
		[SerializeField, Tooltip("// TODO:")]
		private ProjectileEvent OnStartCharge = new ProjectileEvent();
		[SerializeField, Tooltip("// TODO:")]
		private ProjectileEvent OnStopCharge = new ProjectileEvent();

		[SerializeField]
		[Tooltip("Electrostatic disruption (either weapon mode) works on this object. Defaults to true.")]
		internal bool canInterrupt = true;

		private ParticleSystem m_PS;

		private void Awake() { m_PS = GetComponentInChildren<ParticleSystem>(); }

		internal bool StartCharge(Gun sourceGun, FiringState weapType)
		{
			if(canInterrupt && OnStartCharge.GetPersistentEventCount() > 0)
			{
				if(m_PS && !m_PS.isPlaying)
					m_PS.Play();
				OnStartCharge.Invoke(sourceGun, weapType);
			}
			return canInterrupt;
		}

		internal bool StopCharge(Gun sourceGun, FiringState weapType)
		{
			if(canInterrupt && OnStopCharge.GetPersistentEventCount() > 0)
			{
				if(m_PS && m_PS.isPlaying)
					m_PS.Stop();
				OnStopCharge.Invoke(sourceGun, weapType);
			}
			return canInterrupt;
		}
	}
}
