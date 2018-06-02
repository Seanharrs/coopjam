using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coop
{
  public class PauseMenu : MonoBehaviour {

    [SerializeField]
    private EventSystem m_EventSystem;

    [SerializeField]
    private GameObject m_StartButton;

    public void Awake()
    {
      //m_EventSystem = GetComponent<EventSystem>();
      //var allSystems = FindObjectsOfType<EventSystem>();
      //foreach (var sys in allSystems)
      //{
      //  if (sys != m_EventSystem)
      //    Destroy(sys);
      //}

      Debug.Log("Awake : " + m_EventSystem.name);
      //StartCoroutine(WaitThenSelect());
      //m_EventSystem.SetSelectedGameObject(m_StartButton);
      //m_StartButton.GetComponent<Button>().Select();

    }

    private IEnumerator WaitThenSelect()
    {
      yield return new WaitForSeconds(0.5f);
      m_EventSystem.SetSelectedGameObject(m_StartButton);
      m_StartButton.GetComponent<Button>().Select();
    }

    public void OnEnable()
    {
      m_EventSystem.firstSelectedGameObject = m_StartButton;
      m_EventSystem.gameObject.SetActive(true);
      //m_EventSystem.SetSelectedGameObject(m_StartButton);
      //m_StartButton.GetComponent<Button>().Select();
      Debug.Log("Enabled");
    }

    private void OnDisable()
    {
      m_EventSystem.gameObject.SetActive(false);
    }

    public void LoadMainMenu()
    {
      CoopGameManager.OpenLevel(0); // First level should be main menu.
      Time.timeScale = 1;
    }

    public void ResetLevel()
    {
      int scene = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
      Time.timeScale = 1;
    }

    public void QuitGame()
    {
      Application.Quit();
    }

  }
}