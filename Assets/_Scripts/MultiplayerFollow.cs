using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Coop
{
	public class MultiplayerFollow : MonoBehaviour
	{
		private enum Zoom { Out, In, None };

		/// <summary>The world coordinate of the bottom left point of the camera view.</summary>
		public Vector2 minVisiblePos
		{
			get
			{
				Vector2 pos = transform.position;
				pos.x -= horizLength;
				pos.y -= vertLength;
				return pos;
			}
		}

		/// <summary>The world coordinate of the top right point of the camera view.</summary>
		public Vector2 maxVisiblePos
		{
			get
			{
				Vector2 pos = transform.position;
				pos.x += horizLength;
				pos.y += vertLength;
				return pos;
			}
		}

		/// <summary>The distance between the top most and bottom most points visible to the camera.</summary>
		public float vertLength { get { return m_CurrZoom; } }

		/// <summary>The distance between the left most and right most points visible to the camera.</summary>
		public float horizLength { get { return m_CurrZoom * Screen.width / Screen.height; } }

		[SerializeField]
		private Vector3 m_Offset = new Vector3(0, 0, -10f);

		[SerializeField]
		private Transform m_BottomLeftIndicator;

		[SerializeField]
		private Transform m_TopRightIndicator;

		private Vector2 m_LevelMin = Vector2.zero, m_LevelMax = Vector2.zero;
		private Vector2 m_CamMin { get { return m_LevelMin + new Vector2(horizLength, vertLength); } }
		private Vector2 m_CamMax { get { return m_LevelMax - new Vector2(horizLength, vertLength); } }

		private CoopUserControl[] m_Players;

		private const float MAX_ZOOM_LEVEL = 14f;
		private const float MIN_ZOOM_LEVEL = 8f;
		private const float DELTA_ZOOM = 0.01f;

		private Camera m_Cam;
		private float m_CurrZoom;
		private List<Zoom> m_ZoomStates = new List<Zoom>();

		private void Awake()
		{
			m_Cam = GetComponent<Camera>();
			m_CurrZoom = m_Cam.orthographicSize;

			AcquirePlayerRefs();

			m_LevelMin.x = m_BottomLeftIndicator.position.x;
			m_LevelMin.y = m_BottomLeftIndicator.position.y;

			m_LevelMax.x = m_TopRightIndicator.position.x;
			m_LevelMax.y = m_TopRightIndicator.position.y;

			m_BottomLeftIndicator.gameObject.SetActive(false);
			m_TopRightIndicator.gameObject.SetActive(false);
		}

		internal void AcquirePlayerRefs()
		{
			m_Players = FindObjectsOfType<CoopUserControl>();
		}

		private void LateUpdate()
		{
			if(m_Players == null || m_Players.Count() == 0)
				return;

			if(m_ZoomStates.All(z => z == Zoom.In) && m_CurrZoom > MIN_ZOOM_LEVEL)
			{
				float newZoom = m_CurrZoom - DELTA_ZOOM;
				m_CurrZoom = newZoom < MIN_ZOOM_LEVEL ? MIN_ZOOM_LEVEL : newZoom;
				m_Cam.orthographicSize = m_CurrZoom;
			}

			m_ZoomStates = new List<Zoom>();
			Debug.Log("NEW FRAME");

			float maxY = m_Players.Max(p => p.transform.position.y);
			Vector3 avgPos = m_Players.Select(p => p.transform.position).Aggregate((total, next) => total += next) / m_Players.Length;

			avgPos.y = Mathf.Max(avgPos.y, maxY - vertLength / 2);

			Vector3 clamped = avgPos + m_Offset;
			clamped.x = Mathf.Clamp(clamped.x, m_CamMin.x, m_CamMax.x);
			clamped.y = Mathf.Clamp(clamped.y, m_CamMin.y, m_CamMax.y);
			transform.position = clamped;
		}

		/// <summary>Constrains an object to be fully within the view of the camera.</summary>
		/// <param name="pos">The world position of the object to be constrained.</param>
		/// <param name="spriteBounds">The visual bounds of the object to be constrained.</param>
		/// <param name="constrainAxisY">Should the object be constrained along the camera Y axis.</param>
		/// <returns>The constrained world position of the object.</returns>
		public Vector3 ConstrainToView(Vector3 pos, Vector3 spriteBounds, bool constrainAxisY = false)
		{
			Vector3 camPos = transform.position;

			float minX = camPos.x - horizLength + spriteBounds.x;
			float maxX = camPos.x + horizLength - spriteBounds.x;
			
			float zoomPadding = 2f;
			if(pos.x >= maxX - (zoomPadding / 2) || pos.x <= minX + (zoomPadding / 2))
			{
				if(m_CurrZoom < MAX_ZOOM_LEVEL)
				{
					float newZoom = m_CurrZoom + DELTA_ZOOM;
					m_CurrZoom = newZoom > MAX_ZOOM_LEVEL ? MAX_ZOOM_LEVEL : newZoom;
					m_Cam.orthographicSize = m_CurrZoom;
				}
				m_ZoomStates.Add(Zoom.Out);
			}
			else if(pos.x <= maxX - zoomPadding && pos.x >= minX + zoomPadding)
				m_ZoomStates.Add(Zoom.In);
			else
				m_ZoomStates.Add(Zoom.None);
			Debug.Log(m_ZoomStates.Last());
			pos.x = Mathf.Clamp(pos.x, minX, maxX);

			if(constrainAxisY)
			{
				float minY = camPos.y - vertLength + spriteBounds.y;
				float maxY = camPos.y + vertLength - spriteBounds.y;
				pos.y = Mathf.Clamp(pos.y, minY, maxY);
			}
			else
			{
				float maxY = camPos.y + vertLength - spriteBounds.y;
				pos.y = Mathf.Clamp(pos.y, pos.y, maxY);
			}

			return pos;
		}

		/// <summary>
		/// Callback to draw gizmos that are pickable and always drawn.
		/// </summary>
		void OnDrawGizmos()
		{
			if(m_BottomLeftIndicator && m_TopRightIndicator)
			{
				var bottomLeft = m_BottomLeftIndicator.transform.position;
				var topRight = m_TopRightIndicator.transform.position;
				var topLeft = new Vector3(bottomLeft.x, topRight.y);
				var bottomRight = new Vector3(topRight.x, bottomLeft.y);

				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(bottomLeft, topLeft + Vector3.up);
				Gizmos.DrawLine(bottomLeft, bottomRight + Vector3.right);
				Gizmos.DrawLine(topRight, topLeft - Vector3.right);
				Gizmos.DrawLine(topRight, bottomRight + Vector3.down);
			}
		}
	}
}