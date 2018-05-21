using UnityEngine;
using UnityEditor;

namespace Coop
{
  [CustomEditor(typeof(CircuitObject))]
  public class CircuitObjectEditor : Editor
  {
    private bool m_MultiSwitch;

    private SerializedProperty multiSwitch;
    private SerializedProperty onPositive;
    private SerializedProperty onNegative;
    private SerializedProperty onEnd;

    private void OnEnable()
    {
      multiSwitch = serializedObject.FindProperty("m_MultiSwitch");
      onPositive = serializedObject.FindProperty("onStateChanged_Positive");
      onNegative = serializedObject.FindProperty("onStateChanged_Negative");
      onEnd = serializedObject.FindProperty("onStateChanged_Off");
    }

    public override void OnInspectorGUI()
    {
      GUI.enabled = false;
      EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((CircuitObject)target), typeof(CircuitObject), false);
      GUI.enabled = true;

      EditorGUILayout.PropertyField(multiSwitch, new GUIContent("Multi-state"));

      EditorGUILayout.Space();

      if (multiSwitch.boolValue)
      {
        EditorGUILayout.PropertyField(onPositive, new GUIContent("On State Change Positive"));
        EditorGUILayout.PropertyField(onNegative, new GUIContent("On State Change Negative"));
      }
      else
        EditorGUILayout.PropertyField(onPositive, new GUIContent("On State Change On"));

      EditorGUILayout.PropertyField(onEnd, new GUIContent("On State Change Off"));

      serializedObject.ApplyModifiedProperties();
    }
  }
}