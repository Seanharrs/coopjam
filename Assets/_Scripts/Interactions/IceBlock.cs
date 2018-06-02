using UnityEngine;

namespace Coop {
  public class IceBlock : MonoBehaviour, IThermalSensitive
  {
    private int m_FreezeLevel = 5;

    [SerializeField, Tooltip("Order sprites from most frozen (index 0) to least frozen (highest index)")]
    private Sprite[] m_StateSprites;

    private void Awake()
    {
      m_FreezeLevel = m_StateSprites.Length;
    }

    public void OnThermalHit_Cool(Gun gun, FiringState weaponType)
    {
      if (m_FreezeLevel < m_StateSprites.Length)
      {
        m_FreezeLevel++;
        UpdateSprite();
      }
      return;
    }

    public void OnThermalHit_Heat(Gun gun, FiringState weaponType)
    {
      m_FreezeLevel--;
      if (m_FreezeLevel == 0)
        Destroy(gameObject);
      else
        UpdateSprite();
    }

    private void UpdateSprite()
    {
      Debug.Log("Freeze Level: " + m_FreezeLevel);
      GetComponent<SpriteRenderer>().sprite = m_StateSprites[m_StateSprites.Length - m_FreezeLevel];
    }
  }
}