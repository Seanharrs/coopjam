using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public void StartGame()
	{
		Coop.CoopGameManager.SelectPlayersThenOpen("Obstacle_Tutorial");
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
