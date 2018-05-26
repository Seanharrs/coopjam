using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class MainMenu : MonoBehaviour
  {
    [Tooltip("Which level to open if the 'Play' button is pressed.")]
    [SerializeField] string m_NextLevel = "Obstacle_Tutorial";
    [SerializeField] private AudioClip m_MainMenuMusic;

    public AudioClip m_SFX_MenuNavigate;
    public AudioClip m_SFX_MenuConfirm;

    void Awake()
    {
        m_NextLevel = !string.IsNullOrEmpty(Coop.CoopGameManager.nextLevelOverride) 
                        ? Coop.CoopGameManager.nextLevelOverride 
                        : m_NextLevel;
                        
        CoopGameManager.nextLevelOverride = null;

    }

    void Start()
    {
        var snap = CoopGameManager.instance.m_AudioMixer.FindSnapshot("MainMenu");
        snap.TransitionTo(1.5f);
        
        var source = CoopGameManager.instance.musicAudioSource;
        source.clip = m_MainMenuMusic;
        source.loop = true;
        source.Play();
    }

    public void PlayNavigateSound()
    {
      CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_SFX_MenuNavigate);
    }
    public void PlayConfirmSound()
    {
      CoopGameManager.instance.ambientAudioSource.PlayOneShot(m_SFX_MenuConfirm);
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
}