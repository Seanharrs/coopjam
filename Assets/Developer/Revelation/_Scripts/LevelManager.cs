using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Coop {
  public class LevelManager : MonoBehaviour {

    [SerializeField]
    [Tooltip("Which scene should be loaded when level is complete?")]
    internal string nextLevel;

    [SerializeField]
    internal UnityEvent levelCompleted;

    public void LevelComplete()
    {
      levelCompleted.Invoke(); // Inform subscribers that level is complete.

      CoopGameManager.OpenLevel(nextLevel); // TODO: Show UI  and let player click continue before loading next level?
    }

  }
}