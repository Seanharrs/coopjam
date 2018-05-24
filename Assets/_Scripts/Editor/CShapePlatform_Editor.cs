using UnityEngine;
using UnityEditor;
using System;

namespace Coop
{
  [CustomEditor(typeof(CShapePlatform))]
  public class CShapePlatform_Editor : Editor
  {
    CShapePlatform m_CornerPlatform;

    Transform m_BottomCornerGO;
    Transform m_TopCornerGO;
    Transform m_MiddleExtentGO;
    Transform m_TopExtentGO;
    Transform m_BottomExtentGO;

    SpriteRenderer m_BottomCornerRenderer;
    SpriteRenderer m_TopCornerRenderer;
    SpriteRenderer m_MiddleExtentRenderer;
    SpriteRenderer m_TopExtentRenderer;
    SpriteRenderer m_BottomExtentRenderer;

    BoxCollider2D m_BottomCornerCollider;
    BoxCollider2D m_TopCornerCollider;
    BoxCollider2D m_MiddleExtentCollider;
    BoxCollider2D m_TopExtentCollider;
    BoxCollider2D m_BottomExtentCollider;
    

    static bool m_UseStandard = false;
    static bool m_Snapping = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
    }
    
    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset()
    {
      m_CornerPlatform = (CShapePlatform)target;

      m_BottomCornerGO = m_CornerPlatform.transform.Find("Bottom-Corner");
      m_TopCornerGO = m_CornerPlatform.transform.Find("Top-Corner");
      m_MiddleExtentGO = m_CornerPlatform.transform.Find("Middle");
      m_TopExtentGO = m_CornerPlatform.transform.Find("Top-Right");
      m_BottomExtentGO = m_CornerPlatform.transform.Find("Bottom-Right");
      
      m_BottomCornerRenderer = m_BottomCornerGO.GetComponent<SpriteRenderer>();
      m_TopCornerRenderer = m_TopCornerGO.GetComponent<SpriteRenderer>();
      m_MiddleExtentRenderer = m_MiddleExtentGO.GetComponent<SpriteRenderer>();
      m_TopExtentRenderer = m_TopExtentGO.GetComponent<SpriteRenderer>();
      m_BottomExtentRenderer = m_BottomExtentGO.GetComponent<SpriteRenderer>();

      m_BottomCornerCollider = m_BottomCornerGO.GetComponent<BoxCollider2D>();
      m_TopCornerCollider = m_TopCornerGO.GetComponent<BoxCollider2D>();
      m_MiddleExtentCollider = m_MiddleExtentGO.GetComponent<BoxCollider2D>();
      m_TopExtentCollider = m_TopExtentGO.GetComponent<BoxCollider2D>();
      m_BottomExtentCollider = m_BottomExtentGO.GetComponent<BoxCollider2D>();

      m_BottomCornerGO.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
      m_TopCornerGO.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
      m_MiddleExtentGO.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
      m_TopExtentGO.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
      m_BottomExtentGO.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;

      m_BottomCornerRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_TopCornerRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_MiddleExtentRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_TopExtentRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_BottomExtentRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_BottomCornerCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_TopCornerCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_MiddleExtentCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_TopExtentCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
      m_BottomExtentCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;


      EditorApplication.RepaintHierarchyWindow ();
      EditorApplication.DirtyHierarchyWindowSorting();
        
    }

    public void DrawCustomInspector()
    {

      #region Top Size Buttons
      
      Vector2 oldTopSize = m_TopExtentRenderer.size;
      
      GUILayout.BeginHorizontal();
      GUILayout.Label("End Size A:");

      string widthText = oldTopSize.x.ToString();
      string newWidth = GUILayout.TextField(widthText);
      if(newWidth != widthText && !String.IsNullOrEmpty(newWidth) && int.Parse(newWidth) >= 1)
        oldTopSize.x = int.Parse(newWidth);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldTopSize.x > 1)
      {
        oldTopSize.x -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldTopSize.x += 1;
      }
      GUILayout.EndHorizontal();

      if(oldTopSize != m_TopExtentRenderer.size)
      {
        m_TopExtentRenderer.size = oldTopSize;
        var offset = m_TopExtentCollider.offset;
        offset.x = oldTopSize.x / 2;
        offset.y = oldTopSize.y / 2;
        m_TopExtentCollider.offset = offset;
        m_TopExtentCollider.size = oldTopSize;
      }
      #endregion

      #region Bottom Size Buttons
      
      Vector2 oldBottomSize = m_BottomExtentRenderer.size;
      
      GUILayout.BeginHorizontal();
      GUILayout.Label("End Size B:");

