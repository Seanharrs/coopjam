using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coop 
{
  public class LevelGoal : MonoBehaviour {

    List<Platformer2DUserControl> overlappedPlayers = new List<Platformer2DUserControl>();

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.GetComponent<Platformer2DUserControl>())
      {
        if(AddUnique(other.GetComponent<Platformer2DUserControl>()))
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
      if (other.GetComponent<Platformer2DUserControl>())
      {
        overlappedPlayers.Remove(other.GetComponent<Platformer2DUserControl>());
      }
    }

    bool AddUnique(Platformer2DUserControl controller)
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