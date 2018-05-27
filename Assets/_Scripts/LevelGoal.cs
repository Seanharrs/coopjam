using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof(AudioClip))]
  public class LevelGoal : MonoBehaviour
  {
    [SerializeField]
    private AudioClip m_OpenSound;

    private AudioSource m_AudioSource;
    List<CoopUserControl> overlappedPlayers = new List<CoopUserControl>();

    void Awake()
    {
      m_AudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.GetComponent<CoopUserControl>())
      {
        if (AddUnique(other.GetComponent<CoopUserControl>()))
        {
          if (CoopGameManager.instance.playerData.Count() == overlappedPlayers.Count())
          {
            // Debug.Log("All players overlapping");
            if (m_OpenSound && m_AudioSource)
            {
              Debug.Log("Playing door open sound. [LevelGoal] (" + Time.time + ")");
              m_AudioSource.clip = m_OpenSound;
              m_AudioSource.loop = false;
              m_AudioSource.Play();

            }
            else
            {
              if (!m_OpenSound)
                Debug.LogWarning("Open sound not provided.");
              if (!m_AudioSource)
                Debug.LogWarning("AUdiosource not provided.");
            }
            
            StartCoroutine(WaitThenAct(5f, () => 
                          {
                            FindObjectOfType<LevelManager>().LevelComplete();
                          }));

          }
        }
      }
    }

    IEnumerator WaitThenAct(float waitTime, Action act)
    {
      yield return new WaitForSeconds(waitTime);
      act();
    }

    void OnTriggerExit2D(Collider2D other)
    {
      if (other.GetComponent<CoopUserControl>())
      {
        overlappedPlayers.Remove(other.GetComponent<CoopUserControl>());
      }
    }

    bool AddUnique(CoopUserControl controller)
    {
      if (!overlappedPlayers.Contains(controller))
      {
        overlappedPlayers.Add(controller);
        return true;
      }

      return false;
    }
  }
}