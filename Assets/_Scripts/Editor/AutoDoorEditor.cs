using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Coop
{
  [CustomEditor(typeof(AutoDoor))]
  public class AutoDoorEditor : Editor
  {
    private SerializedProperty m_ClosedPos;
    private SerializedProperty m_OpenPos;

    private AutoDoor m_Door;
    
    static bool m_Snapping = true;

    private void OnEnable()
    {
      m_ClosedPos = serializedObject.FindProperty("m_ClosedPos");
      m_OpenPos = serializedObject.FindProperty("m_OpenPos");
      m_Door = (AutoDoor) target;
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      AutoDoor door = (AutoDoor)target;
      DrawDefaultInspector();

      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Set Open From Current"))
        SetPositions(door, m_ClosedPos, m_OpenPos, Vector3.up);

      if (GUILayout.Button("Set Closed From Current"))
        SetPositions(door, m_OpenPos, m_ClosedPos, -Vector3.up);
      GUILayout.EndHorizontal();

      GUILayout.Label("Note:", EditorStyles.boldLabel);
      GUILayout.Box("Green Handle represents the bottom-left corner of the 'closed' position. Red handle represents the bottom-left corner of the 'opened' position.", GUILayout.ExpandWidth(true));

      m_Snapping = GUILayout.Toggle(m_Snapping, "Snap Position Handles?");

      serializedObject.ApplyModifiedProperties();
    }

    private void SetPositions(AutoDoor door, SerializedProperty curr, SerializedProperty other, Vector3 dir)
    {
      curr.vector3Value = door.transform.position;
      other.vector3Value = curr.vector3Value + (door.GetComponent<SpriteRenderer>().bounds.size.y * dir);
    }

    void OnSceneGUI()
    {
      
      float size = HandleUtility.GetHandleSize(m_Door.m_ClosedPos) * 0.25f;
      float snap = 1f;
      
      Vector3 handleDirection = Vector3.up;

      Handles.color = Color.green;
      EditorGUI.BeginChangeCheck();
      Vector3 newTargetPosition = Handles.Slider2D(m_Door.m_ClosedPos, m_Door.transform.forward, m_Door.transform.right, m_Door.transform.up, size, Handles.SphereHandleCap, snap);
      if (EditorGUI.EndChangeCheck())
      {
        if(m_Snapping)
          newTargetPosition = new Vector3(Mathf.Round(newTargetPosition.x),
                                          Mathf.Round(newTargetPosition.y),
                                          Mathf.Round(newTargetPosition.z));
        Undo.RecordObject(m_Door, "Change Door Close Position");
        m_Door.m_ClosedPos = newTargetPosition;
      }

      Handles.color = Color.red;
      EditorGUI.BeginChangeCheck();
      newTargetPosition = Handles.Slider2D(m_Door.m_OpenPos, m_Door.transform.forward, m_Door.transform.right, m_Door.transform.up, size, Handles.SphereHandleCap, snap);
      if (EditorGUI.EndChangeCheck())
      {
        if(m_Snapping)
          newTargetPosition = new Vector3(Mathf.Round(newTargetPosition.x),
                                          Mathf.Round(newTargetPosition.y),
                                          Mathf.Round(newTargetPosition.z));
        Undo.RecordObject(m_Door, "Change Door Open Position");
        m_Door.m_OpenPos = newTargetPosition;
      }
    }

  }
}