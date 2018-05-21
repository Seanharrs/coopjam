using UnityEngine;
using UnityEditor;

namespace Coop
{
  [CustomEditor(typeof(CircuitObject))]
  public class CircuitObjectEditor : Editor
  {
    private SerializedProperty m_IsMultiState;
    private SerializedProperty m_OnPositive;
    private SerializedProperty m_OnNegative;
    private SerializedProperty m_OnEnd;

    private void OnEnable()
    {
      m_IsMultiState = serializedObject.FindProperty("m_IsMultiState");
      m_OnPositive = serializedObject.FindProperty("m_OnStateChanged_Positive");
      m_OnNegative = serializedObject.FindProperty("m_OnStateChanged_Negative");
      m_OnEnd = serializedObject.FindProperty("m_OnStateChanged_Off");
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