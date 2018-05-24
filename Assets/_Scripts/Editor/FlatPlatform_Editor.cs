using UnityEngine;
using UnityEditor;
using System;

namespace Coop
{
  [CustomEditor(typeof(Platform))]
  public class Platform_Editor : Editor
  {
    Platform m_Platform;
    SpriteRenderer m_SpriteRenderer;
    BoxCollider2D m_BoxCollider2D;

    static bool m_UseStandard = false;
    static bool m_Snapping = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
      m_Platform = (Platform)target;
      
      m_SpriteRenderer = m_Platform.GetComponent<SpriteRenderer>();
      m_BoxCollider2D = m_Platform.GetComponent<BoxCollider2D>();

    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset()
    {
      m_Platform = (Platform)target;
      
      m_SpriteRenderer = m_Platform.GetComponent<SpriteRenderer>();
      m_BoxCollider2D = m_Platform.GetComponent<BoxCollider2D>();
      
      m_SpriteRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_BoxCollider2D.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
        
    }

    public void DrawCustomInspector()
    {
      Vector2 oldSize = m_SpriteRenderer.size;

      #region Width Buttons
      GUILayout.BeginHorizontal();
      GUILayout.Label("Width");

      string widthText = oldSize.x.ToString();
      string newWidth = GUILayout.TextField(widthText);
      if(newWidth != widthText && !String.IsNullOrEmpty(newWidth) && int.Parse(newWidth) >= 1)
        oldSize.x = int.Parse(newWidth);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldSize.x > 1)
      {
        oldSize.x -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldSize.x += 1;
      }
      GUILayout.EndHorizontal();
      #endregion

      #region Height Buttons
      GUILayout.BeginHorizontal();
      GUILayout.Label("Height");
      string heightText = oldSize.y.ToString();
      string newHeight = GUILayout.TextField(heightText);
      if(newHeight != heightText && !String.IsNullOrEmpty(newHeight) && int.Parse(newHeight) >= 1)
        oldSize.y = int.Parse(newHeight);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldSize.y > 1)
      {
        oldSize.y -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldSize.y += 1;
      }
      GUILayout.EndHorizontal();
      #endregion

      if(oldSize != m_SpriteRenderer.size)
      {
        m_SpriteRenderer.size = oldSize;
        var offset = m_BoxCollider2D.offset;
        offset.x = oldSize.x / 2;
        offset.y = oldSize.y / 2;
        m_BoxCollider2D.offset = offset;
        m_BoxCollider2D.size = oldSize;
      }


      #region Sprite Management
      GUILayout.BeginHorizontal();
      GUILayout.Label("Sprite: ", GUILayout.ExpandWidth(false));
      Sprite s = EditorGUILayout.ObjectField(m_SpriteRenderer.sprite, typeof(Sprite), false, GUILayout.ExpandWidth(true)) as Sprite;
      if(s != m_SpriteRenderer.sprite)
      {
        m_SpriteRenderer.sprite = s;
      }
      GUILayout.EndHorizontal();
      #endregion

    }

    public override void OnInspectorGUI()
    {

      if(!m_UseStandard) DrawCustomInspector();

      if(!m_UseStandard && GUILayout.Button("Use Standard Inspectors"))
      {
        m_UseStandard = true;
        Reset();
      }
      else if (m_UseStandard && GUILayout.Button("Use Custom Inspector"))
      {
        m_UseStandard = false;
        Reset();
      }
        
    }


    
    void OnSceneGUI()
    {

      #region Width Handles
      Vector3 startWidthPos = m_Platform.transform.position + m_SpriteRenderer.size.x * m_Platform.transform.right + m_SpriteRenderer.size.y * m_Platform.transform.up;
      Vector3 newWidthPos = Handles.PositionHandle(startWidthPos, Quaternion.identity);
      if(newWidthPos != startWidthPos)
      {
        Vector2 oldWidth = m_SpriteRenderer.size;
        // Width
        oldWidth.x = newWidthPos.x - m_SpriteRenderer.transform.position.x;
        if(oldWidth.x < 1) oldWidth.x = 1;
        if(m_Snapping) oldWidth.x = Mathf.Round(oldWidth.x);
        // Height
        oldWidth.y = newWidthPos.y - m_SpriteRenderer.transform.position.y;
        if(oldWidth.y < 1) oldWidth.y = 1;
        if(m_Snapping) oldWidth.y = Mathf.Round(oldWidth.y);

        m_SpriteRenderer.size = oldWidth;
        var offset = m_BoxCollider2D.offset;
        offset.x = oldWidth.x / 2;
        offset.y = oldWidth.y / 2;
        m_BoxCollider2D.offset = offset;
        m_BoxCollider2D.size = oldWidth;

        Repaint();
      }
      #endregion

    }
  }
}