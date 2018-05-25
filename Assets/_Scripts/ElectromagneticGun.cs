using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class ElectromagneticGun : Gun {

    [SerializeField]
    [Tooltip("How far in Unity units that the EM field can affect targets")]
    private float m_EmDistance;
    [SerializeField]
    [Tooltip("How powerful is the EM field?")]
    private int m_ForceAmount;
    [SerializeField]
    [Tooltip("Primary weapon particle system prefab.")]
    private ParticleSystem primaryParticleSystem;
    [SerializeField]
    [Tooltip("Secondary weapon particle system prefab.")]
    private ParticleSystem secondaryParticleSystem;

    private List<Electromagnetic> m_MagnetizedObjects = new List<Electromagnetic>();
    private List<Electrostatic> m_InterruptedObjects = new List<Electrostatic>();
    private FiringState m_CurrentFiringState = FiringState.None;

    bool m_IsFiring = false;

    // Allocate up to 10 hit results in case other non-electromagnetic things are in the way. If we have more than 10, it will fail, but this seems reasonable.
    RaycastHit2D[] m_Results = new RaycastHit2D[10];
    List<GameObject> m_ResultObjects = new List<GameObject>();
    ContactFilter2D m_Filter = new ContactFilter2D() { useTriggers = true };
    int hits = 0;

    private int PopulateHitResults()
    {
      m_Results = new RaycastHit2D[10];
      var hits = Physics2D.Raycast(AmmoSpawnLocation.position, AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x), m_Filter, m_Results, m_EmDistance);
      m_ResultObjects.Clear();
      for(var i = 0; i < hits; i++)
      {
        var hit = m_Results[i];
        if(!hit) Debug.Log("No hit?");
        else if(!hit.collider) Debug.Log("No Collider?");
        else if(!hit.collider.gameObject) Debug.Log ("No GameObject.");
        else m_ResultObjects.Add(hit.collider.gameObject);
      }
      return hits;
    }

    public override Projectile Fire(FiringState weapType, Vector2? direction = null, Vector2? target = null)
    {
      if (m_CurrentFiringState == FiringState.Primary || m_CurrentFiringState == FiringState.Secondary) return null;
      
      ParticleSystem ps1; 
      ParticleSystem.MainModule main1;
      ParticleSystem ps2;
      ParticleSystem.MainModule main2;

      var owningPlayer = GetComponentInParent<CoopCharacter2D>().transform;

      switch(weapType)
      {
        case FiringState.Primary:
          ps1 = Instantiate(primaryParticleSystem, AmmoSpawnLocation.position, Quaternion.identity);
          main1 = ps1.main;
          main1.scalingMode = ParticleSystemScalingMode.Hierarchy;
          ps1.transform.SetParent(AmmoSpawnLocation);
          ps1.transform.localScale = ps1.transform.localScale * (Mathf.Sign(owningPlayer.localScale.x) * m_EmDistance / 7.5f);
          
          ps2 = ps1.GetComponentsInChildren<ParticleSystem>()[1];
          main2 = ps2.main;
          main2.scalingMode = ParticleSystemScalingMode.Hierarchy;
          //ps2.transform.SetParent(ps1.transform);

          Debug.Log("Initiating sound: " + m_SecondaryAmmoFireSound.name);
          m_AudioSource.clip = m_PrimaryAmmoFireSound;
          m_AudioSource.loop = true;
          m_AudioSource.Play();
          break;
        case FiringState.Secondary:
          ps1 = Instantiate(secondaryParticleSystem, AmmoSpawnLocation.position, Quaternion.identity);
          main1 = ps1.main;
          main1.scalingMode = ParticleSystemScalingMode.Hierarchy;
          ps1.transform.SetParent(AmmoSpawnLocation);
          ps1.transform.localScale = ps1.transform.localScale * (Mathf.Sign(owningPlayer.localScale.x) * m_EmDistance / 7.5f);
          
          ps2 = ps1.GetComponentsInChildren<ParticleSystem>()[1];
          main2 = ps2.main;
          main2.scalingMode = ParticleSystemScalingMode.Hierarchy;
          //ps2.transform.SetParent(ps1.transform);

          Debug.Log("Initiating sound: " + m_SecondaryAmmoFireSound.name);
          m_AudioSource.clip = m_SecondaryAmmoFireSound;
          m_AudioSource.loop = true;
          m_AudioSource.Play();
          break;
      }

      m_CurrentFiringState = weapType;
      
      // Debug.Log("Firing EM gun.");
      m_IsFiring = true;

      GetInitialEMObjects(weapType);

      // EM gun does not generate projectiles.
      return null;
    }

    private void GetInitialEMObjects(FiringState weapType)
    {
      m_MagnetizedObjects.Clear();
      m_InterruptedObjects.Clear();
      hits = PopulateHitResults();
      if (hits > 0)
      {
        for (var i = 0; i < hits; i++)
        {
          var emComponent = m_Results[i].collider.GetComponent<Electromagnetic>();
          if (emComponent != null)
          {
            AddElectromagnetic(weapType, emComponent);
          }

          var staticComponent = m_Results[i].collider.GetComponent<Electrostatic>();
          if (staticComponent != null && staticComponent.canInterrupt)
          {
            AddElectroStatic(staticComponent);
          }
        }
      }
    }

    private void AddElectroStatic(Electrostatic staticComponent)
    {
      // TODO: Should particle effect be different for electrostatic?
      staticComponent.StartCharge(this, m_CurrentFiringState);
      m_InterruptedObjects.Add(staticComponent);
    }

    private void AddElectromagnetic(FiringState weapType, Electromagnetic emComponent)
    {
      var rb = emComponent.GetComponent<Rigidbody2D>();
      if (rb)
        rb.bodyType = RigidbodyType2D.Dynamic;
      if (weapType == FiringState.Primary && emComponent.canAttract)
      {
        if (emComponent.StartPull(this, m_CurrentFiringState))
          m_MagnetizedObjects.Add(emComponent);
      }
      else if (weapType == FiringState.Secondary && emComponent.canRepel)
      {
        if (emComponent.StartPush(this, m_CurrentFiringState))
          m_MagnetizedObjects.Add(emComponent);
      }
    }

    public override void StopFiring()
    {

      m_AudioSource.Stop();

      var particleSystem = AmmoSpawnLocation.GetChild(0);
      if(particleSystem)
      {
        Destroy(particleSystem.gameObject);
      }

      // Debug.Log("Stopped firing EM gun.");
      m_IsFiring = false;
      foreach (var obj in m_MagnetizedObjects)
      {
        StopMagneticEffect(obj);
      }
      m_MagnetizedObjects.Clear();

      foreach(var obj in m_InterruptedObjects)
      {
        StopInterruptEffect(obj);
      }
      m_InterruptedObjects.Clear();

      m_CurrentFiringState = FiringState.None;
    }

    private void StopInterruptEffect(Electrostatic obj)
    {
      obj.StopCharge(this, m_CurrentFiringState);
    }

    private void StopMagneticEffect(Electromagnetic obj)
    {
      obj.StopPull(this, m_CurrentFiringState);
      obj.StopPush(this, m_CurrentFiringState);
      Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
      if (!rb) return;

      // TODO: Is this okay? Do we want to dampen the movement instead of stopping it 100%?
      rb.velocity = Vector2.zero;
      rb.angularVelocity = 0;
    }

    private void Update()
    {
      PopulateHitResults();

      if(m_CurrentFiringState != FiringState.None)
      {
        foreach(var hit in m_Results)
        {
          var c = hit.collider;
          if(!c) continue;
          var em = c.GetComponent<Electromagnetic>();
          if(em)
          {
            if(!m_MagnetizedObjects.Contains(em))
            {
              AddElectromagnetic(m_CurrentFiringState, em);
            }
          }

          var es = c.GetComponent<Electrostatic>();
          if(es)
          {
            if(!m_InterruptedObjects.Contains(es))
            {
              AddElectroStatic(es);
            }
          }
        }

        for(var i = m_MagnetizedObjects.Count; i > 0; i--)
        {
          var obj = m_MagnetizedObjects[i-1];

          if(!m_ResultObjects.Contains(obj.gameObject)) { 
            StopMagneticEffect(obj);
            m_MagnetizedObjects.Remove(obj); 
          }
          var rb = obj.GetComponent<Rigidbody2D>();
          if(!rb) return;
          var direction = AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x);
          rb.AddForce(direction * (m_CurrentFiringState == FiringState.Primary ? -m_ForceAmount : m_ForceAmount));
        }

        for(var i = m_InterruptedObjects.Count; i > 0; i--)
        {
          var obj = m_InterruptedObjects[i -1];
          if(!m_ResultObjects.Contains(obj.gameObject)) { 
            StopInterruptEffect(obj);
            m_InterruptedObjects.Remove(obj); 
          }
        }
      }
    }

    private void OnDrawGizmos()
    {
      Gizmos.DrawLine(AmmoSpawnLocation.position, AmmoSpawnLocation.position + (AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x) * m_EmDistance));
    }
  }
}