using UnityEngine;
using UnityEngine.Events;
using Coop;

namespace Coop
{
	public class Interactable : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent m_OnInteract = new UnityEvent();

		[SerializeField]
		internal Vector2 m_IconOffset = Vector2.zero;

		public void Interact()
		{
			m_OnInteract.Invoke();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.gameObject.GetComponent<CoopCharacter2D>() != null)
				CoopGameManager.ShowInteractIcon(this);
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if(collision.gameObject.GetComponent<CoopCharacter2D>() != null)
				CoopGameManager.HideInteractIcon();
		}
	}
}
