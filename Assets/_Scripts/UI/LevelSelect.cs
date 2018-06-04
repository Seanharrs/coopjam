using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Coop
{
	public class LevelSelect : MonoBehaviour
	{
		[System.Serializable]
		private struct Level
		{
			public string name;
			//public int buildIndex;
			public Sprite preview;
		}

		[SerializeField]
		private AudioClip m_MenuNavigateSFX;

		[SerializeField]
		private AudioClip m_MenuConfirmSFX;

		[SerializeField]
		private Text m_LevelName;

		[SerializeField]
		private Image m_LevelPreview;

		[SerializeField]
		private Level[] m_Levels;

		private int m_Index = 0;
		private float m_SpamDelay = 0f;

		private void Awake()
		{
			IEnumerable<string> duplicateNames =
				m_Levels.Where(l1 => m_Levels.Where(l2 => l1.name == l2.name /*|| l1.buildIndex == l2.buildIndex*/).Count() > 1)
						.Select(l => l.name)
						.Distinct();

			foreach(string level in duplicateNames)
				Debug.LogError("Duplicate scene entry by name or build index: \"" + level + "\".");

			if(m_Levels.Length == 0)
				Debug.LogError("Level Select requires at least one level in the array.");

			SetLevel(m_Index);
		}

		private void Update()
		{
			if(m_SpamDelay > 0f)
			{ 
				m_SpamDelay -= Time.deltaTime;
				return;
			}

			if(MoveLeft())
			{
				m_SpamDelay = 0.5f;
				PreviousLevel();
			}
			else if(MoveRight())
			{
				m_SpamDelay = 0.5f;
				NextLevel();
			}
			else if(Confirm())
			{
				LoadSelectedLevel();
			}
		}

		private bool MoveLeft()
		{
			return Input.GetAxis("Horizontal_KB") < -Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P1") < -Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P2") < -Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P3") < -Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P4") < -Mathf.Epsilon;
		}

		private bool MoveRight()
		{
			return Input.GetAxis("Horizontal_KB") > Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P1") > Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P2") > Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P3") > Mathf.Epsilon
				|| Input.GetAxis("Horizontal_P4") > Mathf.Epsilon;
		}

		private bool Confirm()
		{
			return Input.GetAxis("Submit_KB") > Mathf.Epsilon
				|| Input.GetAxis("Submit_P1") > Mathf.Epsilon
				|| Input.GetAxis("Submit_P2") > Mathf.Epsilon
				|| Input.GetAxis("Submit_P3") > Mathf.Epsilon
				|| Input.GetAxis("Submit_P4") > Mathf.Epsilon;
		}

		public void PlayNavigateSound()
		{
			CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_MenuNavigateSFX);
		}

		public void PlayConfirmSound()
		{
			CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_MenuConfirmSFX);
		}

		public void LoadSelectedLevel()
		{
			CoopGameManager.SelectPlayersThenOpen(m_Levels[m_Index].name);
		}

		public void NextLevel()
		{
			SetLevel(m_Index + 1);
		}

		public void PreviousLevel()
		{
			SetLevel(m_Index - 1);
		}

		private void SetLevel(int newIndex)
		{
			if(newIndex < 0)
				newIndex = m_Levels.Length - 1;
			else if(newIndex >= m_Levels.Length)
				newIndex = 0;

			m_Index = newIndex;
			m_LevelName.text = m_Levels[m_Index].name.Replace('_', ' ');
			m_LevelPreview.sprite = m_Levels[m_Index].preview;
		}
	}
}
