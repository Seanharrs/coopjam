using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof (AudioSource))]
  public class Checkpoint : MonoBehaviour {

    internal bool IsVisible { get; private set; }

    [SerializeField]
    internal Transform spawnAtPoint;

    [SerializeField]
    internal ParticleSystem m_ParticleSystem;

    internal AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip m_Sound_TurnOn;
    [SerializeField]
    private AudioClip m_Sound_ConstantOn;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
      Debug.Log("Setting last checkpoint.");
      // TODO: Visual and sound effects.
      FindObjectOfType<LevelManager>().ActiveCheckpoint = this;

    }

    /// <summary>
    /// OnBecameVisible is called when the renderer became visible by any camera.
    /// </summary>
    void OnBecameVisible()
    {
        IsVisible = true;
    }

    /// <summary>
    /// OnBecameInvisible is called when the renderer is no longer visible by any camera.
    /// </summary>
    void OnBecameInvisible()
    {
        IsVisible = false;
    }

    internal void SetActive(bool active)
    {
      if(!m_AudioSource)
        m_AudioSource = GetComponent<AudioSource>();

      if(active)
      {
        m_ParticleSystem.Play();
        m_AudioSource.PlayOneShot(m_Sound_TurnOn);
        m_AudioSource.clip = m_Sound_ConstantOn;
        m_AudioSource.loop = true;
        m_AudioSource.Play();
      }
      else
      {
        m_ParticleSystem.Stop();
        m_AudioSource.Stop();
      }
    }

  }
}