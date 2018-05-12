using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ThermalSensitive : MonoBehaviour
{
    private enum ThermalState { Melted, Frozen };

    [SerializeField]
    private Sprite m_FrozenSprite;

    [SerializeField]
    private Sprite m_MeltedSprite;

    [SerializeField]
    private ThermalState m_State = ThermalState.Melted;

    private SpriteRenderer m_Renderer;
    private BoxCollider2D m_Coll;

    private void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Coll = GetComponent<BoxCollider2D>();
        m_Coll.isTrigger = m_State == ThermalState.Melted;
    }

    public void Freeze()
    {
        if(m_State == ThermalState.Frozen)
            return;

        m_State = ThermalState.Frozen;
        m_Renderer.sprite = m_FrozenSprite;
        m_Coll.isTrigger = false;
    }

    public void Melt()
    {
        if(m_State == ThermalState.Melted)
            return;

        m_State = ThermalState.Melted;
        m_Renderer.sprite = m_MeltedSprite;
        m_Coll.isTrigger = true;
    }
}
