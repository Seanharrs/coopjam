using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coop 
{
  public class LevelGoal : MonoBehaviour {

    List<CoopUserControl> overlappedPlayers = new List<CoopUserControl>();

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.GetComponent<CoopUserControl>())
      {
        if(AddUnique(other.GetComponent<CoopUserControl>()))
        {
          if(CoopGameManager.instance.playerData.Count() == overlappedPlayers.Count())
          {
            // Debug.Log("All players overlapping");
            FindObjectOfType<LevelManager>().LevelComplete();
          }
        }
      }
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
      if(!overlappedPlayers.Contains(controller))
      {
        overlappedPlayers.Add(controller);
        return true;
      }
      return false;
    }
  }
}