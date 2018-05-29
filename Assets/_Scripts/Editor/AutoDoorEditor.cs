using UnityEngine;
using UnityEditor;

namespace Coop
{
	[CustomEditor(typeof(AutoDoor))]
	public class AutoDoorEditor : Editor
	{
		private SerializedProperty m_ClosedPos;
		private SerializedProperty m_OpenPos;

		private Transform m_Transform;

		static bool m_Snapping = true;

		private void OnEnable()
		{
			m_ClosedPos = serializedObject.FindProperty("m_ClosedPos");
			m_OpenPos = serializedObject.FindProperty("m_OpenPos");
			m_Transform = ((AutoDoor)target).transform;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			AutoDoor door = (AutoDoor)target;
			DrawDefaultInspector();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Set Open From Current"))
				SetPositions(door, m_ClosedPos, m_OpenPos, Vector3.up);

			if(GUILayout.Button("Set Closed From Current"))
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

		private void OnSceneGUI()
		{

			float size = HandleUtility.GetHandleSize(m_ClosedPos.vector3Value) * 0.25f;
			float snap = 1f;

			Vector3 handleDirection = Vector3.up;

			Handles.color = Color.green;
			EditorGUI.BeginChangeCheck();
			Vector3 newTargetPosition = Handles.Slider2D(m_ClosedPos.vector3Value, m_Transform.forward, m_Transform.right, m_Transform.up, size, Handles.SphereHandleCap, snap);
			if(EditorGUI.EndChangeCheck())
			{
				if(m_Snapping)
					newTargetPosition = new Vector3(Mathf.Round(newTargetPosition.x),
													Mathf.Round(newTargetPosition.y),
													Mathf.Round(newTargetPosition.z));
				Undo.RecordObject(target, "Change Door Close Position");
				m_ClosedPos.vector3Value = newTargetPosition;
			}

			Handles.color = Color.red;
			EditorGUI.BeginChangeCheck();
			newTargetPosition = Handles.Slider2D(m_OpenPos.vector3Value, m_Transform.forward, m_Transform.right, m_Transform.up, size, Handles.SphereHandleCap, snap);
			if(EditorGUI.EndChangeCheck())
			{
				if(m_Snapping)
					newTargetPosition = new Vector3(Mathf.Round(newTargetPosition.x),
													Mathf.Round(newTargetPosition.y),
													Mathf.Round(newTargetPosition.z));
				Undo.RecordObject(target, "Change Door Open Position");
				m_OpenPos.vector3Value = newTargetPosition;
			}
		}
	}
}