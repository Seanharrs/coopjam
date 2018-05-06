using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

namespace Coop
{
  [Serializable]
  public class PlayerData
  {
    [Tooltip("Up to 4 players.")]
    public int playerIndex;
    [Tooltip("Whether this player is active")]
    public bool playerActive = false;
    
    [Space]
    [Tooltip("Reference to currently used GameObject instance.")] // TODO: Hide in inspector after testing
    public PlatformerCharacter2D playerCharacter;
    
    [Space]
    [Tooltip("Controller scriptable objects")] // TODO: Hide in inspector after testing
    public PlayerControlData controlData;
    [Tooltip("Will hold a reference to the current Gun's prefab.")] // TODO: Hide in inspector after testing
    public Gun playerGun; // TODO: This may not be necessary, depending on design - perhaps it wont hurt to use it either way.

  }

  public class CoopGameManager : MonoBehaviour
  {

    [Header("Options")]
    public bool allowKeyboard = false;

    public List<PlayerControlData> playerControlData;

    [Header("Development/Debugging")]
    [SerializeField] //TODO: Hide after we're sure this is all good.
    public List<PlayerData> playerData = new List<PlayerData> {
      new PlayerData { playerIndex = 0 },
      new PlayerData { playerIndex = 1 },
      new PlayerData { playerIndex = 2 },
      new PlayerData { playerIndex = 3 },
    };

    void Awake()
    {
      DontDestroyOnLoad(this);
    }
  }
}