using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    private class Level
    {
        public string name;
        public Sprite preview;
    }

    [SerializeField]
    private Text m_LevelName;

    [SerializeField]
    private Image m_LevelPreview;

    [SerializeField]
    private Level[] m_Levels;

    private int m_Index;

    private void Awake()
    {
        var lvls = m_Levels.Select(l1 => new { l1.name, count = m_Levels.Where(l2 => l2.name == l1.name).Count() });
        foreach(var lvl in lvls.Where(l => l.count > 1))
            Debug.LogError("Level \"" + lvl.name + "\" cannot appear in the Levels array more than once.");

        m_Index = 0;
    }

    public void LoadSelectedLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_Levels[m_Index].name);
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
        m_LevelName.text = m_Levels[m_Index].name;
        m_LevelPreview.sprite = m_Levels[m_Index].preview;
    }
}
