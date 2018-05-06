using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class PlayerSelectMenu : MonoBehaviour
  {

    [SerializeField]
    private List<GameObject> playerSelectAnchors;
    private List<PlayerSelectControl> playerSelectControls = new List<PlayerSelectControl>();
    [SerializeField]
    private List<Sprite> playerPortraits;

    private int m_NumPlayers = 2;
    public int NumPlayers
    {
      get { return m_NumPlayers; }
      set
      {
        m_NumPlayers = Mathf.Clamp(value, 2, 4); // Allow 2 to 4 players.
      }
    }
    void OnEnable()
    {
      // So hacky
      // if (m_NumPlayers == 2)
      // {
      //   playerSelectAnchors[0].SetActive(false);
      //   playerSelectAnchors[1].SetActive(true);
      //   playerSelectAnchors[2].SetActive(true);
      //   playerSelectAnchors[3].SetActive(false);
      // }
      // else if (m_NumPlayers == 3)
      // {
      //   playerSelectAnchors[0].SetActive(true);
      //   playerSelectAnchors[1].SetActive(true);
      //   playerSelectAnchors[2].SetActive(true);
      //   playerSelectAnchors[3].SetActive(false);
      // }
      // else if (m_NumPlayers == 4)
      // {
      //   playerSelectAnchors[0].SetActive(true);
      //   playerSelectAnchors[1].SetActive(true);
      //   playerSelectAnchors[2].SetActive(true);
      //   playerSelectAnchors[3].SetActive(true);
      // }

      foreach (var anchor in playerSelectAnchors)
      {
        var selectControl = anchor.GetComponentInChildren<PlayerSelectControl>();
        playerSelectControls.Add(selectControl);
        selectControl.playerIndex = playerSelectAnchors.IndexOf(anchor);
        selectControl.leftButton.onClick.AddListener(delegate { LeftButton_Click(selectControl); });
        selectControl.rightButton.onClick.AddListener(delegate { RightButton_Click(selectControl); });
        selectControl.readyButton.onClick.AddListener(delegate { ReadyButton_Click(selectControl); });
      }
    }

    void LeftButton_Click(PlayerSelectControl whichControl)
    {
      Debug.Log("Left button was clicked for player: " + (whichControl.playerIndex + 1));
    }
    void RightButton_Click(PlayerSelectControl whichControl)
    {
      Debug.Log("Right button was clicked for player: " + (whichControl.playerIndex + 1));
    }
    void ReadyButton_Click(PlayerSelectControl whichControl)
    {
      Debug.Log("Ready button was clicked for player: " + (whichControl.playerIndex + 1));
    }

  }

}