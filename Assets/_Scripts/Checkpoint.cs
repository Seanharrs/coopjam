using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class Checkpoint : MonoBehaviour {

    internal bool IsVisible { get; private set; }

    [SerializeField]
    internal Transform spawnAtPoint;

    [SerializeField]
    internal ParticleSystem particleSystem;

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
      if(active)
        particleSystem.Play();
      else
        particleSystem.Stop();
    }
  }
}