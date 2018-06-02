using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Coop
{
  public class MessageZone : MonoBehaviour
  {

    [SerializeField, TextArea]
    internal string m_Message;

    [SerializeField, Range(1, 10)]
    internal float m_MessageDisplayTime = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.GetComponent<CoopCharacter2D>())
      {
        StartCoroutine(CoopGameManager.ShowMessage(m_Message, m_MessageDisplayTime));
      }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      if (collision.GetComponent<CoopCharacter2D>())
        CoopGameManager.HideMessage();
    }

  }
}