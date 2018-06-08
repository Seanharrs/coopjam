 // AutoBreakPrefabConnection.cs
 
 using UnityEngine;
 
 [ExecuteInEditMode]
 public class AutoBreakPrefabConnection : MonoBehaviour
 {
     void Start()
     {
         #if UNITY_EDITOR
          UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
         #endif
         DestroyImmediate(this); // Remove this script
     }
 }