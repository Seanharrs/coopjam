using UnityEngine;
using UnityEditor;
using System;

namespace Coop
{
  [CustomEditor(typeof(CornerPlatform))]
  public class CornerPlatform_Editor : Editor
  {
    CornerPlatform m_CornerPlatform;

    SpriteRenderer m_CornerRenderer;
    SpriteRenderer m_TopRenderer;
    SpriteRenderer m_RightRenderer;

    BoxCollider2D m_CornerCollider;
    BoxCollider2D m_TopCollider;
    BoxCollider2D m_RightCollider;

    static bool m_UseStandard = false;
    static bool m_Snapping = true;

    void Awake()
    {
      // m_CornerPlatform = (CornerPlatform)target;
      
      // m_CornerRenderer = m_CornerPlatform.transform.Find("Corner").GetComponent<SpriteRenderer>();
      // m_CornerCollider = m_CornerPlatform.transform.Find("Corner").GetComponent<BoxCollider2D>();
      
      // m_TopRenderer = m_CornerPlatform.transform.Find("Top").GetComponent<SpriteRenderer>();
      // m_TopCollider = m_CornerPlatform.transform.Find("Top").GetComponent<BoxCollider2D>();
      
      // m_RightRenderer = m_CornerPlatform.transform.Find("Right").GetComponent<SpriteRenderer>();
      // m_RightCollider = m_CornerPlatform.transform.Find("Right").GetComponent<BoxCollider2D>();

    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset()
    {
      m_CornerPlatform = (CornerPlatform)target;
      
      m_CornerRenderer = m_CornerPlatform.transform.Find("Corner").GetComponent<SpriteRenderer>();
      m_CornerCollider = m_CornerPlatform.transform.Find("Corner").GetComponent<BoxCollider2D>();
      
      m_TopRenderer = m_CornerPlatform.transform.Find("Top").GetComponent<SpriteRenderer>();
      m_TopCollider = m_CornerPlatform.transform.Find("Top").GetComponent<BoxCollider2D>();
      
      m_RightRenderer = m_CornerPlatform.transform.Find("Right").GetComponent<SpriteRenderer>();
      m_RightCollider = m_CornerPlatform.transform.Find("Right").GetComponent<BoxCollider2D>();
      
      m_CornerPlatform.transform.Find("Corner").hideFlags =  m_UseStandard ? HideFlags.None : HideFlags.HideInHierarchy;
      m_CornerPlatform.transform.Find("Right").hideFlags =  m_UseStandard ? HideFlags.None : HideFlags.HideInHierarchy;
      m_CornerPlatform.transform.Find("Top").hideFlags =  m_UseStandard ? HideFlags.None : HideFlags.HideInHierarchy;

      EditorApplication.RepaintHierarchyWindow ();
      EditorApplication.DirtyHierarchyWindowSorting();
        
    }

    public void DrawCustomInspector()
    {

      Vector2 oldRightSize = m_RightRenderer.size;
      
      #region Width Buttons
      GUILayout.BeginHorizontal();
      GUILayout.Label("Size A:");

      string widthText = oldRightSize.x.ToString();
      string newWidth = GUILayout.TextField(widthText);
      if(newWidth != widthText && !String.IsNullOrEmpty(newWidth) && int.Parse(newWidth) >= 1)
        oldRightSize.x = int.Parse(newWidth);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldRightSize.x > 1)
      {
        oldRightSize.x -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldRightSize.x += 1;
      }
      GUILayout.EndHorizontal();
      #endregion

      if(oldRightSize != m_RightRenderer.size)
      {
        m_RightRenderer.size = oldRightSize;
        var offset = m_RightCollider.offset;
        offset.x = oldRightSize.x / 2;
        offset.y = oldRightSize.y / 2;
        m_RightCollider.offset = offset;
        m_RightCollider.size = oldRightSize;
      }


      Vector2 oldTopSize = m_TopRenderer.size;

      #region Height Buttons
      GUILayout.BeginHorizontal();
      GUILayout.Label("Size B:");
      string heightText = oldTopSize.y.ToString();
      string newHeight = GUILayout.TextField(heightText);
      if(newHeight != heightText && !String.IsNullOrEmpty(newHeight) && int.Parse(newHeight) >= 1)
        oldTopSize.y = int.Parse(newHeight);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldTopSize.y > 1)
      {
        oldTopSize.y -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldTopSize.y += 1;
      }
      GUILayout.EndHorizontal();
      #endregion
      
