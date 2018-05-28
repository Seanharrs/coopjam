using UnityEngine;
using UnityEditor;
using System;

namespace Coop
{
	[CustomEditor(typeof(BoxPlatform))]
	public class BoxPlatform_Editor : Editor
	{
		private BoxPlatform m_CornerPlatform;

		private Transform m_TopLeft;
		private Transform m_TopRight;
		private Transform m_BottomLeft;
		private Transform m_BottomRight;
		private Transform m_TopMid;
		private Transform m_BottomMid;
		private Transform m_LeftMid;
		private Transform m_RightMid;

		private SpriteRenderer m_TopLeftRenderer;
		private SpriteRenderer m_TopRightRenderer;
		private SpriteRenderer m_BottomLeftRenderer;
		private SpriteRenderer m_BottomRightRenderer;
		private SpriteRenderer m_TopMidRenderer;
		private SpriteRenderer m_BottomMidRenderer;
		private SpriteRenderer m_LeftMidRenderer;
		private SpriteRenderer m_RightMidRenderer;

		private BoxCollider2D m_TopLeftCollider;
		private BoxCollider2D m_TopRightCollider;
		private BoxCollider2D m_BottomLeftCollider;
		private BoxCollider2D m_BottomRightCollider;
		private BoxCollider2D m_TopMidCollider;
		private BoxCollider2D m_BottomMidCollider;
		private BoxCollider2D m_LeftMidCollider;
		private BoxCollider2D m_RightMidCollider;
		
		private static bool m_UseStandard = false;
		private static bool m_Snapping = true;

		private void Reset()
		{
			m_CornerPlatform = (BoxPlatform)target;

			m_TopLeft = m_CornerPlatform.transform.Find("Top-Left-Corner");
			m_TopRight = m_CornerPlatform.transform.Find("Top-Right-Corner");
			m_BottomLeft = m_CornerPlatform.transform.Find("Bottom-Left-Corner");
			m_BottomRight = m_CornerPlatform.transform.Find("Bottom-Right-Corner");
			m_TopMid = m_CornerPlatform.transform.Find("Top-Middle");
			m_BottomMid = m_CornerPlatform.transform.Find("Bottom-Middle");
			m_LeftMid = m_CornerPlatform.transform.Find("Left-Middle");
			m_RightMid = m_CornerPlatform.transform.Find("Right-Middle");

			m_TopLeftRenderer = m_TopLeft.GetComponent<SpriteRenderer>();
			m_TopRightRenderer = m_TopRight.GetComponent<SpriteRenderer>();
			m_BottomLeftRenderer = m_BottomLeft.GetComponent<SpriteRenderer>();
			m_BottomRightRenderer = m_BottomRight.GetComponent<SpriteRenderer>();
			m_TopMidRenderer = m_TopMid.GetComponent<SpriteRenderer>();
			m_BottomMidRenderer = m_BottomMid.GetComponent<SpriteRenderer>();
			m_LeftMidRenderer = m_LeftMid.GetComponent<SpriteRenderer>();
			m_RightMidRenderer = m_RightMid.GetComponent<SpriteRenderer>();

			m_TopLeftCollider = m_TopLeft.GetComponent<BoxCollider2D>();
			m_TopRightCollider = m_TopRight.GetComponent<BoxCollider2D>();
			m_BottomLeftCollider = m_BottomLeft.GetComponent<BoxCollider2D>();
			m_BottomRightCollider = m_BottomRight.GetComponent<BoxCollider2D>();
			m_TopMidCollider = m_TopMid.GetComponent<BoxCollider2D>();
			m_BottomMidCollider = m_BottomMid.GetComponent<BoxCollider2D>();
			m_LeftMidCollider = m_LeftMid.GetComponent<BoxCollider2D>();
			m_RightMidCollider = m_RightMid.GetComponent<BoxCollider2D>();

			m_TopLeft.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_TopRight.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_BottomLeft.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_BottomRight.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_TopMid.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_BottomMid.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_LeftMid.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;
			m_RightMid.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.NotEditable;

			m_TopLeftRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_TopRightRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomLeftRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomRightRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_TopMidRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomMidRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_LeftMidRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_RightMidRenderer.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;

			m_TopLeftCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_TopRightCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomLeftCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomRightCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_TopMidCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_BottomMidCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_LeftMidCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;
			m_RightMidCollider.hideFlags = m_UseStandard ? HideFlags.None : HideFlags.HideInInspector;

			EditorApplication.RepaintHierarchyWindow();
			EditorApplication.DirtyHierarchyWindowSorting();
		}

