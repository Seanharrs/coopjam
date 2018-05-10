using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {

  [SerializeField]
	internal string nextLevel;

  [SerializeField]
  internal UnityEvent levelCompleted;

  public void LevelComplete()
  {
    levelCompleted.Invoke();
    // TODO: Show UI first and let player click continue?
    SceneManager.LoadScene(nextLevel);
  }

}
