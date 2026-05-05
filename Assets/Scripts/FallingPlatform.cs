using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;
    public float respawnTime = 5f;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 7)
        {
            Invoke("Crumble", fallDelay);
        }
    }

    private void Crumble()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
        Invoke(nameof(Toggle), respawnTime);
    }
    
    private void Toggle()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.GetComponent<Collider>().enabled = true;
    }
}
