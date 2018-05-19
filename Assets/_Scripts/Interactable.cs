using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private UnityEvent m_OnInteract = new UnityEvent();

    public void Interact()
    {
        m_OnInteract.Invoke();
    }
}