		public void DrawCustomInspector()
		{
			DrawSizeButtons("Width:", Vector2.right);
			DrawSizeButtons("Height:", Vector2.up);

			GUILayout.BeginHorizontal();
			m_Snapping = GUILayout.Toggle(m_Snapping, "Snapping");
			GUILayout.EndHorizontal();
		}

		private void DrawSizeButtons(string label, Vector2 dir)
		{
			SpriteRenderer leftRenderer;
			SpriteRenderer rightRenderer;
			BoxCollider2D leftColl;
			BoxCollider2D rightColl;
			Transform leftOffset;
			Transform midOffset;
			Transform rightOffset;

			Vector2 size;
			string lenStr;

			if(dir == Vector2.up)
			{
				leftRenderer = m_LeftMidRenderer;
				rightRenderer = m_RightMidRenderer;
				leftColl = m_LeftMidCollider;
				rightColl = m_RightMidCollider;
				leftOffset = m_TopLeft;
				midOffset = m_TopMid;
				rightOffset = m_TopRight;

				size = leftRenderer.size;
				lenStr = size.y.ToString();
			}
			else
			{
				leftRenderer = m_TopMidRenderer;
				rightRenderer = m_BottomMidRenderer;
				leftColl = m_TopMidCollider;
				rightColl = m_BottomMidCollider;
				leftOffset = m_TopRight;
				midOffset = m_RightMid;
				rightOffset = m_BottomRight;

				size = leftRenderer.size;
				lenStr = size.x.ToString();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label(label);

			string newLenStr = GUILayout.TextField(lenStr);
			if(newLenStr != lenStr && !String.IsNullOrEmpty(newLenStr) && int.Parse(newLenStr) >= 1)
			{
				if(dir == Vector2.up)
					size.y = int.Parse(newLenStr);
				else
					size.x = int.Parse(newLenStr);
			}
			if(GUILayout.Button("-", GUILayout.Width(20)) && (size * dir).magnitude > 1)
				size -= dir;
			if(GUILayout.Button("+", GUILayout.Width(20)))
				size += dir;

			GUILayout.EndHorizontal();

			if(size != leftRenderer.size)
			{
				leftRenderer.size = size;
				var offset = leftColl.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				leftColl.offset = offset;
				leftColl.size = size;

				rightRenderer.size = size;
				offset = rightColl.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				rightColl.offset = offset;
				rightColl.size = size;

				if(dir == Vector2.up)
				{
					var newSize = leftOffset.localPosition;
					newSize.y = size.y + 1;
					leftOffset.localPosition = newSize;

					newSize = midOffset.localPosition;
					newSize.y = size.y + 1;
					midOffset.localPosition = newSize;

					newSize = rightOffset.localPosition;
					newSize.y = size.y + 1;
					rightOffset.localPosition = newSize;
				}
				else
				{
					var newSize = leftOffset.localPosition;
					newSize.x = size.x + 1;
					leftOffset.localPosition = newSize;

					newSize = midOffset.localPosition;
					newSize.x = size.x + 1;
					midOffset.localPosition = newSize;

					newSize = rightOffset.localPosition;
					newSize.x = size.x + 1;
					rightOffset.localPosition = newSize;
				}
			}
		}

		public override void OnInspectorGUI()
		{
			if(!m_UseStandard)
			{
				DrawCustomInspector();
				if(GUILayout.Button("Use Standard Inspectors"))
				{
					m_UseStandard = true;
					Reset();
				}
			}
			else if(m_UseStandard && GUILayout.Button("Use Custom Inspector"))
			{
				m_UseStandard = false;
				Reset();
			}
		}

		private void OnSceneGUI()
		{
			Handles.color = Color.blue;

			#region Width Extent
			Vector3 startWidth = m_RightMid.position + m_RightMidRenderer.size.x * m_RightMid.right + m_RightMidRenderer.size.y * m_RightMid.up / 2;
			Vector3 newWidth = Handles.Slider(startWidth, m_RightMid.right, HandleUtility.GetHandleSize(startWidth) * .25f, Handles.SphereHandleCap, 1f);
			if(newWidth != startWidth)
			{
				Vector2 size = m_TopMidRenderer.size;
				size.x = newWidth.x - m_TopMid.position.x;
				if(size.x < 1) size.x = 1;
				if(m_Snapping) size.x = Mathf.Round(size.x);

				m_TopMidRenderer.size = size;
				var offset = m_TopMidCollider.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				m_TopMidCollider.offset = offset;
				m_TopMidCollider.size = size;

				size = m_BottomMidRenderer.size;
				size.x = newWidth.x - m_BottomMid.position.x;
				if(size.x < 1) size.x = 1;
				if(m_Snapping) size.x = Mathf.Round(size.x);

				m_BottomMidRenderer.size = size;
				offset = m_BottomMidCollider.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				m_BottomMidCollider.offset = offset;
				m_BottomMidCollider.size = size;

				var newSize = m_TopRight.localPosition;
				newSize.x = size.x + 1;
				m_TopRight.localPosition = newSize;

				newSize = m_RightMid.localPosition;
				newSize.x = size.x + 1;
				m_RightMid.localPosition = newSize;

				newSize = m_BottomRight.localPosition;
				newSize.x = size.x + 1;
				m_BottomRight.localPosition = newSize;

				Repaint();
			}
			#endregion

			#region Height Extent
			Vector3 startHeight = m_TopMid.position + m_TopMidRenderer.size.x * m_TopMid.right / 2 + m_TopMidRenderer.size.y * m_TopMid.up;
			Vector3 newHeight = Handles.Slider(startHeight, m_TopMid.up, HandleUtility.GetHandleSize(startHeight) * .25f, Handles.SphereHandleCap, 1f);
			if(newHeight != startHeight)
			{
				Vector2 size = m_LeftMidRenderer.size;
				size.y = newHeight.y - m_LeftMid.position.y;
				if(size.y < 1) size.y = 1;
				if(m_Snapping) size.y = Mathf.Round(size.y);

				m_LeftMidRenderer.size = size;
				var offset = m_LeftMidCollider.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				m_LeftMidCollider.offset = offset;
				m_LeftMidCollider.size = size;

				size = m_RightMidRenderer.size;
				size.y = newHeight.y - m_RightMid.position.y;
				if(size.y < 1) size.y = 1;
				if(m_Snapping) size.y = Mathf.Round(size.y);

				m_RightMidRenderer.size = size;
				offset = m_RightMidCollider.offset;
				offset.x = size.x / 2;
				offset.y = size.y / 2;
				m_RightMidCollider.offset = offset;
				m_RightMidCollider.size = size;

				var newSize = m_TopLeft.localPosition;
				newSize.y = size.y + 1;
				m_TopLeft.localPosition = newSize;

				newSize = m_TopMid.localPosition;
				newSize.y = size.y + 1;
				m_TopMid.localPosition = newSize;

				newSize = m_TopRight.localPosition;
				newSize.y = size.y + 1;
				m_TopRight.localPosition = newSize;

				Repaint();
			}
			#endregion
		}
	}
}