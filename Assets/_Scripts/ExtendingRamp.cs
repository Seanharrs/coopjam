using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [RequireComponent(typeof (PolygonCollider2D))]
  public class ExtendingRamp : MonoBehaviour
  {

    [SerializeField]
    private PhysicsMaterial2D m_NormalRampMaterial;
    [SerializeField]
    private PhysicsMaterial2D m_SlipperyRampMaterial;

    private PolygonCollider2D m_Collider;
    private Animator m_Animator;

    void Awake() 
    {
      m_Collider = GetComponent<PolygonCollider2D>();
      m_Animator = GetComponent<Animator>();
    }

    public void Extend()
    {
      m_Animator.SetTrigger("Extend");
    }
    public void Retract()
    {
      m_Animator.SetTrigger("Retract");
    }

    public void MakeSlippery()
    {
      ChangeMaterial(true);
    }
    public void MakeWalkable()
    {
      ChangeMaterial(false);
    }

    // This C# function can be called by an Animation Event
    public void ChangeMaterial(bool isSlippery = false)
    {
      if(isSlippery)
      {
        m_Collider.sharedMaterial = m_SlipperyRampMaterial;
      }
      else
        m_Collider.sharedMaterial = m_NormalRampMaterial;
    }
  }
}