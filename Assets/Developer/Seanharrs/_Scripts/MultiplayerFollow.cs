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

    [SerializeField]
    private Transform m_BottomLeftIndicator;
    [SerializeField]
    private Transform m_TopRightIndicator;

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
    public float vertLength { get; private set; }

    /// <summary>The distance between the left most and right most points visible to the camera.</summary>
    public float horizLength { get; private set; }

    private void Awake()
    {
        AcquirePlayerRefs();

        if(m_BottomLeftIndicator)
        {
            m_MinMapX = m_BottomLeftIndicator.position.x;
            m_MinMapY = m_BottomLeftIndicator.position.y;
            m_BottomLeftIndicator.gameObject.SetActive(false);
        }
        if(m_TopRightIndicator)
        {
            m_MaxMapX = m_TopRightIndicator.position.x;
            m_MaxMapY = m_TopRightIndicator.position.y;
            m_TopRightIndicator.gameObject.SetActive(false);
        }

        vertLength = GetComponent<Camera>().orthographicSize;
        horizLength = vertLength * Screen.width / Screen.height;

        m_MinCamX = m_MinMapX + horizLength;
        m_MaxCamX = m_MaxMapX - horizLength;
        m_MinCamY = m_MinMapY + vertLength;
        m_MaxCamY = m_MaxMapY - vertLength;
    }

    internal void AcquirePlayerRefs()
    {
        m_Players = FindObjectsOfType<Coop.Platformer2DUserControl>();
    }

    private void LateUpdate()
    {
        if(m_Players == null || m_Players.Count() == 0) return;
        Vector3 avgPos = m_Players.Select(p => p.transform.position).Aggregate((total, next) => total += next) / m_Players.Length;

        Vector3 clamped = avgPos + m_Offset;
        clamped.x = Mathf.Clamp(clamped.x, m_MinCamX, m_MaxCamX);
        clamped.y = Mathf.Clamp(clamped.y, m_MinCamY, m_MaxCamY);
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
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        if(constrainAxisY)
        {
            float minY = camPos.y - vertLength + spriteBounds.y;
            float maxY = camPos.y + vertLength - spriteBounds.y;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        //TODO kill player if they fall off the bottom of the camera?
        //     how do we then handle reverse gravity sending player off the top?

        return pos;
    }
}
