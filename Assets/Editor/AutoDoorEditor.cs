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
        
        private void OnEnable()
        {
            m_ClosedPos = serializedObject.FindProperty("m_ClosedPos");
            m_OpenPos = serializedObject.FindProperty("m_OpenPos");
        }

        public override void OnInspectorGUI()
        {
          serializedObject.Update();

          AutoDoor door = (AutoDoor)target;
          DrawDefaultInspector();

          if(GUILayout.Button("Set Open From Current"))
            SetPositions(door, m_ClosedPos, m_OpenPos, Vector3.up);

		  if(GUILayout.Button("Set Closed From Current"))
		    SetPositions(door, m_OpenPos, m_ClosedPos, -Vector3.up);

	      serializedObject.ApplyModifiedProperties();
		}

		private void SetPositions(AutoDoor door, SerializedProperty curr, SerializedProperty other, Vector3 dir)
		{
			curr.vector3Value = door.transform.position;
			other.vector3Value = curr.vector3Value + (door.GetComponent<SpriteRenderer>().bounds.size.y * dir);
		}
    }
}