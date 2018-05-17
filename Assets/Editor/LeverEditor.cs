using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Lever))]
public class LeverEditor : Editor
{
    SerializedProperty m_ConnectedObj;
    SerializedProperty m_ActivationType;
    SerializedProperty m_TimeActive;

    private void OnEnable()
    {
        m_ConnectedObj = serializedObject.FindProperty("m_ConnectedObj");
        m_ActivationType = serializedObject.FindProperty("m_ActivationType");
        m_TimeActive = serializedObject.FindProperty("m_TimeActive");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Lever)target), typeof(Lever), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(m_ConnectedObj);
        EditorGUILayout.PropertyField(m_ActivationType);

        if(m_ActivationType.enumValueIndex == (int)Lever.ActivationType.Timed)
            m_TimeActive.floatValue = EditorGUILayout.FloatField(new GUIContent("Time Active"), m_TimeActive.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