      if(oldTopSize != m_TopRenderer.size)
      {
        m_TopRenderer.size = oldTopSize;
        var offset = m_TopCollider.offset;
        offset.x = oldTopSize.x / 2;
        offset.y = oldTopSize.y / 2;
        m_TopCollider.offset = offset;
        m_TopCollider.size = oldTopSize;
      }


      #region Snap settings
      GUILayout.BeginHorizontal();
      m_Snapping = GUILayout.Toggle(m_Snapping, "Snapping:");
      GUILayout.EndHorizontal();
      #endregion

      #region Rotation
      GUILayout.BeginHorizontal();
      if(GUILayout.Button("Rotate CCW"))
      {
        var start = m_CornerPlatform.transform.rotation;
        start = Quaternion.Euler(start.eulerAngles.x, start.eulerAngles.y, start.eulerAngles.z + 90);
        m_CornerPlatform.transform.rotation = start;
      }
      if(GUILayout.Button("Rotate CW"))
      {
        var start = m_CornerPlatform.transform.rotation;
        start = Quaternion.Euler(start.eulerAngles.x, start.eulerAngles.y, start.eulerAngles.z - 90);
        m_CornerPlatform.transform.rotation = start;
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


      Handles.color = Color.blue;

      #region Bottom Extent
      Vector3 startPos = m_RightRenderer.transform.position + m_RightRenderer.size.x * m_RightRenderer.transform.right + m_RightRenderer.size.y * m_RightRenderer.transform.up / 2;
      // Vector3 newPos = Handles.PositionHandle(startPos, Quaternion.identity);
      Vector3 newPos = Handles.Slider(startPos, m_RightRenderer.transform.right, HandleUtility.GetHandleSize(startPos) * .25f, Handles.SphereHandleCap, 1f);
      if(newPos != startPos)
      {
        Vector2 oldBottomSize = m_RightRenderer.size;
        oldBottomSize.x = Vector3.Distance(newPos, m_RightRenderer.transform.position);
        if(oldBottomSize.x < 1) oldBottomSize.x = 1;
        if(m_Snapping) oldBottomSize.x = Mathf.Round(oldBottomSize.x);
        m_RightRenderer.size = oldBottomSize;
        var offset = m_RightCollider.offset;
        offset.x = oldBottomSize.x / 2;
        offset.y = oldBottomSize.y / 2;
        m_RightCollider.offset = offset;
        m_RightCollider.size = oldBottomSize;

        Repaint();

      }
      #endregion
      
      #region Middle Size
      Vector3 topStartPos = m_TopRenderer.transform.position + m_TopRenderer.size.x * m_TopRenderer.transform.right / 2 + m_TopRenderer.size.y * m_TopRenderer.transform.up;
      // Vector3 newTopPos = Handles.PositionHandle(topStartPos, Quaternion.identity);
      Vector3 newTopPos = Handles.Slider(topStartPos, m_TopRenderer.transform.up, HandleUtility.GetHandleSize(topStartPos) * .25f, Handles.SphereHandleCap, 1f);
      if(newTopPos != topStartPos)
      {
        Vector2 oldTopSize = m_TopRenderer.size;
        oldTopSize.y = Vector3.Distance(newTopPos, m_TopRenderer.transform.position);
        if(oldTopSize.y < 1) oldTopSize.y = 1;
        if(m_Snapping) oldTopSize.y = Mathf.Round(oldTopSize.y);
        m_TopRenderer.size = oldTopSize;
        var offset = m_TopCollider.offset;
        offset.x = oldTopSize.x / 2;
        offset.y = oldTopSize.y / 2;
        m_TopCollider.offset = offset;
        m_TopCollider.size = oldTopSize;
        
        Repaint();

      }
      #endregion
    }

  }
}