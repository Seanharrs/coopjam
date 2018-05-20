using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Coop
{
    [CustomEditor(typeof(AutoDoor))]
    public class AutoDoorEditor : Editor
    {
        private SerializedProperty m_ClosePos;
        private SerializedProperty m_OpenPos;
        
        private void OnEnable()
        {
            m_ClosePos = serializedObject.FindProperty("m_ClosePos");
            m_OpenPos = serializedObject.FindProperty("m_OpenPos");
        }

        public override void OnInspectorGUI()
        {
          serializedObject.Update();

          AutoDoor door = (AutoDoor)target;
          DrawDefaultInspector();

          if(GUILayout.Button("Set From Current Position")) {
            m_ClosePos.vector3Value = door.transform.position;
            m_OpenPos.vector3Value = m_ClosePos.vector3Value + (door.GetComponent<SpriteRenderer>().bounds.size.y * Vector3.up);
            serializedObject.ApplyModifiedProperties();
          }
        }
    }
}