      string bottomWidthText = oldBottomSize.x.ToString();
      string newBottomWidth = GUILayout.TextField(bottomWidthText);
      if(newBottomWidth != bottomWidthText && !String.IsNullOrEmpty(newBottomWidth) && int.Parse(newBottomWidth) >= 1)
        oldBottomSize.x = int.Parse(newBottomWidth);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldBottomSize.x > 1)
      {
        oldBottomSize.x -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldBottomSize.x += 1;
      }
      GUILayout.EndHorizontal();

      if(oldBottomSize != m_BottomExtentRenderer.size)
      {
        m_BottomExtentRenderer.size = oldBottomSize;
        var offset = m_BottomExtentCollider.offset;
        offset.x = oldBottomSize.x / 2;
        offset.y = oldBottomSize.y / 2;
        m_BottomExtentCollider.offset = offset;
        m_BottomExtentCollider.size = oldBottomSize;
      }
      #endregion


      #region Middle Size Buttons
      
      Vector2 oldMiddleSize = m_MiddleExtentRenderer.size;
      
      GUILayout.BeginHorizontal();
      GUILayout.Label("Middle Size:");

      string middleWidthText = oldMiddleSize.y.ToString();
      string newMiddleWidth = GUILayout.TextField(middleWidthText);
      if(newMiddleWidth != middleWidthText && !String.IsNullOrEmpty(newMiddleWidth) && int.Parse(newMiddleWidth) >= 1)
        oldMiddleSize.y = int.Parse(newMiddleWidth);

      if(GUILayout.Button("-", GUILayout.Width(20)) && oldMiddleSize.y > 1)
      {
        oldMiddleSize.y -= 1;
      }
      if(GUILayout.Button("+", GUILayout.Width(20)))
      {
        oldMiddleSize.y += 1;
      }
      GUILayout.EndHorizontal();

      if(oldMiddleSize != m_MiddleExtentRenderer.size)
      {
        m_MiddleExtentRenderer.size = oldMiddleSize;
        var offset = m_MiddleExtentCollider.offset;
        offset.x = oldMiddleSize.x / 2;
        offset.y = oldMiddleSize.y / 2;
        m_MiddleExtentCollider.offset = offset;
        m_MiddleExtentCollider.size = oldMiddleSize;
        
        var oldTopCornerPos = m_TopCornerCollider.transform.localPosition;
        oldTopCornerPos.y = oldMiddleSize.y + 1;
        m_TopCornerCollider.transform.localPosition = oldTopCornerPos;
        
        var oldTopExtentPos = m_TopExtentCollider.transform.localPosition;
        oldTopExtentPos.y = oldMiddleSize.y + 1;
        m_TopExtentCollider.transform.localPosition = oldTopExtentPos;

      }
      #endregion

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

      #region Bottom Extent
      Vector3 startPos = m_BottomExtentGO.transform.position + m_BottomExtentRenderer.size.x * m_BottomExtentGO.transform.right + m_BottomExtentRenderer.size.y * m_BottomExtentGO.transform.up / 2;
      Vector3 newPos = Handles.PositionHandle(startPos, Quaternion.identity);
      if(newPos != startPos)
      {
        Vector2 oldBottomSize = m_BottomExtentRenderer.size;
        oldBottomSize.x = Vector3.Distance(newPos, m_BottomExtentRenderer.transform.position);
        if(oldBottomSize.x < 1) oldBottomSize.x = 1;
        if(m_Snapping) oldBottomSize.x = Mathf.Round(oldBottomSize.x);
        m_BottomExtentRenderer.size = oldBottomSize;
        var offset = m_BottomExtentCollider.offset;
        offset.x = oldBottomSize.x / 2;
        offset.y = oldBottomSize.y / 2;
        m_BottomExtentCollider.offset = offset;
        m_BottomExtentCollider.size = oldBottomSize;

        Repaint();
      }
      #endregion

      #region Top Extent
      Vector3 topStartPos = m_TopExtentGO.transform.position + m_TopExtentRenderer.size.x * m_TopExtentGO.transform.right + m_TopExtentRenderer.size.y * m_TopExtentGO.transform.up / 2;
      Vector3 newTopPos = Handles.PositionHandle(topStartPos, Quaternion.LookRotation(m_TopExtentGO.transform.forward));
      if(newTopPos != topStartPos)
      {
        Vector2 oldTopSize = m_TopExtentRenderer.size;
        oldTopSize.x = Vector3.Distance(newTopPos, m_TopExtentRenderer.transform.position);
        if(oldTopSize.x < 1) oldTopSize.x = 1;
        if(m_Snapping) oldTopSize.x = Mathf.Round(oldTopSize.x);
        m_TopExtentRenderer.size = oldTopSize;
        var offset = m_TopExtentCollider.offset;
        offset.x = oldTopSize.x / 2;
        offset.y = oldTopSize.y / 2;
        m_TopExtentCollider.offset = offset;
        m_TopExtentCollider.size = oldTopSize;

        Repaint();
      }
      #endregion

      
      #region Middle Size
      Vector3 middleStartPos = m_MiddleExtentGO.transform.position + m_MiddleExtentRenderer.size.x * m_MiddleExtentGO.transform.right / 2 + (m_MiddleExtentRenderer.size.y + 1) * m_MiddleExtentGO.transform.up;
      Vector3 newMiddlePos = Handles.PositionHandle(middleStartPos, Quaternion.identity);
      if(newMiddlePos != middleStartPos)
      {
        Vector2 oldMiddleSize = m_MiddleExtentRenderer.size;
        oldMiddleSize.y = Vector3.Distance(newMiddlePos, m_MiddleExtentRenderer.transform.position);
        if(oldMiddleSize.y < 1) oldMiddleSize.y = 1;
        if(m_Snapping) oldMiddleSize.y = Mathf.Round(oldMiddleSize.y);
        m_MiddleExtentRenderer.size = oldMiddleSize;
        var offset = m_MiddleExtentCollider.offset;
        offset.x = oldMiddleSize.x / 2;
        offset.y = oldMiddleSize.y / 2;
        m_MiddleExtentCollider.offset = offset;
        m_MiddleExtentCollider.size = oldMiddleSize;

        var oldTopCornerPos = m_TopCornerCollider.transform.localPosition;
        oldTopCornerPos.y = oldMiddleSize.y + 1;
        m_TopCornerCollider.transform.localPosition = oldTopCornerPos;
        
        var oldTopExtentPos = m_TopExtentCollider.transform.localPosition;
        oldTopExtentPos.y = oldMiddleSize.y + 1;
        m_TopExtentCollider.transform.localPosition = oldTopExtentPos;

        Repaint();

      }
      #endregion
    }


  }
}