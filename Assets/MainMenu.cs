using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
  [Tooltip("Which level to open if the 'Play' button is pressed.")]
  [SerializeField] string m_NextLevel = "Obstacle_Tutorial";

  void Awake()
  {
      m_NextLevel = !string.IsNullOrEmpty(Coop.CoopGameManager.nextLevelOverride) 
                      ? Coop.CoopGameManager.nextLevelOverride 
                      : m_NextLevel;
                      
      Coop.CoopGameManager.nextLevelOverride = null;
  }

	public void StartGame()
	{
		Coop.CoopGameManager.SelectPlayersThenOpen(m_NextLevel);
	}

	public void SelectLevel()
	{
		Coop.CoopGameManager.OpenLevel("Level_Select");
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
