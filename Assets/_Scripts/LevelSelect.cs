using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    private struct Level
    {
        public string name;
        public int buildIndex;
        public Sprite preview;
    }

    [SerializeField]
    private Text m_LevelName;

    [SerializeField]
    private Image m_LevelPreview;

    [SerializeField]
    private Level[] m_Levels;

    private int m_Index = 0;

    private void Awake()
    {
        IEnumerable<string> duplicateNames = 
            m_Levels.Where(l1 => m_Levels.Where(l2 => l1.name == l2.name || l1.buildIndex == l2.buildIndex).Count() > 1)
                    .Select(l => l.name)
                    .Distinct();

        foreach(string level in duplicateNames)
            Debug.LogError("Duplicate scene entry by name or build index: \"" + level + "\".");

        if(m_Levels.Length == 0)
            Debug.LogError("Level Select requires at least one level in the array.");

        SetLevel(m_Index);
    }

    public void LoadSelectedLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_Levels[m_Index].buildIndex);
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
