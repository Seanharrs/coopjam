using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoDoor))]
public class AutoDoorEditor : Editor {

    public override void OnInspectorGUI()
    {

      AutoDoor door = (AutoDoor)target;

      DrawDefaultInspector();

      if(GUILayout.Button("Set From Current Position")) {
        door.m_ClosePos = door.transform.position;
        door.m_OpenPos = door.m_ClosePos + (door.GetComponent<SpriteRenderer>().bounds.size.y * Vector3.up);
        EditorUtility.SetDirty(door); // TODO: Unity does not recommend this as it does not have an "undo" operation attached when applying changes. However, using serialized properties was just a lot to manage when writing this, so leaving that til later.
      }
    }
}
