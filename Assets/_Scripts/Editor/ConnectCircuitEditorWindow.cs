using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Coop
{
  class ConnectCircuitEditorWindow : EditorWindow
  {

    CircuitObject m_Source;
    ICircuitObjectListener m_Target;

    bool m_AddForOnOrPositive = true;
    bool m_AddForNegative = true;
    bool m_AddForOff = true;

    public static void Show(CircuitObject source, ICircuitObjectListener target)
    {
      var window = (ConnectCircuitEditorWindow)EditorWindow.GetWindow(typeof(ConnectCircuitEditorWindow));
      window.m_Source = source;
      window.m_Target = target;
      window.titleContent = new GUIContent("Circuit Listeners");
      window.Show();
    }

    private void OnGUI()
    {

      // Checkboxes

      if (m_Source.IsMultiState)
      {
        m_AddForOnOrPositive = GUILayout.Toggle(m_AddForOnOrPositive, "Add Listener for Positive");
        m_AddForNegative = GUILayout.Toggle(m_AddForNegative, "Add Listener for Negative");
      }
      else
      {
        m_AddForOnOrPositive = GUILayout.Toggle(m_AddForOnOrPositive, "Add Listener for On");
        m_AddForNegative = false;
      }

      m_AddForOff = GUILayout.Toggle(m_AddForOff, "Add Listener for Off");

      GUILayout.BeginHorizontal();

      if(GUILayout.Button("Save"))
      {

        UnityEditor.Events.UnityEventTools.RemovePersistentListener(m_Source.m_OnStateChanged_Positive, new UnityAction<CircuitObject>(m_Target.OnStateChangePositive));
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(m_Source.m_OnStateChanged_Negative, new UnityAction<CircuitObject>(m_Target.OnStateChangeNegative));
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(m_Source.m_OnStateChanged_Off, new UnityAction<CircuitObject>(m_Target.OnStateChangeOff));

        if (m_AddForOnOrPositive)
          UnityEditor.Events.UnityEventTools.AddPersistentListener(m_Source.m_OnStateChanged_Positive, m_Target.OnStateChangePositive);
        if (m_AddForNegative)
          UnityEditor.Events.UnityEventTools.AddPersistentListener(m_Source.m_OnStateChanged_Negative, m_Target.OnStateChangeNegative);
        if (m_AddForOff)
          UnityEditor.Events.UnityEventTools.AddPersistentListener(m_Source.m_OnStateChanged_Off, m_Target.OnStateChangeOff);

        EditorUtility.SetDirty(m_Source);

        Close();
      }
      if (GUILayout.Button("Cancel"))
        this.Close();

      GUILayout.EndHorizontal();
    }

  }
}
