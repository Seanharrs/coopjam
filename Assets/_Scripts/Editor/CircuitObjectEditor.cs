using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Coop
{
  [CustomEditor(typeof(CircuitObject))]
  public class CircuitObjectEditor : Editor
  {
    private SerializedProperty m_IsMultiState;
    private SerializedProperty m_OnPositive;
    private SerializedProperty m_OnNegative;
    private SerializedProperty m_OnEnd;
    private CircuitObject m_Target;

    public static bool isConnecting = false;
    private const float handleSize = 0.8f;
    private const float pickSize = 0.6f;
    private const float lineSize = 10f;

    private List<ICircuitObjectListener> listeners = new List<ICircuitObjectListener>();

    private void OnEnable()
    {
      m_Target = (CircuitObject)target;
      m_IsMultiState = serializedObject.FindProperty("m_IsMultiState");
      m_OnPositive = serializedObject.FindProperty("m_OnStateChanged_Positive");
      m_OnNegative = serializedObject.FindProperty("m_OnStateChanged_Negative");
      m_OnEnd = serializedObject.FindProperty("m_OnStateChanged_Off");
    }

    private void OnSceneGUI()
    {
      float size = HandleUtility.GetHandleSize(m_Target.transform.position) * 0.25f;

      Vector3 point = m_Target.transform.position + Vector3.right;

      if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape
        || Event.current.isMouse && Event.current.button == 1)
      {
        if (isConnecting) isConnecting = false;
      }

      if(Handles.Button(point, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotHandleCap))
      {
        isConnecting = !isConnecting;
        if(isConnecting)
        {
          listeners = FindObjectsOfType<MonoBehaviour>().OfType<ICircuitObjectListener>().ToList();
        }
      }

      if(isConnecting)
      {
        if (listeners != null && listeners.Count > 0)
          listeners.ForEach(listener =>
          {
            MonoBehaviour listenerBehavior = listener as MonoBehaviour;
            Handles.color = Color.red;
            if(Handles.Button(listenerBehavior.transform.position, Quaternion.identity, size * handleSize * 2, size * pickSize, Handles.SphereHandleCap))
            {
              ConnectCircuitEditorWindow.Show(m_Target, listener);
              isConnecting = false;
            }
          });

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;
        Handles.DrawDottedLine(point, mousePosition, size * lineSize);
        HandleUtility.Repaint();
      }
      
    }

    public override void OnInspectorGUI()
    {
	  serializedObject.Update();
      
      GUI.enabled = false;
      EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((CircuitObject)target), typeof(CircuitObject), false);
      GUI.enabled = true;

      EditorGUILayout.PropertyField(m_IsMultiState, new GUIContent("Multi-state"));

      EditorGUILayout.Space();

      if (m_IsMultiState.boolValue)
      {
        EditorGUILayout.PropertyField(m_OnPositive, new GUIContent("On State Change Positive"));
        EditorGUILayout.PropertyField(m_OnNegative, new GUIContent("On State Change Negative"));
      }
      else
        EditorGUILayout.PropertyField(m_OnPositive, new GUIContent("On State Change On"));

      EditorGUILayout.PropertyField(m_OnEnd, new GUIContent("On State Change Off"));

      serializedObject.ApplyModifiedProperties();
    }
  }
}