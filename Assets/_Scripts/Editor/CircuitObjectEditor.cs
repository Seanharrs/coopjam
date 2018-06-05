using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

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

    public Sprite sprite_IconLightning;
    private Texture2D tex_IconLightning;
    public Sprite sprite_IconChainlink;
    private Texture2D tex_IconChainlink;

    public GUIStyle textureButtonStyle;
    public GUISkin buttonSkin;

    private List<ICircuitObjectListener> listeners = new List<ICircuitObjectListener>();
    private List<ICircuitObjectListener> connectedListeners = new List<ICircuitObjectListener>();

    private void OnEnable()
    {
      m_Target = (CircuitObject)target;
      m_IsMultiState = serializedObject.FindProperty("m_IsMultiState");
      m_OnPositive = serializedObject.FindProperty("m_OnStateChanged_Positive");
      m_OnNegative = serializedObject.FindProperty("m_OnStateChanged_Negative");
      m_OnEnd = serializedObject.FindProperty("m_OnStateChanged_Off");

      tex_IconLightning = SpriteToTexture(sprite_IconLightning, 1f);
      tex_IconChainlink = SpriteToTexture(sprite_IconChainlink, 1f);

      textureButtonStyle = buttonSkin.button;

      listeners = FindObjectsOfType<MonoBehaviour>().OfType<ICircuitObjectListener>().ToList();

      GetConnectedListeners();
    }

    private Texture2D SpriteToTexture(Sprite sprite, float sizeFactor = 1f)
    {
      if (!sprite) return null;
      var newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
      var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
      newTexture.SetPixels(pixels);
      newTexture.Apply();
      if (sizeFactor != 1f)
      {
        newTexture.Resize((int)(sprite.rect.width * sizeFactor), (int)(sprite.rect.height * sizeFactor));
        newTexture.Apply();
      }
      return newTexture;
    }

    private void OnSceneGUI()
    {
      float size = HandleUtility.GetHandleSize(m_Target.transform.position) * 0.25f;

      var sr = m_Target.GetComponent<SpriteRenderer>();

      //Vector3 point = m_Target.transform.position + Vector3.down;
      Vector3 bottomCenterPoint = sr.bounds.center + sr.bounds.extents.y * Vector3.down;
      Vector3 spriteCenterPoint = sr.bounds.center;

      if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape
        || Event.current.isMouse && Event.current.button == 1)
      {
        if (isConnecting) isConnecting = false;
      }

      Rect sourceRect = new Rect();

      if (tex_IconLightning != null)
      {
        //Handles.Label(point, tex_IconLighting);
        Handles.BeginGUI();
        var guiPoint = HandleUtility.WorldToGUIPoint(bottomCenterPoint) - tex_IconLightning.height / 2 * Vector2.down;
        
        var rect = new Rect(guiPoint.x - (tex_IconLightning.width / 2), 
                            guiPoint.y - (tex_IconLightning.height / 2), 
                            tex_IconLightning.width, 
                            tex_IconLightning.height);
        sourceRect = rect;

        if (GUI.Button(rect, new GUIContent(tex_IconLightning), textureButtonStyle))
        {
          isConnecting = !isConnecting;
          if (isConnecting)
          {
            listeners = FindObjectsOfType<MonoBehaviour>().OfType<ICircuitObjectListener>().ToList();
            GetConnectedListeners();
          }
        }
        Handles.EndGUI();
      }

      var startPoint = HandleUtility.GUIPointToWorldRay(sourceRect.center);
      if (connectedListeners != null && connectedListeners.Count > 0)
        connectedListeners.ForEach(listener =>
        {
          Handles.DrawDottedLine(spriteCenterPoint, ((MonoBehaviour)listener).transform.position, size * lineSize);
        });

      if (isConnecting)
      {
        if (listeners != null && listeners.Count > 0)
          listeners.ForEach(listener =>
          {
            MonoBehaviour listenerBehavior = listener as MonoBehaviour;

            Handles.color = Color.red;
            Handles.BeginGUI();
            var guiPoint = HandleUtility.WorldToGUIPoint(listenerBehavior.transform.position);

            var rect = new Rect(guiPoint.x - (tex_IconChainlink.width / 2),
                                guiPoint.y - (tex_IconChainlink.height / 2),
                                tex_IconChainlink.width,
                                tex_IconChainlink.height);

            if (GUI.Button(rect, new GUIContent(tex_IconChainlink), textureButtonStyle))
            {
              ConnectCircuitEditorWindow.Show(m_Target, listener);
              isConnecting = false;
            }
            Handles.EndGUI();
          });

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;
        Handles.color = Color.yellow;
        Handles.DrawLine(startPoint.origin, mousePosition);
      }
      
      HandleUtility.Repaint();

    }

    private void GetConnectedListeners()
    {
      connectedListeners.Clear();

      var positiveEventCount = m_Target.m_OnStateChanged_Positive.GetPersistentEventCount();
      for (var i = 0; i < positiveEventCount; i++)
      {
        var validListener = listeners != null && listeners.Count > 0 ? listeners.FirstOrDefault(l => (m_Target.m_OnStateChanged_Positive.GetPersistentTarget(i) as ICircuitObjectListener) == l) : null;
        if (validListener != null)
        {
          connectedListeners.Add(validListener);
        }
      }

      var negativeEventCount = m_Target.m_OnStateChanged_Negative.GetPersistentEventCount();
      for (var i = 0; i < negativeEventCount; i++)
      {
        var validListener = listeners != null && listeners.Count > 0 ? listeners.FirstOrDefault(l => (m_Target.m_OnStateChanged_Negative.GetPersistentTarget(i) as ICircuitObjectListener) == l) : null;
        if (validListener != null)
        {
          connectedListeners.Add(validListener);
        }
      }

      var offEventCount = m_Target.m_OnStateChanged_Off.GetPersistentEventCount();
      for (var i = 0; i < offEventCount; i++)
      {
        var validListener = listeners != null && listeners.Count > 0 ? listeners.FirstOrDefault(l => (m_Target.m_OnStateChanged_Off.GetPersistentTarget(i) as ICircuitObjectListener) == l) : null;
        if (validListener != null)
        {
          connectedListeners.Add(validListener);
        }
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