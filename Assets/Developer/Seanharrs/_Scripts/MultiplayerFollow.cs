using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiplayerFollow : MonoBehaviour
{
    private Coop.Platformer2DUserControl[] m_Players;

    [SerializeField]
    private Vector3 m_Offset;

    [SerializeField]
    [Tooltip("The X coordinate of the left-most tile in the map")]
    private float m_MinMapX;

    [SerializeField]
    [Tooltip("The X coordinate of the right-most tile in the map")]
    private float m_MaxMapX;

    [SerializeField]
    [Tooltip("The Y coordinate of the lowest tile in the map")]
    private float m_MinMapY;

    [SerializeField]
    [Tooltip("The Y coordinate of the highest tile in the map")]
    private float m_MaxMapY;

    private float m_MinCamX, m_MaxCamX, m_MinCamY, m_MaxCamY;

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

    /// <summary>The distance between the top most and bottom most points visible to the camera</summary>
    public float vertLength { get; private set; }

    /// <summary>The distance between the left most and right most points visible to the camera</summary>
    public float horizLength { get; private set; }

    private void Awake()
    {
        m_Players = FindObjectsOfType<Coop.Platformer2DUserControl>();

        vertLength = GetComponent<Camera>().orthographicSize;
        horizLength = vertLength * Screen.width / Screen.height;

        m_MinCamX = m_MinMapX + horizLength;
        m_MaxCamX = m_MaxMapX - horizLength;
        m_MinCamY = m_MinMapY + vertLength;
        m_MaxCamY = m_MaxMapY - vertLength;
    }

    private void LateUpdate()
    {
        Vector3 avgPos = m_Players.Select(p => p.transform.position).Aggregate((total, next) => total += next) / m_Players.Length;

        Vector3 clamped = avgPos + m_Offset;
        clamped.x = Mathf.Clamp(clamped.x, m_MinCamX, m_MaxCamX);
        clamped.y = Mathf.Clamp(clamped.y, m_MinCamY, m_MaxCamY);
        transform.position = clamped;
    }
}
