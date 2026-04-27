using System;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public IInteractable interactable;
    public GameObject toggle;

    public GameObject text;

    private bool done = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            text.SetActive(true);
            player.interactable = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            text.SetActive(false);
            if (player.interactable == this)
            {
                player.interactable = null;
            }
        }
    }

    public void Activate()
    {
        if(done)
        {
            return;
        }
        text.SetActive(false);
        if (interactable != null)
        {
            interactable.Interact();
        }
        done = true;
        if (toggle != null)
        {
            toggle.SetActive(toggle.activeInHierarchy);
        }
    }
}

public abstract class IInteractable : MonoBehaviour
{
    public virtual void Interact()
    {
        
    }
}
