using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Coop
{
  [CustomGridBrush(true, false, false, "[Coop Game] > Prefab Brush")]
  public class PrefabBrush : GridBrush
  {
    [SerializeField]
    internal static GameObject currentPrefab;

    static PrefabBrush()
    {
      Selection.selectionChanged += SelectionChanged;
    }

    private static void SelectionChanged()
    {
      UpdatePrefab();
    }

    internal static GameObject UpdatePrefab() 
    {
      GameObject obj = Selection.activeGameObject;
      if(!obj) return null;
      if (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
      {
        Debug.Log(obj.name + ": Selected a prefab.");
        currentPrefab = obj;

        return obj;
      }
      else
      {
        Debug.LogWarning(obj.name + ": Not a prefab.");
        return null;
      }
    }

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
      if(!currentPrefab) return;

      if (brushTarget.layer == 31)
				return;

      GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(currentPrefab);
      Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
			if (instance != null)
			{
				instance.transform.SetParent(brushTarget.transform);
				instance.transform.position = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(new Vector3Int(position.x, position.y, 0))); // + new Vector3(.5f, .5f, .5f)));
			}

      base.Paint(gridLayout, brushTarget, position);
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, 0));
			if (erased != null)
				Undo.DestroyObjectImmediate(erased.gameObject);
		}

    private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
		{
			int childCount = parent.childCount;
			Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
			Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
			Bounds bounds = new Bounds((max + min)*.5f, max - min);

			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (bounds.Contains(child.position))
					return child;
			}
			return null;
		}
  }

  [CustomEditor(typeof(PrefabBrush))]
	public class PrefabBrushEditor : GridBrushEditorBase
	{
		private PrefabBrush prefabBrush { get { return target as PrefabBrush; } }

		private SerializedProperty m_Prefabs;
		private SerializedObject m_SerializedObject;

		protected void OnEnable()
		{
			m_SerializedObject = new SerializedObject(target);
			//m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
      PrefabBrush.UpdatePrefab();
		}

		public override void OnPaintInspectorGUI()
		{
			m_SerializedObject.UpdateIfRequiredOrScript();
			// prefabBrush.m_PerlinScale = EditorGUILayout.Slider("Perlin Scale", prefabBrush.m_PerlinScale, 0.001f, 0.999f);
			// prefabBrush.m_Z = EditorGUILayout.IntField("Position Z", prefabBrush.m_Z);
				
			//EditorGUILayout.PropertyField(m_Prefabs, true);

      if(PrefabBrush.currentPrefab)
        GUILayout.Label(AssetPreview.GetAssetPreview(PrefabBrush.currentPrefab));
      
      m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();

      
		}
	}

}